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
using System.Xml.Linq;

namespace ASPNETCoreWithCLI
{
    public class Program
    {
        public static Task<int> Main(string[] args)
        {
            Parser parser = BuildCommandLine()
                .UseDefaults()
                .UseMiddleware((context, next) =>
                {
                    //ctxCustom = context;
                    return next(context);
                })
                .UseHost(
                    _ => CreateHostBuilder(_),
                    builder =>
                    {
                        //builder.Properties.TryGetValue(typeof(InvocationContext), out var context);
                        InvocationContext context = builder.GetInvocationContext();
                        var args = context.ParseResult.UnparsedTokens.ToArray();
                        builder.ConfigureHostConfiguration(config =>
                        {
                            config.AddCommandLine(args);
                        });

                        builder.ConfigureServices(services =>
                        {
                            services.AddSingleton<IFoo, FooSimulator>();
                            // Bind to options
                            services.AddOptions<FooOptions>().BindCommandLine();
                            context = builder.GetInvocationContext();
                        });
                        
                        //var args = context.ParseResult.UnparsedTokens.ToArray();
                        //builder.ConfigureHostConfiguration(config =>
                        //{
                        //    config.AddCommandLine(args);
                        //});
                        //.ConfigureHostConfiguration(config => { config.AddCommandLine(host.GetInvocationContext().BindingContext.ParseResult.UnparsedTokens.ToArray()); });
                    })
                .UseMiddleware((context, next) =>
                {
                    IHost host = context.GetHost();
                    return next(context);
                })
                .Build();

            return parser.InvokeAsync(args);
            //return parser.Invoke(args); 
        }

        private static CommandLineBuilder BuildCommandLine()
        {
            var root = new RootCommand() {
                new Argument("name"),
                new Option<string>("--bar"),
                new Option<string>("--baz")
            };
            root.Handler = CommandHandler.Create<string, ParseResult, IConsole, IHost>(Run);
            //root.TreatUnmatchedTokensAsErrors = false;

            Command simulate = new Command("simulate");
            simulate.AddOption(new Option<int>("--level", "-l"));
            simulate.Handler = CommandHandler.Create<int, ParseResult, IConsole, IHost>(Simulate);
            root.AddCommand(simulate);

            return new CommandLineBuilder(root);
        }

        private static async Task Simulate(int level, ParseResult result, IConsole console, IHost host)
        {
            var serviceProvider = host.Services;
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(typeof(Program));

            logger.LogInformation($"Simulating level {level}");
            await host.WaitForShutdownAsync();

            logger.LogInformation("Ready Player One");
        }

        private static async Task Run(string name, ParseResult result, IConsole console, IHost host)
        {
            var serviceProvider = host.Services;
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(typeof(Program));

            logger.LogInformation(name);
            await host.WaitForShutdownAsync();

            logger.LogInformation("Ready Player One");
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(config =>
                {
                    config.AddCommandLine(args);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

    }

    public class FooOptions
    {
        public string Bar { get; set; }
        public int Baz { get; set; }
    }

    public interface IFoo {
        int DoIt() => 100;
    }

    public class RealFoo : IFoo { };

    public class FooSimulator : IFoo {
        public int DoIt() { return 1; }
    }
}
