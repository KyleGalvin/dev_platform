namespace QuizBuilder.Util
{
    public static class EnvironmentVars
    {
        public static string? GetKeycloakPublicKey()
        {
            return Environment.GetEnvironmentVariable("KEYCLOAK_PUBLIC_KEY");
        }

        public static string? GetKeycloakApiSecret()
        {
            return Environment.GetEnvironmentVariable("QUIZBUILDER_OAUTH_CLIENT_SECRET");
        }

        public static string? GetKeycloakClientId()
        {
            return Environment.GetEnvironmentVariable("QUIZBUILDER_CLIENTID");
        }

        public static string? GetKeycloakHost()
        {
            return Environment.GetEnvironmentVariable("KEYCLOAK_HOSTNAME");
        }

        public static string? GetServicePort()
        {
            return Environment.GetEnvironmentVariable("QUIZBUILDER_PORT");
        }

        public static string? GetQuizbuilderHostname()
        {
            return Environment.GetEnvironmentVariable("QUIZBUILDER_HOSTNAME");
        }

        public static string? GetPostgresUser()
        {
            return Environment.GetEnvironmentVariable("DB_USER");
        }

        public static string? GetPostgresPassword()
        {
            return Environment.GetEnvironmentVariable("DB_PASSWORD");
        }

        public static string? GetPostgresSchema()
        {
            return Environment.GetEnvironmentVariable("QUIZBUILDER_DB_SCHEMA");
        }

        public static string? GetDatabaseName()
        {
            return Environment.GetEnvironmentVariable("QUIZBUILDER_DB_NAME");
        }

        public static string? GetPostgresHost()
        {
            return Environment.GetEnvironmentVariable("DB_HOST");
        }
        public static string? GetPostgresPort()
        {
            return Environment.GetEnvironmentVariable("DB_PORT");
        }
    }
}
