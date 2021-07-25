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
                    // Execute middleware before running host
                    return next(context);
                })
                .UseHost(
                    args => CreateHostBuilder(args),
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
                            // Bind to options from command line
                            services.AddOptions<FooOptions>().BindCommandLine();

                            // Use invocation context for smart things
                            context = builder.GetInvocationContext();
                        });
                    })
                .UseMiddleware((context, next) =>
                {
                    // Host is available in middleware now
                    IHost host = context.GetHost();
                    return next(context);
                })
                .Build();

            return parser.InvokeAsync(args);
        }

        private static CommandLineBuilder BuildCommandLine()
        {
            var root = new RootCommand();
            root.Handler = CommandHandler.Create<IHost>(async (host) => await host.WaitForShutdownAsync());
            //root.TreatUnmatchedTokensAsErrors = false;

            Command simulate = new Command("simulate")
            {
                new Option<string>("--bar"),
                new Option<string>("--baz")
            };
            
            Option<int> level = new Option<int>(new[] { "--level", "-l" }, "Simulation level");
            level.IsRequired = true;
            simulate.AddOption(level);
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

            logger.LogInformation("Terminating simulation");
        }

        private static async Task Run(IHost host)
        {
            await host.WaitForShutdownAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
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
