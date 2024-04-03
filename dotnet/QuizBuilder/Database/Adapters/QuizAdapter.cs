using Dapper;
using QuizBuilder.Models;

namespace QuizBuilder.Database.Adapters
{
    public class QuizAdapter : BaseAdapter
    {
        public static string ColumnMapper = "id as \"Id\", title as \"Title\", ownerid as \"OwnerId\", published as \"Published\"";
        public async Task CreateQuiz(Quiz quiz) 
        {
            using (var conn = OpenConnection(_connectionString)) 
            {
                var sqlString = $"INSERT INTO quiz (id, title, ownerid, published) VALUES  (@id, @title, @ownerid, @published);";
                await conn.ExecuteAsync(sqlString, new
                {
                    id = quiz.Id,
                    ownerid = quiz.OwnerId,
                    title = quiz.Title,
                    published = quiz.Published
                });
            }
        }

        public async Task UpdateQuiz(Quiz quiz)
        {
            using (var conn = OpenConnection(_connectionString))
            {
                var sqlString = "UPDATE quiz SET title = @title, published = @published WHERE id = @id;";
                await conn.ExecuteAsync(sqlString, new
                {
                    id = quiz.Id,
                    title = quiz.Title,
                    published = quiz.Published
                });
            }
        }

        public Quiz GetQuiz(string id)
        {
            using (var conn = OpenConnection(_connectionString))
            {
                var sqlString = $"SELECT {ColumnMapper} FROM quiz WHERE id = @id";
                return conn.QueryFirst<Quiz>(sqlString, new { id = id });
            }
        }

        public IEnumerable<Quiz> GetQuizzes(int top, int skip)
        {
            using (var conn = OpenConnection(_connectionString))
            {
                var sqlString = $"SELECT {ColumnMapper} FROM quiz LIMIT @top OFFSET @skip";
                return conn.Query<Quiz>(sqlString, new { top = top, skip = skip });
            }
        }

        public async Task DeleteQuiz(string id)
        {
            using (var conn = OpenConnection(_connectionString))
            {
                var sqlString = $"DELETE FROM quiz WHERE id = @id";
                await conn.ExecuteAsync(sqlString, new { id = id });
            }
        }
    }
}
