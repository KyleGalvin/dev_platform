using QuizBuilder.Models;
using Refit;
using System.Net;

namespace XUnitIntegrationTests
{
    internal static class AssertRefitExtensions
    {
        internal static async Task ThrowsNotFoundAsync(Func<Task> testCode)
        {
            try
            {
                await testCode();
            }
            catch (ApiException ex) 
            {
                Assert.Equal(HttpStatusCode.NotFound , ex.StatusCode);
                await CheckErrors(ex);
            }
            Assert.Fail("Refit did not find a failed response");
        }

        internal static async Task ThrowsBadRequestAsync(Func<Task> testCode)
        {
            try
            {
                await testCode();
            }
            catch (ApiException ex)
            {
                Assert.Equal(HttpStatusCode.BadRequest, ex.StatusCode);
                await CheckErrors(ex);
            }
            Assert.Fail("Refit did not find a failed response");
        }

        internal static async Task ThrowsForbiddenAsync(Func<Task> testCode)
        {
            try
            {
                await testCode();
            }
            catch (ApiException ex)
            {
                Assert.Equal(HttpStatusCode.Forbidden, ex.StatusCode);
            }
            Assert.Fail("Refit did not find a failed response");
        }

        internal static async Task ThrowsUnauthorizedAsync(Func<Task> testCode)
        {
            try
            {
                await testCode();
            }
            catch (ApiException ex)
            {
                Assert.Equal(HttpStatusCode.Unauthorized, ex.StatusCode);
            }
            Assert.Fail("Refit did not find a failed response");
        }

        private static async Task CheckErrors(ApiException ex) 
        {
            var errors = await ex.GetContentAsAsync<List<ErrorResult>>();
            Assert.NotNull(errors);
            Assert.NotEmpty(errors);
        }
    }
}
