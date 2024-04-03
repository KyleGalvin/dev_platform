using FluentMigrator;
using FluentMigrator.Expressions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace QuizBuilder.Database.Migrations
{
    [Migration(1)]
    public class CreateQuizTable : Migration
    {
        public override void Up()
        {
            Create.Table("quiz").WithColumn("id").AsString();
            Alter.Table("quiz").AddColumn("title").AsString();
            Alter.Table("quiz").AddColumn("ownerid").AsString();
            Alter.Table("quiz").AddColumn("published").AsBoolean();

            Create.PrimaryKey("pk_quiz_id").OnTable("quiz").Column("id");
        }

        public override void Down()
        {
            Delete.Table("quiz");
        }
    }
}
