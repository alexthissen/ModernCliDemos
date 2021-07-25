using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Invocation;
using System.CommandLine;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.CommandLine.Parsing;

namespace ASPNETCoreWithCLI
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Parser parser = BuildCommandLine()
                .UseDefaults()
                .UseHost(_ => CreateHostBuilder(_),
                    host =>
                    {
                        host.ConfigureServices(services =>
                        {
                            services.AddSingleton<IFoo, Foo>();
                        });
                        //.UseCommandHandler<;
                        //.ConfigureHostConfiguration(config => { config.AddCommandLine(host.GetInvocationContext().BindingContext.ParseResult.UnparsedTokens.ToArray()); });
                    })
                .Build();

                //return parser.InvokeAsync(args); 
                return parser.Invoke(args); 
        }

    private static CommandLineBuilder BuildCommandLine()
    {
        var root = new RootCommand(){
                new Option<string>("--name"){
                    IsRequired = true
                }
            };
        root.Handler = CommandHandler.Create<string, IFoo, ParseResult, IConsole, IHost>(Run);
        return new CommandLineBuilder(root);
    }

    private static void Run(string name, IFoo foo, ParseResult result, IConsole console, IHost host)
    {
        var serviceProvider = host.Services;
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger(typeof(Program));

        logger.LogInformation(name);
        host.WaitForShutdown();
        //host.Run();

        logger.LogInformation("Ready Player One");
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });

    }

    public interface IFoo { }
    public class Foo : IFoo { }
}
