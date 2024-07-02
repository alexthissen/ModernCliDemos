using Emulator;
using Spectre.Console;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AdvancedCLI
{
    [ExampleUsage("convert zarlor.bin", "Convert 'zarlor.bin' image file to 'zarlor.lnx' ROM file")]
    [ExampleUsage("convert zarlor.bin --output game.lnx", "Convert 'zarlor.bin' to 'game.lnx'")]
    public class ConvertCommand : Command
    {
        public ConvertCommand() : base("convert", "Converts binary ROM to Handy ROM file")
        {
            // Arguments
            Argument<FileInfo> inputArgument = 
                new Argument<FileInfo>("input", "Binary ROM file (*.bin)")
                .ExistingOnly();
            this.AddArgument(inputArgument);

            // Options
            Option<FileInfo> outputOption = new Option<FileInfo>(new[] { "--output", "-o" },
                isDefault: true,
                parseArgument: arg =>
                {
                    if (arg.Tokens.Any()) return new FileInfo(arg.Tokens.Single().Value);
                    FileInfo input = arg.FindResultFor(inputArgument)?.GetValueOrDefault<FileInfo>();
                    if (input == null) return null;
                    return new FileInfo(Path.ChangeExtension(input.Name, ".lnx"));
                },
                description: "Output Handy ROM file")
                .LegalFileNamesOnly();
            this.AddOption(outputOption);

            // Naming convention binding
            this.Handler = CommandHandler.Create<FileInfo, FileInfo, IConsole, CancellationToken>(Convert);

            // Alternative with lambda expression
            //this.SetHandler(async (context) =>
            //{
            //    FileInfo input = context.ParseResult.GetValueForArgument(inputArgument);
            //    FileInfo output = context.ParseResult.GetValueForOption<FileInfo>(outputOption);

            //    int returnCode = await Convert(
            //        input, output, context.Console, context.GetCancellationToken());
            //});
        }

        private Task<int> Convert(FileInfo input, FileInfo output, IConsole console, CancellationToken token)
        {
            AnsiConsole.Progress()
                .AutoRefresh(false)
                .AutoClear(false)
                .HideCompleted(false)
                .Columns(new ProgressColumn[]
                {
                    new TaskDescriptionColumn(),    // Task description
                    new ProgressBarColumn(),        // Progress bar
                    new PercentageColumn(),         // Percentage
                    new RemainingTimeColumn(),      // Remaining time
                    new SpinnerColumn() { Spinner = Spinner.Known.Default } 
                })
                .Start(progress =>
                {
                    // Define tasks
                    var task1 = progress.AddTask("[green]Converting[/]");
                    var task2 = progress.AddTask("[green]Writing[/]");

                    // Use cancellation token or pass down to async method supporting it
                    while (!progress.IsFinished && !token.IsCancellationRequested)
                    {
                        task1.Increment(1.5);
                        task2.Increment(0.5);
                        Thread.Sleep(20);

                        // Check whether output is piped 
                        if (!console.IsOutputRedirected) 
                            progress.Refresh();
                    }
                });

            console.Out.Write("Finished converting");
            return Task.FromResult(0); // Non-zero exit code indicates failure
        }
    }
}