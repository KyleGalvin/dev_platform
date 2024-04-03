using Microsoft.Extensions.DependencyInjection;
using QuizBuilder.Models;
using QuizBuilder.Services;
using Refit;
using System.Net;
using XUnitIntegrationTests.Clients;

namespace XUnitIntegrationTests
{
    public class QuizBuilderTests : IClassFixture<QuizBuilderFixture>
    {
        private QuizBuilderFixture Fixture;
        private IUserClient UserClient;

        public QuizBuilderTests(QuizBuilderFixture fixture) 
        {
            Fixture = fixture;
            var userClient = Fixture.ServiceProvider.GetService<IUserClient>();
            if(userClient == null) throw new NullReferenceException("Could not get IUserClient via DI. Is it registered?");
            UserClient =  userClient;
        }

        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public async Task<User> CreateUser(string email, string password) 
        {
            var httpResult = await UserClient.CreateUser(email, password);
            var user = httpResult.Content;
            Assert.Equal(email, user.Email);
            Assert.True(string.IsNullOrEmpty(user.Password)); //plaintext password does not come back
            Assert.False(user.Id.Length == 0); //keycloak assigns a UUID to reference the user
            return user;
        }

        public async Task<string> Login(User user, string password) 
        {

            var token = (await UserClient.Login(user.Email, "password")).Content;
            Assert.False(token.Length == 0);
            return "Bearer " + token;
        }

        public async Task<IApiResponse<Quiz>> CreateQuiz(string authToken, Quiz quiz = null) 
        {
            if (quiz == null) quiz = new Quiz() { Title = RandomString(20) };
            var client = Fixture.ServiceProvider.GetService<IQuizClient>();
            return await client.CreateQuiz(authToken, quiz);
        }

        public async Task<IApiResponse<QuizQuestion>> CreateQuizQuestion(string authToken, string quizId, QuizQuestion quizQuestion = null)
        {
            if (quizQuestion == null) quizQuestion = new QuizQuestion() {Title = RandomString(20) };
            quizQuestion.QuizId = quizId;
            var client = Fixture.ServiceProvider.GetService<IQuizQuestionClient>();
            return await client.CreateQuizQuestion(authToken, quizQuestion);
        }

        public async Task<IApiResponse<QuizQuestionChoice>> CreateQuizQuestionChoice(string authToken, string questionId, QuizQuestionChoice quizQuestionChoice = null)
        {
            if (quizQuestionChoice == null) quizQuestionChoice = new QuizQuestionChoice() { Title = RandomString(20) };
            quizQuestionChoice.QuizQuestionId = questionId;
            var client = Fixture.ServiceProvider.GetService<IQuizQuestionChoiceClient>();
            return await client.CreateQuizQuestionChoice(authToken, quizQuestionChoice);
        }

        public async Task<IApiResponse<QuizResponse>> CreateQuizResponse(string authToken, QuizResponse response)
        {
            var client = Fixture.ServiceProvider.GetService<IQuizResponseClient>();
            return await client.CreateQuizResponse(authToken, response);
        }

        public async Task<IApiResponse<QuizResponse>> GetQuizResponse(string authToken, string quizId, string ownerId)
        {
            var client = Fixture.ServiceProvider.GetService<IQuizResponseClient>();
            return await client.GetQuizResponse(authToken, quizId, ownerId);
        }

        public async Task<IApiResponse<QuizScore>> GetQuizResponseScore(string authToken, string quizId, string ownerId)
        {
            var client = Fixture.ServiceProvider.GetService<IQuizResponseClient>();
            return await client.GetQuizResponseScore(authToken, quizId, ownerId);
        }

        public async Task<IApiResponse<Quiz>> GetQuiz(string authToken, string id)
        {
            return await Fixture.ServiceProvider.GetService<IQuizClient>().GetQuiz(authToken, id);
        }

        public async Task<IApiResponse<IEnumerable<Quiz>>> GetQuizzes(string authToken, int top, int skip)
        {
            return await Fixture.ServiceProvider.GetService<IQuizClient>().GetQuizzes(authToken, top, skip);
        }

        public async Task<IApiResponse<IEnumerable<Quiz>>> GetMyAnsweredQuizzes(string authToken, int top, int skip)
        {
            return await Fixture.ServiceProvider.GetService<IQuizClient>().GetMyAnsweredQuizzes(authToken, top, skip);
        }

        public async Task<IApiResponse<QuizQuestion>> GetQuizQuestion(string authToken, string id)
        {
            return await Fixture.ServiceProvider.GetService<IQuizQuestionClient>().GetQuizQuestion(authToken, id);
        }

        public async Task<IApiResponse<QuizQuestionChoice>> GetQuizQuestionChoice(string authToken, string id)
        {
            return await Fixture.ServiceProvider.GetService<IQuizQuestionChoiceClient>().GetQuizQuestionChoice(authToken, id);
        }

