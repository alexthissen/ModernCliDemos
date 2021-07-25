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
        public static async Task Main(string[] args) => await BuildCommandLine()
            .UseHost(_ => Host.CreateDefaultBuilder(),
                host =>
                {
                    host.ConfigureServices(services =>
                    {
                        //services.AddSingleton<IConsole, Console>();
                    });
                })
            .UseDefaults()
            .Build()
            .InvokeAsync(args);

    private static CommandLineBuilder BuildCommandLine()
    {
        var root = new RootCommand(){
                new Option<string>("--name"){
                    IsRequired = true
                }
            };
        root.Handler = CommandHandler.Create<string, ParseResult, IConsole, IHost>(Run);
        return new CommandLineBuilder(root);
    }

    private static void Run(string name, ParseResult result, IConsole console, IHost host)
    {
        var serviceProvider = host.Services;
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger(typeof(Program));

        logger.LogInformation(name);
        CreateHostBuilder(result.UnmatchedTokens.ToArray()).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}
