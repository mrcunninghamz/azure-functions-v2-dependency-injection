using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AzureFunctionsV2.DependencyInjection;
using AzureFunctionsV2DI;
using AzureFunctionsV2DI.Service;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: WebJobsStartup(typeof(WebJobsExtensionStartup), "A Web Jobs Extension Sample")]
namespace AzureFunctionsV2DI
{
    public class WebJobsExtensionStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            var serviceProvider = ConfigureServices(builder.Services);
            builder.Services.AddSingleton(new InjectBindingProvider(serviceProvider));
            builder.AddExtension<InjectConfiguration>();
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            
            var containterBuilder = new ContainerBuilder();
            containterBuilder.Populate(services);
            
            //Register services and modules
            //containterBuilder.RegisterModule();
            var testValue = config.GetValue<string>("Test");
            containterBuilder.Register(_ => new DemoService(testValue)).As<IDemoService>().InstancePerLifetimeScope();

            var applicationContainer = containterBuilder.Build();
            return new AutofacServiceProvider(applicationContainer);
        }
    }
}
