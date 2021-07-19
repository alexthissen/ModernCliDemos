using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Help;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Threading.Tasks;

namespace DemoCLI
{
    class Program
    {
        public static async Task<int> Main(string[] args)
        {
            RootCommand rootCommand = new RootCommand("Steam Command-line Interface");

            // Show commandline help unless a subcommand was used.
            rootCommand.Handler = CommandHandler.Create<IHelpBuilder>(help =>
                {
                    help.Write(rootCommand);
                    return 1;
                });
            var builder = new CommandLineBuilder(rootCommand).UseDefaults();

            var parser = builder.Build();
            return await parser.InvokeAsync(args);
        }
    }
}