        public async Task<IApiResponse<Quiz>> PublishQuiz(string authToken, string id)
        {
            return await Fixture.ServiceProvider.GetService<IQuizClient>().PublishQuiz(authToken, id);
        }

        public async Task<IApiResponse<Quiz>> UpdateQuiz(string authToken, Quiz quiz)
        {
            return await Fixture.ServiceProvider.GetService<IQuizClient>().UpdateQuiz(authToken, quiz);
        }

        public async Task<IApiResponse<QuizQuestion>> UpdateQuizQuestion(string authToken, QuizQuestion quizQuestion)
        {
            return await Fixture.ServiceProvider.GetService<IQuizQuestionClient>().UpdateQuizQuestion(authToken, quizQuestion);
        }

        public async Task<IApiResponse<QuizQuestionChoice>> UpdateQuizQuestionChoice(string authToken, QuizQuestionChoice quizQuestionChoice)
        {
            return await Fixture.ServiceProvider.GetService<IQuizQuestionChoiceClient>().UpdateQuizQuestionChoice(authToken, quizQuestionChoice);
        }

        public async Task<IApiResponse> DeleteQuiz(string authToken, string id)
        {
            return await Fixture.ServiceProvider.GetService<IQuizClient>().DeleteQuiz(authToken, id);
        }

        public async Task<IApiResponse> DeleteQuizQuestion(string authToken, string id)
        {
            return await Fixture.ServiceProvider.GetService<IQuizQuestionClient>().DeleteQuizQuestion(authToken, id);
        }

        public async Task<IApiResponse> DeleteQuizQuestionChoice(string authToken, string id)
        {
            return await Fixture.ServiceProvider.GetService<IQuizQuestionChoiceClient>().DeleteQuizQuestionChoice(authToken, id);
        }

        public async Task<IApiResponse> DeleteQuizResponse(string authToken, string quizId, string ownerId)
        {
            return await Fixture.ServiceProvider.GetService<IQuizResponseClient>().DeleteQuizResponse(authToken, quizId, ownerId);
        }

        [Fact]
        public async Task CanCreateUserAndLogin()
        {
            var user = await CreateUser("testuser1@seasprig.ca", "password");
            var token = await Login(user, "password");

            //cleanup test
            var response =await UserClient.DeleteUser(token, user.Id);
        }

        [Fact]
        public async Task AuthRejectsBadTokens()
        {
            var user = await CreateUser("testuser1@seasprig.ca", "password");
            var token = await Login(user, "password");

            //invalid token
            var result = await UserClient.DeleteUser("", user.Id);
            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);

            //cleanup test
            await UserClient.DeleteUser(token, user.Id);
        }

        [Fact]
        public async Task UserCanCreateQuiz()
        {
            var user = await CreateUser("testuser1@seasprig.ca", "password");
            var token = await Login(user, "password");

            var quiz = (await CreateQuiz(token, new Quiz()
            {
                Title = "MyFirstQuiz"
            })).Content;

            Assert.False(string.IsNullOrEmpty(quiz.Title));
            Assert.False(string.IsNullOrEmpty(quiz.OwnerId));
            Assert.False(quiz.Published);
            Assert.Empty(quiz.Questions); //these dont come by default on the quiz get/create. Fetch them separate

            var getEndpointResult = (await GetQuiz(token, quiz.Id)).Content;

            Assert.Equal(quiz.Title, getEndpointResult.Title);
            Assert.Equal(quiz.Id, getEndpointResult.Id);
            Assert.Equal(quiz.OwnerId, getEndpointResult.OwnerId);
            Assert.Equal(quiz.Published, getEndpointResult.Published);
            Assert.Equal(quiz.Questions, getEndpointResult.Questions);

            var newTitle = "updated title";
            quiz.Title = newTitle;

            var updatedQuiz = (await UpdateQuiz(token, quiz)).Content;

            Assert.Equal(newTitle, updatedQuiz.Title);

            await DeleteQuiz(token, quiz.Id);

            //cleanup test
            await UserClient.DeleteUser(token, user.Id);
        }

        [Fact]
        public async Task UserCanCreateQuizWithQuestions()
        {
            var user = await CreateUser("testuser1@seasprig.ca", "password");
            var token = await Login(user, "password");
            var quiz = (await CreateQuiz(token)).Content;


            var question = (await CreateQuizQuestion(token, quiz.Id, new QuizQuestion()
            {
                Title = "My Quiz Question"
            })).Content;

            Assert.False(string.IsNullOrEmpty(question.Title));
            Assert.False(string.IsNullOrEmpty(question.Id));
            Assert.Equal(quiz.Id, question.QuizId);
            Assert.Empty(question.AnswerChoice); //these dont come by default on the question get/create. Fetch them separate

            var getEndpointResult = (await GetQuizQuestion(token, question.Id)).Content;

            Assert.Equal(question.Title, getEndpointResult.Title);
            Assert.Equal(question.Id, getEndpointResult.Id);
            Assert.Equal(question.AnswerChoice, getEndpointResult.AnswerChoice);

            var newTitle = "Updated Quiz Question";
            question.Title = newTitle;
            var updatedQuestion = (await UpdateQuizQuestion(token, question)).Content;

            Assert.Equal(newTitle, updatedQuestion.Title);

            await DeleteQuizQuestion(token, question.Id);

            //cleanup test
            await DeleteQuiz(token, quiz.Id);
            await UserClient.DeleteUser(token, user.Id);
        }

