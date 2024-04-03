using FluentMigrator;
using FluentMigrator.Expressions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace QuizBuilder.Database.Migrations
{
    [Migration(5)]
    public class AddMultiFieldToQuizQuestion : Migration
    {
        public override void Up()
        {
            Alter.Table("quizquestion").AddColumn("ismulti").AsBoolean();

        }

        public override void Down()
        {
            Delete.Column("ismulti").FromTable("quizquestion");
        }
    }
}
