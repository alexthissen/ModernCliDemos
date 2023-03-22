using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNETCoreWithCLI
{
    public class Program
    {
        public static Task<int> Main(string[] args)
        {
            Parser parser = BuildCommandLine()
                .UseDefaults()
                .UseMiddleware(async (context, next) =>
                {
                    // Execute middleware before running host
                    // Could decide to not propagate and short circuit calling
                    //if (context.ParseResult.FindResultFor(versionOption) is { } result)
                    //{ }
                    await next(context);
                }, MiddlewareOrder.ErrorReporting)
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
                            services.AddOptions<MigrationOptions>().BindCommandLine();

                            // Use invocation context for smart things
                            context = builder.GetInvocationContext();
                        });

                        // Register command handlers to use dependency injection when constructed
                        builder.UseCommandHandler<MigrationCommand, MigrationCommandHandler>();
                    })
                .UseMiddleware((context, next) =>
                {
                    // Host is available in middleware after UseHost call
                    IHost host = context.GetHost();

                    // Use host.Services.GetService and other methods on IServiceProvider
                    return next(context);
                })
                .Build();

            return parser.InvokeAsync(args);
        }

        private static CommandLineBuilder BuildCommandLine()
        {
            var root = new RootCommand()
            {
                new Option<string>("--message"),
                new Option<string>("--version")
            };
            root.Handler = CommandHandler.Create<int, ParseResult, IConsole, IHost>(RunHost);
            root.TreatUnmatchedTokensAsErrors = false;
            Option<int> level = new Option<int>(new[] { "--level", "-l" }, "Simulation level");
            level.IsRequired = true;
            root.AddOption(level);
            root.AddCommand(new MigrationCommand());

            return new CommandLineBuilder(root);
        }

        private static async Task RunHost(int level, ParseResult result, IConsole console, IHost host)
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

    public class MigrationCommand : Command
    {
        public MigrationCommand() : base("migrate")
        {
            AddOption(new Option<string>("--message"));
            AddOption(new Option<int>("--version"));
            Option<int> level = new Option<int>(new[] { "--level", "-l" }, "Simulation level");
            level.IsRequired = true;
            AddOption(level);
        }
    }

    public class MigrationCommandHandler : ICommandHandler
    {
        private readonly IMigrator migrator;

        public MigrationCommandHandler(IMigrator migrator, ParseResult result, IConsole console, IHost host)
        {
            this.migrator = migrator;
        }

        // Properties matched from binding context
        public string Message { get; set; }
        public int Version { get; set; }
        public int Level { get; set; }

        // Properties set from dependency injection
        public IConsole Console { get; set; }
        public IHost Host { get; set; }
        public BindingContext BindingContext { get; set; }

        public Task<int> InvokeAsync(InvocationContext context)
        {
            migrator.Migrate(new MigrationOptions () {  
                Message = this.Message,
                Version = this.Version
            });
            return Task.FromResult(1);
        }
    }

    public class MigrationOptions
    {
        public string Message { get; set; }
        public int Version { get; set; }
    }

    public interface IMigrator {
        int Migrate(MigrationOptions options) => 100;
    }

    public class RealMigrator : IMigrator { };

    public class SimulationMigrator : IMigrator {
        public int Migrate() { return 1; }
    }
}
