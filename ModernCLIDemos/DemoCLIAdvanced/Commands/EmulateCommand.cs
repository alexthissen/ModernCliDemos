using Emulator;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using System.IO;
using System.Linq;
using System.Text;
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

            Option<int> magnificationOption = new Option<int>("--magnification", "Magnification of screen");
            magnificationOption.AddAlias("-m");
            magnificationOption.SetDefaultValue(4);
            magnificationOption.IsRequired = false;
            magnificationOption.AddValidator(option =>
            {
                // Dangerous to read value here, as it might not be a parseable value
                //int value = option.GetValueOrDefault<int>();

                if (option.Token == null) return;
                if (!Int32.TryParse(option.Tokens[0].Value, out int value) || value <= 0 || value > 20)
                    option.ErrorMessage = "Magnification must be an integer value between 1 and 20";
            });
            this.AddOption(magnificationOption);

            this.Handler = CommandHandler.Create<EmulatorClientOptions, bool>(Emulate);
        }

        private void Emulate(EmulatorClientOptions options, bool verbose)
        {
            // To demonstrate exception handling
            if (options.Magnification == 3) throw new CommandException("Intentional expection");

            new EmulatorClient(options).Run();
        }
    }
}