        [Fact]
        public async Task UserCanCreateQuizWithQuestionsAndChoices()
        {
            var user = await CreateUser("testuser1@seasprig.ca", "password");
            var token = await Login(user, "password");
            var quiz = (await CreateQuiz(token)).Content;
            var question = (await CreateQuizQuestion(token, quiz.Id)).Content;


            var questionChoice = (await CreateQuizQuestionChoice(token, question.Id, new QuizQuestionChoice()
            {
                Title = "Choice 1",
                IsCorrect = true
            })).Content;

            Assert.False(string.IsNullOrEmpty(questionChoice.Title));
            Assert.False(string.IsNullOrEmpty(questionChoice.Id));
            Assert.True(questionChoice.IsCorrect);

            var getEndpointResult = (await GetQuizQuestionChoice(token, questionChoice.Id)).Content;

            Assert.Equal(questionChoice.Title, getEndpointResult.Title);
            Assert.Equal(questionChoice.Id, getEndpointResult.Id);
            Assert.Equal(questionChoice.IsCorrect, getEndpointResult.IsCorrect);

            var newTitle = "Choice 1 revised";
            questionChoice.Title = newTitle;
            var updatedQuestionChoice = (await UpdateQuizQuestionChoice(token, questionChoice)).Content;

            Assert.Equal(newTitle, updatedQuestionChoice.Title);

            await DeleteQuizQuestionChoice(token, questionChoice.Id);

            //cleanup test
            await DeleteQuizQuestion(token, question.Id);
            await DeleteQuiz(token, quiz.Id);
            await UserClient.DeleteUser(token, user.Id);
        }

        [Fact]
        public async Task QuizCannotHaveOverTenQuestions()
        {
            var user = await CreateUser("testuser1@seasprig.ca", "password");
            var token = await Login(user, "password");
            var quiz = (await CreateQuiz(token)).Content;
            var questions = new List<QuizQuestion>();
            for (var i = 0; i < 10; i++) 
            {
                questions.Add((await CreateQuizQuestion(token, quiz.Id)).Content);
            }

            //this one should fail
            var result = await CreateQuizQuestion(token, quiz.Id);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            

            //cleanup test
            foreach (var q in questions)
            {
                await DeleteQuizQuestion(token, q.Id);
            }
            await DeleteQuiz(token, quiz.Id);
            await UserClient.DeleteUser(token, user.Id);
        }

        [Fact]
        public async Task QuestionCannotHaveOverFiveChoices()
        {
            var user = await CreateUser("testuser1@seasprig.ca", "password");
            var token = await Login(user, "password");
            var quiz = (await CreateQuiz(token)).Content;
            var question = (await CreateQuizQuestion(token, quiz.Id)).Content;
            var choices = new List<QuizQuestionChoice>();

            for (var i = 0; i < 5; i++)
            {
                choices.Add((await CreateQuizQuestionChoice(token, question.Id)).Content);
            }

            //this one should fail
            var response = await CreateQuizQuestionChoice(token, question.Id);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            //cleanup test
            foreach (var c in choices) 
            {
                await DeleteQuizQuestionChoice(token, c.Id);
            }
            await DeleteQuizQuestion(token, question.Id);
            await DeleteQuiz(token, quiz.Id);
            await UserClient.DeleteUser(token, user.Id);
        }

        [Fact]
        public async Task QuizCanBePublished()
        {
            var user = await CreateUser("testuser1@seasprig.ca", "password");
            var token = await Login(user, "password");
            var quiz = (await CreateQuiz(token)).Content;
            var question = (await CreateQuizQuestion(token, quiz.Id)).Content;
            var choices = new List<QuizQuestionChoice>();

            for (var i = 0; i < 2; i++)
            {
                choices.Add((await CreateQuizQuestionChoice(token, question.Id)).Content);
            }

            var publishedQuiz = (await PublishQuiz(token, quiz.Id)).Content;
            Assert.True(publishedQuiz.Published);

            var getQuiz = (await GetQuiz(token, quiz.Id)).Content;
            Assert.True(getQuiz.Published);

            //cleanup test
            foreach (var c in choices)
            {
                await DeleteQuizQuestionChoice(token, c.Id);
            }
            await DeleteQuizQuestion(token, question.Id);
            await DeleteQuiz(token, quiz.Id);
            await UserClient.DeleteUser(token, user.Id);
        }

