using Dapper;
using QuizBuilder.Models;

namespace QuizBuilder.Database.Adapters
{
    public class QuizResponseAdapter : BaseAdapter
    {
        public static string ColumnMapper = "id as \"Id\", quizid as \"QuizId\", quizquestionid as \"QuizQuestionId\", quizquestionchoiceid as \"QuizQuestionChoiceId\", ownerid as \"OwnerId\"";

        public async Task CreateQuizResponseDetail(QuizResponseDetail quizResponseDetail)
        {
            using (var conn = OpenConnection(_connectionString))
            {
                var sqlString = "INSERT INTO quizresponse (id, quizid, quizquestionid, quizquestionchoiceid, ownerid) VALUES(@id, @quizid, @quizquestionid, @quizquestionchoiceid, @ownerid);";
                await conn.ExecuteAsync(sqlString, new
                {
                    id = quizResponseDetail.Id,
                    quizid = quizResponseDetail.QuizId,
                    quizquestionid = quizResponseDetail.QuizQuestionId,
                    quizquestionchoiceid = quizResponseDetail.QuizQuestionChoiceId,
                    ownerid = quizResponseDetail.OwnerId
                });
            }
        }

        public IEnumerable<QuizResponseDetail> GetQuizResponseDetails(string quizId, string ownerId)
        {
            using (var conn = OpenConnection(_connectionString))
            {
                var sqlString = "SELECT * FROM quizresponse WHERE quizid = @quizid and ownerid = @ownerid;";
                return conn.Query<QuizResponseDetail>(sqlString, new
                {
                    quizid = quizId,
                    ownerid = ownerId
                });
            }
        }

        public IEnumerable<string> GetMyAnsweredQuizzes(int top, int skip, string ownerId)
        {
            using (var conn = OpenConnection(_connectionString))
            {
                var sqlString = "SELECT DISTINCT quizid FROM quizresponse WHERE ownerid = @ownerid LIMIT @top OFFSET @skip;";
                return conn.Query<string>(sqlString, new
                {
                    top = top,
                    skip = skip,
                    ownerid = ownerId
                });
            }
        }
        public async Task DeleteQuizResponse(string quizId, string ownerId)
        {
            using (var conn = OpenConnection(_connectionString))
            {
                var sqlString = $"DELETE FROM quizresponse WHERE quizid = @quizid and ownerid = @ownerid;";
                await conn.ExecuteAsync(sqlString, new { quizid = quizId, ownerid = ownerId });
            }
        }
    }
}
