using FluentMigrator;
using FluentMigrator.Expressions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace QuizBuilder.Database.Migrations
{
    [Migration(3)]
    public class CreateQuizQuestionChoiceTable : Migration
    {
        public override void Up()
        {
            Create.Table("quizquestionchoice").WithColumn("id").AsString();
            Alter.Table("quizquestionchoice").AddColumn("quizquestionid").AsString();
            Alter.Table("quizquestionchoice").AddColumn("title").AsString();
            Alter.Table("quizquestionchoice").AddColumn("iscorrect").AsBoolean();

            Create.PrimaryKey("pk_quizquestionchoice_id").OnTable("quizquestionchoice").Column("id");

            Create.ForeignKey()
                .FromTable("quizquestionchoice").ForeignColumn("quizquestionid")
                .ToTable("quizquestion").PrimaryColumn("id");

        }

        public override void Down()
        {
            Delete.Table("quizquestionchoice");
        }
    }
}
