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
using System.CommandLine.Binding;

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
                    // Could decide to not propagate and short circuit calling
                    return next(context);
                })
                .UseHost(
                    args => CreateHostBuilder(args),
                    builder =>
                    {
                        InvocationContext context = builder.GetInvocationContext();
                        var args = context.ParseResult.UnparsedTokens.ToArray();
                        builder.ConfigureHostConfiguration(config =>
                        {
                            // Leverages "normal" command-line arguments
                            // from Microsoft.Extensions.Configuration.CommandLine
                            // for unparsed tokens after --
                            config.AddCommandLine(args);
                        });

                        builder.ConfigureServices(services =>
                        {
                            // Bind to options from command line
                            services.AddOptions<FooOptions>().BindCommandLine();
                            // Use invocation context for smart things
                            context = builder.GetInvocationContext();
                        });

                        // Register command handlers to use dependency injection when constructed
                        builder.UseCommandHandler<SimulationCommand, SimulationCommandHandler>();
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
            //root.AddCommand(simulate);

            root.AddCommand(new SimulationCommand());

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

    public class SimulationCommand : Command
    {
        public SimulationCommand() : base("simulate")
        {
            AddOption(new Option<string>("--bar"));
            AddOption(new Option<string>("--baz"));
            Option<int> level = new Option<int>(new[] { "--level", "-l" }, "Simulation level");
            level.IsRequired = true;
            AddOption(level);
        }
    }

    public class SimulationCommandHandler : ICommandHandler
    {
        private readonly IFoo foo;

        public SimulationCommandHandler(IFoo foo, ParseResult result, IConsole console, IHost host)
        {
            this.foo = foo;
        }

        // Properties matched from binding context
        public string Bar { get; set; }
        public string Baz { get; set; }
        public int Level { get; set; }

        // Poroperties set from dependency injection
        public IConsole Console { get; set; }
        public IHost Host { get; set; }
        public BindingContext BindingContext { get; set; }

        public Task<int> InvokeAsync(InvocationContext context)
        {
            foo.DoIt();
            return Task.FromResult(1);
        }
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