        [Fact]
        public async Task QuizWithoutQuestionsCannotBePublished()
        {
            var user = await CreateUser("testuser1@seasprig.ca", "password");
            var token = await Login(user, "password");
            var quiz = (await CreateQuiz(token)).Content;

            //this one should fail
            var result = await PublishQuiz(token, quiz.Id);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

            await DeleteQuiz(token, quiz.Id);
            await UserClient.DeleteUser(token, user.Id);
        }

        [Fact]
        public async Task QuizQuestionWithoutChoicesCannotBePublished()
        {
            var user = await CreateUser("testuser1@seasprig.ca", "password");
            var token = await Login(user, "password");
            var quiz = (await CreateQuiz(token)).Content; 
            var question = (await CreateQuizQuestion(token, quiz.Id)).Content;

            //this one should fail
            var result = await PublishQuiz(token, quiz.Id);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

            await DeleteQuizQuestion(token, question.Id);
            await DeleteQuiz(token, quiz.Id);
            await UserClient.DeleteUser(token, user.Id);
        }

        [Fact]
        public async Task UserCanResponseToPublishedQuiz()
        {
            //create published quiz with user1
            var user = await CreateUser("testuser1@seasprig.ca", "password");
            var token = await Login(user, "password");
            var quiz = (await CreateQuiz(token)).Content;
            var question = (await CreateQuizQuestion(token, quiz.Id)).Content;
            var choices = new List<QuizQuestionChoice>();

            for (var i = 0; i < 2; i++)
            {
                choices.Add((await CreateQuizQuestionChoice(token, question.Id)).Content);
            }
            var publishedQuiz = await PublishQuiz(token, quiz.Id);


            //response to quiz with user2
            var alternateUser = await CreateUser("testuser2@seasprig.ca", "password");
            var token2 = await Login(alternateUser, "password");

            var quizResponse = new QuizResponse()
            {
                QuizId = quiz.Id,
                QuizResponseDetails = new List<QuizResponseDetail>()
                {
                    new QuizResponseDetail()
                    {
                        QuizId = quiz.Id,
                        QuizQuestionId = question.Id,
                        QuizQuestionChoiceId = choices.First().Id,
                        OwnerId = alternateUser.Id
                    }
                }
            };

            var response = (await CreateQuizResponse(token2, quizResponse)).Content;
            Assert.Equal(quiz.Id, response.QuizId);
            Assert.Equal(quiz.Id, response.QuizResponseDetails.First().QuizId);
            Assert.Equal(question.Id, response.QuizResponseDetails.First().QuizQuestionId);
            Assert.Equal(choices.First().Id, response.QuizResponseDetails.First().QuizQuestionChoiceId);
            Assert.Equal(alternateUser.Id, response.QuizResponseDetails.First().OwnerId);

            //you can get responses for quizzes you made
            var getResponse = (await GetQuizResponse(token, quiz.Id, alternateUser.Id)).Content;
            Assert.Equal(quiz.Id, getResponse.QuizId);
            Assert.Equal(quiz.Id, getResponse.QuizResponseDetails.First().QuizId);
            Assert.Equal(question.Id, getResponse.QuizResponseDetails.First().QuizQuestionId);
            Assert.Equal(choices.First().Id, getResponse.QuizResponseDetails.First().QuizQuestionChoiceId);
            Assert.Equal(alternateUser.Id, getResponse.QuizResponseDetails.First().OwnerId);

            //cannot respond to your own quiz
            var selfQuizResponse = new QuizResponse()
            {
                QuizId = quiz.Id,
                QuizResponseDetails = new List<QuizResponseDetail>()
                {
                    new QuizResponseDetail()
                    {
                        QuizId = quiz.Id,
                        QuizQuestionId = question.Id,
                        QuizQuestionChoiceId = choices.First().Id,
                        OwnerId = user.Id
                    }
                }
            };
            var result = await CreateQuizResponse(token, selfQuizResponse);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

            //cant get responses for quizzes you didnt make
            result = await GetQuizResponse(token2, quiz.Id, alternateUser.Id);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

            //the owner of a quiz can delete the responses
            await DeleteQuizResponse(token, quiz.Id, alternateUser.Id);

            //cleanup test
            foreach (var c in choices)
            {
                await DeleteQuizQuestionChoice(token, c.Id);
            }
            await DeleteQuizQuestion(token, question.Id);
            await DeleteQuiz(token, quiz.Id);
            await UserClient.DeleteUser(token2, alternateUser.Id);
            await UserClient.DeleteUser(token, user.Id);
            
        }


