using Emulator;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AdvancedCLI
{
    public class EmulateCommand : Command
    {
        public EmulateCommand() : base("emulate", "Simulates Atari Lynx Emulator")
        {
            // Arguments
            this.AddArgument(
                new Argument<FileInfo>("gamerom", "Game ROM file").ExistingOnly());

            // Options
            this.AddOption(new Option<bool>(new[] { "--fullscreen", "-f" }, () => false, "Run full screen"));
            this.AddOption(
                new Option<ControllerType>(
                    new string[] { "--controller", "-c" },
                    () => ControllerType.Keyboard,
                    "Type of controller to use"
                )
            );

            this.AddOption(new MagnificationOption());

            this.Handler = CommandHandler.Create<EmulatorClientOptions, bool, BindingContext>(Emulate);
        }

        internal void Emulate(EmulatorClientOptions options, bool verbose, BindingContext context)
        {
            IAnsiConsole console = (IAnsiConsole)context.GetService(typeof(IAnsiConsole));
            if (!console.Confirm("Ready to start?"))
            {
                console.MarkupLine("[red]Oops[/] [blue]:([/]");
                return;
            }

            // To demonstrate exception handling
            if (options.Magnification == 3) throw new CommandException("Intentional expection");

            new EmulatorClient(options).Run();
        }
    }

    public class MagnificationOption: Option<int>
    {
        public MagnificationOption() : base(aliases: ["--magnification", "-m"])
        {
            this.Description = "Magnification of screen";
            this.SetDefaultValue(4);
            this.IsRequired = false;
            this.AddValidator(option =>
            {
                // Dangerous to read value here, as it might not be a parseable value
                //int value = option.GetValueOrDefault<int>();

                if (option.Token is null) return;
                if (!Int32.TryParse(option.Tokens[0].Value, out int value) || value <= 0 || value > 20)
                    option.ErrorMessage = "Magnification must be an integer value between 1 and 20";
            });
        }
    }
}
