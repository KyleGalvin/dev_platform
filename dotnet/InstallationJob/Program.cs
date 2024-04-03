using Dapper;
using Npgsql;
using Refit;
using System.Data;
using UserManager.Sdk.Clients;
using UserManager.Sdk.Models;

public class Program
{
    public static IDbConnection OpenDatabaseConnection(string connStr)
    {
        var conn = new NpgsqlConnection(connStr);
        conn.Open();
        return conn;
    }

    private static async void CreateDatabaseIfNotExists(IDbConnection conn, string databaseName)
    {
        var giteaquery = $"SELECT datname FROM pg_catalog.pg_database WHERE datname = '{databaseName}'";
        Console.WriteLine($"Checking status of the {databaseName} database");

        var existingdb = conn.Query<string>(giteaquery);
        if (!existingdb.Any())
        {
            conn.Execute($"create database {databaseName};");
            Console.WriteLine($"{databaseName} database created");
        }
        else
        {
            Console.WriteLine($"{databaseName} database already exists");
        }
    }

    private static async Task<string> CreateKeycloakClientIfNotExists(IOAuth2Client client, string token, string realm, ClientRequest request) 
    {
        Console.WriteLine($"Checking if client {request.clientId} exists");
        var existingClients = await client.GetRealmClients(token, realm, request.clientId);
        if (!existingClients.Any(c => c.clientId == request.clientId))
        {
            Console.WriteLine($"Creating client {request.clientId}");
            var result = await client.CreateRealmClient(token, realm, request);
            var createdResourceUri = result.Headers.First(h => h.Key == "Location").Value.FirstOrDefault();
            var id = createdResourceUri.Substring(createdResourceUri.LastIndexOf("/") + 1, createdResourceUri.Length - createdResourceUri.LastIndexOf("/") - 1);
            Console.WriteLine($"Client creation returned {result.StatusCode}  {id}");
            return id;
        }
        else 
        {
            var clientResponse = existingClients.First(c => c.clientId == request.clientId);
            Console.WriteLine($"Client {request.clientId} already exists with Id {clientResponse.Id}");
            return clientResponse.Id;
        }
    }

    private static async Task<string> GetAdminToken(IOAuth2Client client) 
    {
        Console.WriteLine("Getting admin token");
        var tokenRequest = new TokenRequest()
        {
            username = Environment.GetEnvironmentVariable("KEYCLOAK_ADMIN_USERNAME"),
            password = Environment.GetEnvironmentVariable("KEYCLOAK_ADMIN_PASSWORD"),
            client_id = "admin-cli",
            grant_type = "password"
        };
        var keycloakAccessToken = await client.GetKeycloakAccessToken("master", tokenRequest);
        return keycloakAccessToken.access_token;
        
    }
    private static async Task CreateRealmIfNotExists(IOAuth2Client client, string token, string realmName) 
    {
        Console.WriteLine($"Configuring {realmName} realm");
        var existingRealm = await client.GetRealm(token, realmName);
        if(existingRealm.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            Console.WriteLine($"Creating Realm {realmName}");
            var result = await client.CreateRealm(token, new RealmRequest() { realm = realmName});
            Console.WriteLine($"Realm creation returned {result.StatusCode}");
        }
    }

    private static async Task AssignKeycloakClientRoleIfNotExists(IOAuth2Client client, string token, string realmName, string id, string roleName) 
    {

        var serviceAccount = await client.GetClientServiceAccount(token, realmName, id);
        //Console.WriteLine($"Service account {serviceAccount.Username} {serviceAccount.Id} found");
        var existingRoleMappings = await client.GetRoleMappings(token, realmName, serviceAccount.Id);
        var hasMapping = existingRoleMappings?.Content?.ClientMappings?.RealmManagement?.Mappings.Any(m => m.Name == roleName);
        if (!(hasMapping ?? false))
        {
            Console.WriteLine($"Role not found on client");

            var roles = await client.GetRole(token, realmName, serviceAccount.Id, roleName);
            //Console.WriteLine($"Role {roles.First().Id} {roles.First().ClientId} found");
            //Console.WriteLine($"Description {roles.First().Description}");
            var result = await client.MapRoleToClient(token, realmName, serviceAccount.Id, roles.First().ClientId, new List<RoleMapping>() 
            { 
                new RoleMapping() 
                {
                    Id = roles.First().Id,
                    Description = roles.First().Description,
                    Name = roles.First().Role
                }

            });
            Console.WriteLine($"Role mapping result {result.StatusCode}");
        }
        else 
        {
            Console.WriteLine($"Role found on client");
        }
    }

    private static async Task SetupKeycloak() 
    {
        var client = RestService.For<IOAuth2Client>($"http://{Environment.GetEnvironmentVariable("KEYCLOAK_HOSTNAME")}");
        var token = "bearer " + await GetAdminToken(client);

        await CreateRealmIfNotExists(client, token, "platformservices");
        var id = await CreateKeycloakClientIfNotExists(client, token, "platformservices", new ClientRequest()
        {
            clientId = "quizbuilder",
            secret = Environment.GetEnvironmentVariable("QUIZBUILDER_OAUTH_CLIENT_SECRET"),
            publicClient = false,
            serviceAccountsEnabled = true
        });

        await AssignKeycloakClientRoleIfNotExists(client, token, "platformservices", id, "manage-users");

        await CreateKeycloakClientIfNotExists(client, token, "platformservices", new ClientRequest()
        {
            clientId = "gitea",
            secret = Environment.GetEnvironmentVariable("GITEA_OAUTH_CLIENT_SECRET"),
            publicClient = false,
            serviceAccountsEnabled = true,
            redirectUris = new List<string>() {
                Environment.GetEnvironmentVariable("GITEA_REDIRECT_URI"),
            }
        });
    }

    private static void CreateDatabases() 
    {
        try
        {
            using (var conn = OpenDatabaseConnection(_connectionString))
            {
                Console.WriteLine("Connected to postgres database");

                CreateDatabaseIfNotExists(conn, Environment.GetEnvironmentVariable("GITEA_DB_NAME"));
                CreateDatabaseIfNotExists(conn, Environment.GetEnvironmentVariable("QUIZBUILDER_DB_NAME"));
                CreateDatabaseIfNotExists(conn, Environment.GetEnvironmentVariable("KEYCLOAK_DB_NAME"));
            }
        }
        //by throwing an uncaught exception, the kubernetes job will fail and be retried.
        catch (NpgsqlException)
        {
            Console.WriteLine("Cannot connect to postgres database");
            throw;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed for an unexpected reason: {e.Message}");
            throw;
        }
    }

    private static string _connectionString = $"Server={Environment.GetEnvironmentVariable("DB_HOST")};Port=5432;User Id={Environment.GetEnvironmentVariable("DB_USER")};Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};";
    
    
    static async Task Main(string[] args)
    {

        Console.WriteLine("Installing SeaSprig Studio application development platform");
        CreateDatabases();
        await SetupKeycloak();

    }
}