        [Fact]
        public async Task UserCantSupplyMultipleAnswersToSingleSelect()
        {
            //create published quiz with user1
            var user = await CreateUser("testuser1@seasprig.ca", "password");
            var token = await Login(user, "password");
            var quiz = (await CreateQuiz(token)).Content;
            var question = (await CreateQuizQuestion(token, quiz.Id)).Content;
            Assert.False(question.IsMulti);
            var choices = new List<QuizQuestionChoice>();

            //only one correct answer
            choices.Add((await CreateQuizQuestionChoice(token, question.Id, new QuizQuestionChoice() 
            {
                Title = "option1",
                IsCorrect = true
            })).Content);
            choices.Add((await CreateQuizQuestionChoice(token, question.Id, new QuizQuestionChoice()
            {
                Title = "option2",
                IsCorrect = false
            })).Content);
            choices.Add((await CreateQuizQuestionChoice(token, question.Id, new QuizQuestionChoice()
            {
                Title = "option3",
                IsCorrect = false
            })).Content);

            var publishedQuiz = await PublishQuiz(token, quiz.Id);


            //response to quiz with more than one answer
            var alternateUser = await CreateUser("testuser2@seasprig.ca", "password");
            var token2 = await Login(alternateUser, "password");

            var quizResponse = new QuizResponse()
            {
                QuizId = quiz.Id,
                QuizResponseDetails = new List<QuizResponseDetail>()
                {
                    new QuizResponseDetail()
                    {
                        QuizId = quiz.Id,
                        QuizQuestionId = question.Id,
                        QuizQuestionChoiceId = choices.First().Id,
                        OwnerId = alternateUser.Id
                    },
                    new QuizResponseDetail()
                    {
                        QuizId = quiz.Id,
                        QuizQuestionId = question.Id,
                        QuizQuestionChoiceId = choices.Last().Id,
                        OwnerId = alternateUser.Id
                    },
                }
            };


            var updatedQuestion = (await GetQuizQuestion(token, question.Id)).Content;
            Assert.True(updatedQuestion.IsMulti);
            var result =await CreateQuizResponse(token2, quizResponse);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

            //cleanup test
            foreach (var c in choices)
            {
                await DeleteQuizQuestionChoice(token, c.Id);
            }
            await DeleteQuizQuestion(token, question.Id);
            await DeleteQuiz(token, quiz.Id);
            await UserClient.DeleteUser(token2, alternateUser.Id);
            await UserClient.DeleteUser(token, user.Id);

        }

        [Fact]
        public async Task UserCannotResponseToPublishedQuizTwice()
        {
            //create published quiz with user1
            var user = await CreateUser("testuser1@seasprig.ca", "password");
            var token = await Login(user, "password");
            var quiz = (await CreateQuiz(token)).Content;
            var question = (await CreateQuizQuestion(token, quiz.Id)).Content;
            var choices = new List<QuizQuestionChoice>();

            for (var i = 0; i < 2; i++)
            {
                choices.Add((await CreateQuizQuestionChoice(token, question.Id)).Content);
            }
            var publishedQuiz = await PublishQuiz(token, quiz.Id);


            //response to quiz with user2
            var alternateUser = await CreateUser("testuser2@seasprig.ca", "password");
            var token2 = await Login(alternateUser, "password");

            var quizResponse = new QuizResponse()
            {
                QuizId = quiz.Id,
                QuizResponseDetails = new List<QuizResponseDetail>()
                {
                    new QuizResponseDetail()
                    {
                        QuizId = quiz.Id,
                        QuizQuestionId = question.Id,
                        QuizQuestionChoiceId = choices.First().Id,
                        OwnerId = alternateUser.Id
                    }
                }
            };

            var response = await CreateQuizResponse(token2, quizResponse);
            var result = await CreateQuizResponse(token2, quizResponse);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

            //cleanup test
            await DeleteQuizResponse(token, quiz.Id, alternateUser.Id);
            foreach (var c in choices)
            {
                await DeleteQuizQuestionChoice(token, c.Id);
            }
            await DeleteQuizQuestion(token, question.Id);
            await DeleteQuiz(token, quiz.Id);
            await UserClient.DeleteUser(token2, alternateUser.Id);
            await UserClient.DeleteUser(token, user.Id);

        }

        [Fact]
        public async Task UserCannotEditAnotherUsersQuiz()
        {
            //create quiz with user1
            var user = await CreateUser("testuser1@seasprig.ca", "password");
            var token = await Login(user, "password");
            var quiz = (await CreateQuiz(token)).Content;
            var question = (await CreateQuizQuestion(token, quiz.Id)).Content;
            var choices = new List<QuizQuestionChoice>();

            for (var i = 0; i < 2; i++)
            {
                choices.Add((await CreateQuizQuestionChoice(token, question.Id)).Content);
            }

            //try to edit quiz with user2
            var alternateUser = await CreateUser("testuser2@seasprig.ca", "password");
            var token2 = await Login(alternateUser, "password");

            //this should fail
            var result = await CreateQuizQuestion(token2, quiz.Id);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            var result2 = await CreateQuizQuestionChoice(token2, question.Id);
            Assert.Equal(HttpStatusCode.BadRequest, result2.StatusCode);


            //cleanup test
            foreach (var c in choices)
            {
                await DeleteQuizQuestionChoice(token, c.Id);
            }
            await DeleteQuizQuestion(token, question.Id);
            await DeleteQuiz(token, quiz.Id);
            await UserClient.DeleteUser(token2, alternateUser.Id);
            await UserClient.DeleteUser(token, user.Id);

        }


