using Dapper;
using QuizBuilder.Models;

namespace QuizBuilder.Database.Adapters
{
    public class QuizQuestionAdapter : BaseAdapter
    {
        public static string ColumnMapper = "id as \"Id\", title as \"Title\", quizid as \"QuizId\", ismulti as \"IsMulti\"";
        public async Task CreateQuizQuestion(QuizQuestion quizQuestion)
        {
            using (var conn = OpenConnection(_connectionString))
            {
                var sqlString = "INSERT INTO quizquestion (id, quizid, title, ismulti) VALUES  (@id, @quizid, @title, @ismulti);";
                await conn.ExecuteAsync(sqlString, new
                {
                    id = quizQuestion.Id,
                    quizid = quizQuestion.QuizId,
                    ismulti = quizQuestion.IsMulti,
                    title = quizQuestion.Title
                });
            }
        }

        public async Task DeleteQuizQuestion(string id)
        {
            using (var conn = OpenConnection(_connectionString))
            {
                var sqlString = $"DELETE FROM quizquestion WHERE id = @id";
                await conn.ExecuteAsync(sqlString, new { id = id });
            }
        }

        public async Task UpdateQuizQuestion(QuizQuestion quizQuestion)
        {
            using (var conn = OpenConnection(_connectionString))
            {
                var sqlString = "UPDATE quizquestion SET title = @title, ismulti = @ismulti WHERE id = @id;";
                await conn.ExecuteAsync(sqlString, new
                {
                    id = quizQuestion.Id,
                    ismulti = quizQuestion.IsMulti,
                    title = quizQuestion.Title,
                });
            }
        }

        public QuizQuestion GetQuizQuestion(string id)
        {
            using (var conn = OpenConnection(_connectionString))
            {
                var sqlString = $"SELECT {ColumnMapper} FROM quizquestion WHERE id = @id";
                return conn.QueryFirst<QuizQuestion>(sqlString, new { id = id });
            }
        }

        public IEnumerable<QuizQuestion> GetQuestionsForQuiz(string id)
        {
            using (var conn = OpenConnection(_connectionString))
            {
                var sqlString = $"SELECT {ColumnMapper} FROM quizquestion WHERE quizid = @id";
                return conn.Query<QuizQuestion>(sqlString, new { id = id });
            }
        }
    }
}
