using Emulator;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AdvancedCLI
{
    public class ConvertCommand : Command
    {
        public ConvertCommand() : base("convert", "Converts binary ROM to Handy ROM file")
        {
            // Arguments
            Argument inputArgument = new Argument<FileInfo>("input", "Binary ROM file (*.bin)").ExistingOnly();
            this.AddArgument(inputArgument);

            // Options
            this.AddOption(new Option<FileInfo>(new[] { "--output", "-o" },
                isDefault: true,
                parseArgument: arg =>
                {
                    if (arg.Tokens.Any()) return new FileInfo(arg.Tokens.Single().Value);
                    FileInfo input = arg.FindResultFor(inputArgument)?.GetValueOrDefault<FileInfo>();
                    if (input == null) return null;
                    return new FileInfo(Path.ChangeExtension(input.Name, ".lnx"));
                },
                description: "Output Handy ROM file")
                .LegalFileNamesOnly()); 

            this.Handler = CommandHandler.Create<FileInfo, FileInfo, InvocationContext>(Convert);
        }

        private void Convert(FileInfo input, FileInfo output, InvocationContext context)
        {
            var token = context.GetCancellationToken();

            while (!token.IsCancellationRequested)
            {
                context.Console.Out.Write($"Converting from {input.Name} to {output.Name}" + Environment.NewLine);
                Thread.Sleep(2000);
            }
        }
    }
}
