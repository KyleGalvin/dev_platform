using FluentMigrator;
using FluentMigrator.Expressions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace QuizBuilder.Database.Migrations
{
    [Migration(4)]
    public class CreateQuizResponseTable : Migration
    {
        public override void Up()
        {
            Create.Table("quizresponse").WithColumn("id").AsString();
            Alter.Table("quizresponse").AddColumn("quizid").AsString();
            Alter.Table("quizresponse").AddColumn("quizquestionid").AsString();
            Alter.Table("quizresponse").AddColumn("quizquestionchoiceid").AsString();
            Alter.Table("quizresponse").AddColumn("ownerid").AsString();

            Create.PrimaryKey("pk_quizresponse_id").OnTable("quizresponse").Column("id");

            Create.ForeignKey()
                .FromTable("quizresponse").ForeignColumn("quizid")
                .ToTable("quiz").PrimaryColumn("id");

            Create.ForeignKey()
                .FromTable("quizresponse").ForeignColumn("quizquestionid")
                .ToTable("quizquestion").PrimaryColumn("id");

            Create.ForeignKey()
                .FromTable("quizresponse").ForeignColumn("quizquestionchoiceid")
                .ToTable("quizquestionchoice").PrimaryColumn("id");

        }

        public override void Down()
        {
            Delete.Table("quizresponse");
        }
    }
}
