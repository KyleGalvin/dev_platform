using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuizBuilder;
using QuizBuilder.Util;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XUnitIntegrationTests.Clients;

namespace XUnitIntegrationTests
{
    public class QuizBuilderFixture: IAsyncDisposable
    {
        public IServiceProvider ServiceProvider;
        private IHost Server;
        public QuizBuilderFixture()
        {
            ServiceProvider = AddServices(new ServiceCollection());
            Server = TestApplicationStartup.CreateWebHostBuilder([]).Build();
            Task.Run(() => Server.Run());


        }

        public IServiceProvider AddServices(IServiceCollection services) 
        {
            services.ConfigureDependencies();
            var baseAddress = $"http://{EnvironmentVars.GetQuizbuilderHostname() ?? ""}";
            services.AddRefitClient<IUserClient>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseAddress));
            services.AddRefitClient<IQuizClient>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseAddress));
            services.AddRefitClient<IQuizQuestionClient>()
               .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseAddress));
            services.AddRefitClient<IQuizQuestionChoiceClient>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseAddress));
            services.AddRefitClient<IQuizResponseClient>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseAddress));
            var provider = services.BuildServiceProvider();
            return provider;

        }

        public async ValueTask DisposeAsync()
        {
            await Server.StopAsync();
        }
    }
}
