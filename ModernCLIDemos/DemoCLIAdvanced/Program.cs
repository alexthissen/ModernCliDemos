using Emulator;
using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Help;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AdvancedCLI
{
    class Program
    {
        public static async Task<int> Main(string[] args)
        {
            RootCommand rootCommand = new RootCommand("Atari Lynx Tool");
            rootCommand.TreatUnmatchedTokensAsErrors = true;
            rootCommand.AddCommand(new EmulateCommand());
            rootCommand.AddCommand(new ConvertCommand());

            // Show command-line help unless a subcommand was used.
            rootCommand.Handler = CommandHandler.Create(() => rootCommand.Invoke("-h"));

            rootCommand.AddGlobalOption(new Option<bool>(new[] { "--verbose", "-v" }, "Show verbose output"));

            // Parse command-line
            Parser parser = new CommandLineBuilder(rootCommand)
                .UseDefaults()
                .UseExceptionHandler(HandleException)
                .Build();
            return await parser.InvokeAsync(args);
        }

        private static void HandleException(Exception exception, InvocationContext context)
        {
            context.Console.ResetTerminalForegroundColor();
            context.Console.SetTerminalForegroundColor(ConsoleColor.Red);

            if (exception is TargetInvocationException tie && tie.InnerException is object)
            {
                exception = tie.InnerException;
            }

            if (exception is OperationCanceledException)
            {
                context.Console.Error.WriteLine("Operation has been canceled.");
            }
            else if (exception is CommandException command)
            {
                context.Console.Error.WriteLine($"Command '{context.ParseResult.CommandResult.Command.Name}' failed:");
                context.Console.Error.WriteLine($"\t{command.Message}");

                if (command.InnerException != null)
                {
                    context.Console.Error.WriteLine();
                    context.Console.Error.WriteLine(command.InnerException.ToString());
                }
            }

            context.Console.ResetTerminalForegroundColor();
            context.ExitCode = 1;
        }
    }
}
