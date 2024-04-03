using FluentMigrator;
using FluentMigrator.Expressions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace QuizBuilder.Database.Migrations
{
    [Migration(2)]
    public class CreateQuizQuestionTable : Migration
    {
        public override void Up()
        {
            Create.Table("quizquestion").WithColumn("id").AsString();
            Alter.Table("quizquestion").AddColumn("quizid").AsString();
            Alter.Table("quizquestion").AddColumn("title").AsString();

            Create.PrimaryKey("pk_quizquestion_id").OnTable("quizquestion").Column("id");

            Create.ForeignKey()
                .FromTable("quizquestion").ForeignColumn("quizid")
                .ToTable("quiz").PrimaryColumn("id");
        }

        public override void Down()
        {
            Delete.Table("quizquestion");
        }
    }
}