        [Fact]
        public async Task UserCannotRespondToUnpublishedQuiz()
        {
            //create quiz with user1
            var user = await CreateUser("testuser1@seasprig.ca", "password");
            var token = await Login(user, "password");
            var quiz = (await CreateQuiz(token)).Content;
            var question = (await CreateQuizQuestion(token, quiz.Id)).Content;
            var choices = new List<QuizQuestionChoice>();

            for (var i = 0; i < 2; i++)
            {
                choices.Add((await CreateQuizQuestionChoice(token, question.Id)).Content);
            }

            //response to quiz with user2
            var alternateUser = await CreateUser("testuser2@seasprig.ca", "password");
            var token2 = await Login(alternateUser, "password");

            var quizResponse = new QuizResponse()
            {
                QuizId = quiz.Id,
                QuizResponseDetails = new List<QuizResponseDetail>()
                {
                    new QuizResponseDetail()
                    {
                        QuizId = quiz.Id,
                        QuizQuestionId = question.Id,
                        QuizQuestionChoiceId = choices.First().Id,
                        OwnerId = alternateUser.Id
                    }
                }
            };

            //this quiz is unpublished, so this should fail
            var result = await CreateQuizResponse(token2, quizResponse);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

            //cleanup test
            foreach (var c in choices)
            {
                await DeleteQuizQuestionChoice(token, c.Id);
            }
            await DeleteQuizQuestion(token, question.Id);
            await DeleteQuiz(token, quiz.Id);
            await UserClient.DeleteUser(token2, alternateUser.Id);
            await UserClient.DeleteUser(token, user.Id);

        }

        [Fact]
        public async Task UserCannotUpdateAnothersQuiz()
        {
            //create quiz with user1
            var user = await CreateUser("testuser1@seasprig.ca", "password");
            var token = await Login(user, "password");
            var quiz = (await CreateQuiz(token)).Content;
            var question = (await CreateQuizQuestion(token, quiz.Id)).Content;
            var choices = new List<QuizQuestionChoice>();

            for (var i = 0; i < 2; i++)
            {
                choices.Add((await CreateQuizQuestionChoice(token, question.Id)).Content);
            }

            //update quiz with user2
            var alternateUser = await CreateUser("testuser2@seasprig.ca", "password");
            var token2 = await Login(alternateUser, "password");

            //since this quiz belongs to user1, user2 should fail on related create and update calls
            var result = await UpdateQuiz(token2, quiz);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            var result2 = await UpdateQuizQuestion(token2, question);
            Assert.Equal(HttpStatusCode.BadRequest, result2.StatusCode);
            var result3 = await UpdateQuizQuestionChoice(token2, choices.First());
            Assert.Equal(HttpStatusCode.BadRequest, result3.StatusCode);
            //we also shouldnt be able to create or delete questions or choices on the quiz either
            result2 = await CreateQuizQuestion(token2, quiz.Id);
            Assert.Equal(HttpStatusCode.BadRequest, result2.StatusCode);
            result3 = await CreateQuizQuestionChoice(token2, question.Id);
            Assert.Equal(HttpStatusCode.BadRequest, result3.StatusCode);
            var result4 = await DeleteQuizQuestion(token2, question.Id);
            Assert.Equal(HttpStatusCode.BadRequest, result4.StatusCode);
            var result5 = await DeleteQuizQuestionChoice(token2, choices.First().Id);
            Assert.Equal(HttpStatusCode.BadRequest, result5.StatusCode);

            //cleanup test
            foreach (var c in choices)
            {
                await DeleteQuizQuestionChoice(token, c.Id);
            }
            await DeleteQuizQuestion(token, question.Id);
            await DeleteQuiz(token, quiz.Id);
            await UserClient.DeleteUser(token2, alternateUser.Id);
            await UserClient.DeleteUser(token, user.Id);

        }

        [Fact]
        public async Task GetQuizzesPagination()
        {
            //create twenty quizzes with two users
            var user = await CreateUser("testuser1@seasprig.ca", "password");
            var token = await Login(user, "password");
            
            var alternateUser = await CreateUser("testuser2@seasprig.ca", "password");
            var token2 = await Login(alternateUser, "password");

            var quizzes = new List<Quiz>();
            for (var i = 0; i < 20; i++) 
            {
                if (i % 2 == 0) quizzes.Add((await CreateQuiz(token)).Content);
                else quizzes.Add((await CreateQuiz(token2)).Content);
            }

            var pageOne = (await GetQuizzes(token, 10, 0)).Content;
            var pageTwo = (await GetQuizzes(token, 10, 10)).Content;
            var pageThree = (await GetQuizzes(token, 10, 20)).Content;

            Assert.Equal(10, pageOne.Count());
            Assert.Equal(10, pageTwo.Count());
            Assert.Empty(pageThree);
            Assert.Empty(pageOne.Select(p => p.Id).Intersect(pageTwo.Select(p => p.Id)));

            //cleanup test
            for (var i = 0; i < 20; i++)
            {
                if (i % 2 == 0) await DeleteQuiz(token, quizzes[i].Id);
                else await DeleteQuiz(token2, quizzes[i].Id);
            }
            await UserClient.DeleteUser(token2, alternateUser.Id);
            await UserClient.DeleteUser(token, user.Id);

        }

