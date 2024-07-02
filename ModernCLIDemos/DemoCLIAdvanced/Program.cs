using Emulator;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Builder;
using System.CommandLine.Help;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("DemoCLIAdvanced.Tests")]

namespace AdvancedCLI
{
    class Program
    {
        public static async Task<int> Main(string[] args)
        {
            Console.InputEncoding = System.Text.Encoding.UTF8;
            Console.OutputEncoding = System.Text.Encoding.UTF8;

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
                .AddMiddleware(async (context, next) => {
                    context.BindingContext.AddService<IAnsiConsole>(provider => AnsiConsole.Console);
                    await next(context);
                })
                .UseHelp(context =>
                {
                    context.HelpBuilder.CustomizeLayout(_ =>
                        HelpBuilder.Default.GetLayout()
                            .Skip(1)
                            .Prepend(_ => _.Output.Write("Advanced Atari Lynx Emulator CLI"))
                            .Append(ExamplesSection()));
                })
                .UseExceptionHandler(HandleException)
                .Build();
            
            return await parser.InvokeAsync(args);
        }

        private static HelpSectionDelegate ExamplesSection()
        {
            return (context) =>
            {
                var command = context.Command;
                if (command == null) return;

                var examples = command.GetType().GetCustomAttributes<ExampleUsageAttribute>();
                if (examples != null) 
                {
                    context.Output.WriteLine("Examples:");
                    foreach (var example in examples)
                    {
                        context.Output.WriteLine($"  # {example.Explanation}");
                        context.Output.WriteLine("  " + example.Example);
                    }
                }
            };
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
                // Alternative is to use middleware
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

    public class ExampleHelpBuilder : HelpBuilder
    {
        public ExampleHelpBuilder(LocalizationResources localizationResources, int maxWidth = int.MaxValue) : base(localizationResources, maxWidth)
        {
        }

        public override void Write(HelpContext context)
        {
            base.Write(context);
        }
    }
}
