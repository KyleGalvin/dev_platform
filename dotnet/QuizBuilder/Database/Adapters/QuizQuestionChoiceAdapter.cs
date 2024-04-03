using Dapper;
using QuizBuilder.Models;

namespace QuizBuilder.Database.Adapters
{
    public class QuizQuestionChoiceAdapter : BaseAdapter
    {
        public static string ColumnMapper = "id as \"Id\", title as \"Title\", quizquestionid as \"QuizQuestionId\", iscorrect as \"IsCorrect\"";
        public async Task CreateQuizQuestionChoice(QuizQuestionChoice quizQuestionChoice)
        {
            using (var conn = OpenConnection(_connectionString))
            {
                var sqlString = "INSERT INTO quizquestionchoice (id, quizquestionid, title, iscorrect) VALUES  (@id, @quizquestionid, @title, @iscorrect);";
                await conn.ExecuteAsync(sqlString, new
                {
                    id = quizQuestionChoice.Id,
                    quizquestionid = quizQuestionChoice.QuizQuestionId,
                    title = quizQuestionChoice.Title,
                    iscorrect = quizQuestionChoice.IsCorrect
                });
            }
        }

        public async Task DeleteQuizQuestionChoice(string id)
        {
            using (var conn = OpenConnection(_connectionString))
            {
                var sqlString = $"DELETE FROM quizquestionchoice WHERE id = @id";
                await conn.ExecuteAsync(sqlString, new { id = id });
            }
        }

        public async Task UpdateQuizQuestionChoice(QuizQuestionChoice quizQuestionChoice)
        {
            using (var conn = OpenConnection(_connectionString))
            {
                var sqlString = "UPDATE quizquestionchoice SET title = @title, iscorrect = @iscorrect WHERE id = @id;";
                await conn.ExecuteAsync(sqlString, new
                {
                    id = quizQuestionChoice.Id,
                    title = quizQuestionChoice.Title,
                    iscorrect = quizQuestionChoice.IsCorrect
                });
            }
        }

        public QuizQuestionChoice GetQuizQuestionChoice(string id)
        {
            using (var conn = OpenConnection(_connectionString))
            {
                var sqlString = $"SELECT {ColumnMapper} FROM quizquestionchoice WHERE id = @id";
                return conn.QueryFirst<QuizQuestionChoice>(sqlString, new { id = id });
            }
        }
        public IEnumerable<QuizQuestionChoice> GetChoicesForQuestion(string id)
        {
            using (var conn = OpenConnection(_connectionString))
            {
                var sqlString = $"SELECT {ColumnMapper} FROM quizquestionchoice WHERE quizquestionid = @id";
                return conn.Query<QuizQuestionChoice>(sqlString, new { id = id });
            }
        }
        
    }
}