        [Fact]
        public async Task GetMyAnsweredQuizzesPagination()
        {
            //create twenty quizzes with two users
            var user = await CreateUser("testuser1@seasprig.ca", "password");
            var token = await Login(user, "password");

            var alternateUser = await CreateUser("testuser2@seasprig.ca", "password");
            var token2 = await Login(alternateUser, "password");


            //user1 creates 20 quizzes and user2 answers them all
            var quizzes = new List<Quiz>();
            var responses = new List<QuizResponse>();
            var questions = new List<QuizQuestion>();
            var questionChoices = new List<QuizQuestionChoice>();
            for (var i = 0; i < 20; i++)
            {
                var quiz = (await CreateQuiz(token)).Content;
                quizzes.Add(quiz);
                var question = (await CreateQuizQuestion(token, quiz.Id)).Content;
                questions.Add(question);
                var choices = new List<QuizQuestionChoice>();

                for (var j = 0; j < 2; j++)
                {
                    choices.Add((await CreateQuizQuestionChoice(token, question.Id)).Content);
                }
                questionChoices.AddRange(choices);

                var publishedQuiz = await PublishQuiz(token, quiz.Id);

                var quizResponse = new QuizResponse()
                {
                    QuizId = quiz.Id,
                    QuizResponseDetails = new List<QuizResponseDetail>()
                {
                    new QuizResponseDetail()
                    {
                        QuizId = quiz.Id,
                        QuizQuestionId = question.Id,
                        QuizQuestionChoiceId = choices.First().Id,
                        OwnerId = alternateUser.Id
                    }
                }
                };

                var response = (await CreateQuizResponse(token2, quizResponse)).Content;
                responses.Add(response);
            }

            //the user that answered the quizzes gets results,
            //but the user that created them get none
            var pageOne = (await GetMyAnsweredQuizzes(token2, 10, 0)).Content;
            var pageTwo = (await GetMyAnsweredQuizzes(token2, 10, 10)).Content;
            var pageThree = (await GetMyAnsweredQuizzes(token2, 10, 20)).Content;
            Assert.Equal(10, pageOne.Count());
            Assert.Equal(10, pageTwo.Count());
            Assert.Empty(pageThree);
            Assert.Empty(pageOne.Select(p => p.Id).Intersect(pageTwo.Select(p => p.Id)));

            var emptyPage = (await GetMyAnsweredQuizzes(token, 10, 0)).Content;
            Assert.Empty(emptyPage);

            //cleanup test
            foreach (var r in responses)
            {
                await DeleteQuizResponse(token, r.QuizId, alternateUser.Id);
            }
            foreach (var c in questionChoices)
            {
                await DeleteQuizQuestionChoice(token, c.Id);
            }
            foreach (var q in questions)
            {
                await DeleteQuizQuestion(token, q.Id);
            }
            foreach (var q in quizzes)
            {
                await DeleteQuiz(token, q.Id);
            }
            await UserClient.DeleteUser(token2, alternateUser.Id);
            await UserClient.DeleteUser(token, user.Id);

        }

