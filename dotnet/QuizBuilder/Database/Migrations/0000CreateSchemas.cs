using FluentMigrator;
using Microsoft.AspNetCore.Http.HttpResults;

namespace QuizBuilder.Database.Migrations
{
    [Migration(0)]
    public class CreateSchemas : Migration
    {
        public override void Up()
        {
            Create.Schema("keycloak");
            Create.Schema("gitea");
        }

        public override void Down()
        {
            Delete.Schema("keycloak");
            Create.Schema("gitea");
        }
    }
}