        [Fact]
        public async Task QuestionScoreCalculator()
        {
            var allChoices = new List<QuizQuestionChoice>()
            {
                new QuizQuestionChoice() 
                {
                    Id = "1",
                    Title = "Moon is a star",
                    IsCorrect = false
                },
                new QuizQuestionChoice()
                {
                    Id = "2",
                    Title = "Moon is not a star",
                    IsCorrect = true
                },
            };
            Assert.Equal(0, QuizResponseService.GetQuestionScore(allChoices, new List<QuizResponseDetail>()));
            Assert.Equal(1, QuizResponseService.GetQuestionScore(allChoices, new List<QuizResponseDetail>() 
            { 
                new QuizResponseDetail() { QuizQuestionChoiceId = "2" }
            }));
            Assert.Equal(-1, QuizResponseService.GetQuestionScore(allChoices, new List<QuizResponseDetail>()
            {
                new QuizResponseDetail() { QuizQuestionChoiceId = "1" }
            }));

            allChoices = new List<QuizQuestionChoice>()
            {
                new QuizQuestionChoice()
                {
                    Id = "1",
                    Title = "Kelvin",
                    IsCorrect = true
                },
                new QuizQuestionChoice()
                {
                    Id = "2",
                    Title = "Fahrenheit",
                    IsCorrect = true
                },
                new QuizQuestionChoice()
                {
                    Id = "3",
                    Title = "Gram",
                    IsCorrect = false
                },
                new QuizQuestionChoice()
                {
                    Id = "4",
                    Title = "Celsius",
                    IsCorrect = true
                },
                new QuizQuestionChoice()
                {
                    Id = "5",
                    Title = "Liters",
                    IsCorrect = false
                },
            };

            var answer = new List<QuizResponseDetail>()
            {
                new QuizResponseDetail() { QuizQuestionChoiceId = "1" },//kelvin
                new QuizResponseDetail() { QuizQuestionChoiceId = "2" }//Fahrenheit
            };
            Assert.Equal(Math.Round(0.666, 2), Math.Round(QuizResponseService.GetQuestionScore(allChoices, answer), 2));

            answer = new List<QuizResponseDetail>()
            {
                new QuizResponseDetail() { QuizQuestionChoiceId = "1" },//kelvin
                new QuizResponseDetail() { QuizQuestionChoiceId = "2" },//Fahrenheit
                new QuizResponseDetail() { QuizQuestionChoiceId = "4" }//Celsius
            };
            Assert.Equal(1, QuizResponseService.GetQuestionScore(allChoices, answer));


            answer = new List<QuizResponseDetail>()
            {
                new QuizResponseDetail() { QuizQuestionChoiceId = "1" },//kelvin
                new QuizResponseDetail() { QuizQuestionChoiceId = "2" },//Fahrenheit
                new QuizResponseDetail() { QuizQuestionChoiceId = "3" }//Gram
            };
            Assert.Equal(Math.Round(0.166, 2), Math.Round(QuizResponseService.GetQuestionScore(allChoices, answer), 2));

            answer = new List<QuizResponseDetail>()
            {
                new QuizResponseDetail() { QuizQuestionChoiceId = "1" },//kelvin
                new QuizResponseDetail() { QuizQuestionChoiceId = "2" },//Fahrenheit
                new QuizResponseDetail() { QuizQuestionChoiceId = "3" },//Gram
                new QuizResponseDetail() { QuizQuestionChoiceId = "4" },//Celsius
                new QuizResponseDetail() { QuizQuestionChoiceId = "5" }//Liters
            };
            Assert.Equal(0, Math.Round(QuizResponseService.GetQuestionScore(allChoices, answer), 2));
        }

        [Fact]
        public async Task GetQuizScoredResponse()
        {
            //create twenty quizzes with two users
            var user = await CreateUser("testuser1@seasprig.ca", "password");
            var token = await Login(user, "password");

            var alternateUser = await CreateUser("testuser2@seasprig.ca", "password");
            var token2 = await Login(alternateUser, "password");


            //user1 creates a quiz and user2 takes it

            var quiz = (await CreateQuiz(token)).Content;
            var question = (await CreateQuizQuestion(token, quiz.Id)).Content;
            var choices = new List<QuizQuestionChoice>();


            choices.Add((await CreateQuizQuestionChoice(token, question.Id, new QuizQuestionChoice() 
            { 
                Title = "this is correct",
                IsCorrect = true
            })).Content);
            choices.Add((await CreateQuizQuestionChoice(token, question.Id, new QuizQuestionChoice()
            {
                Title = "this is also correct",
                IsCorrect = true
            })).Content);
            choices.Add((await CreateQuizQuestionChoice(token, question.Id, new QuizQuestionChoice()
            {
                Title = "this is not correct",
                IsCorrect = false
            })).Content);
            choices.Add((await CreateQuizQuestionChoice(token, question.Id, new QuizQuestionChoice()
            {
                Title = "this is also not correct",
                IsCorrect = false
            })).Content);



            var publishedQuiz = (await PublishQuiz(token, quiz.Id)).Content;

            var quizResponse = new QuizResponse()
            {
                QuizId = quiz.Id,
                QuizResponseDetails = new List<QuizResponseDetail>()
            {
                new QuizResponseDetail()
                {
                    QuizId = quiz.Id,
                    QuizQuestionId = question.Id,
                    QuizQuestionChoiceId = choices.First().Id,
                    OwnerId = alternateUser.Id
                }
            }
            };

            var response = (await CreateQuizResponse(token2, quizResponse)).Content;

            var userSelfScore = (await GetQuizResponseScore(token2, quiz.Id, alternateUser.Id)).Content;
            Assert.Empty(userSelfScore.QuizResponseDetails);
            var userScore = (await GetQuizResponseScore(token, quiz.Id, alternateUser.Id)).Content;
            Assert.NotEmpty(userScore.QuizResponseDetails);

 
            //cleanup
            await DeleteQuizResponse(token, response.QuizId, alternateUser.Id);
            foreach (var c in choices)
            {
                await DeleteQuizQuestionChoice(token, c.Id);
            }
            await DeleteQuizQuestion(token, question.Id);
            await DeleteQuiz(token, quiz.Id);
            await UserClient.DeleteUser(token2, alternateUser.Id);
            await UserClient.DeleteUser(token, user.Id);
        }


        public void Dispose() 
        {

        }
    }
}