using Emulator;
using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Help;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace StandardCLI
{
    class Program
    {
        static void NormalMain()
        {
            new EmulatorClient().Run();
        }

        public static int Main(string[] args)
        {
            RootCommand rootCommand = new RootCommand("Atari Lynx Emulator Simulator");
            rootCommand.TreatUnmatchedTokensAsErrors = true;

            // Arguments
            rootCommand.AddArgument(new Argument<FileInfo>("gamerom", "Game ROM file"));

            // Options
            rootCommand.AddOption(new Option<bool>(new[] { "--fullscreen", "-f" }, () => false, "Run full screen"));
            rootCommand.AddOption(new Option<ControllerType>(
                new string[] { "--controller", "-c" },
                () => ControllerType.Keyboard,
                "Type of controller to use")
            );

            Option<int> magnificationOption = new Option<int>("--magnification", "Magnification of screen");
            magnificationOption.AddAlias("-m");
            magnificationOption.SetDefaultValue(4);
            magnificationOption.IsRequired = false;
            magnificationOption.AddValidator(option =>
            {
                // Dangerous to read value here, as it might not be a parseable value
                //int value = option.GetValueOrDefault<int>();

                if (option.Token == null) return null;
                if (!Int32.TryParse(option.Tokens[0].Value, out int value) || value <= 0 || value > 20)
                    return "Magnification must be an integer value between 1 and 20";
                return null;
            });
            rootCommand.AddOption(magnificationOption);

            rootCommand.Handler = CommandHandler.Create<int, bool, ControllerType, FileInfo>(
                (magnification, fullScreen, controller, gameRom) =>
                {
                    EmulatorClientOptions options = new EmulatorClientOptions(gameRom)
                    {
                        FullScreen = fullScreen,
                        Magnification = magnification,
                        Controller = controller
                    };
                    Emulate(options);
                });

            // Alternatively, provide separate function instead of lambda
            //rootCommand.Handler = CommandHandler.Create<int, bool, ControllerType, FileInfo, FileInfo>(Emulate);

            // Parse command-line
            Parser parser = new CommandLineBuilder(rootCommand).UseDefaults().Build();
            return parser.Invoke(args);
        }

        private static void Emulate(int magnification, bool fullScreen, ControllerType controller, FileInfo gameRom)
        {
            EmulatorClientOptions options = new EmulatorClientOptions(gameRom)
            {
                FullScreen = fullScreen,
                Magnification = magnification,
                Controller = controller
            };
            new EmulatorClient(options).Run();
        }

        private static void Emulate(EmulatorClientOptions options)
        {
            new EmulatorClient(options).Run();
        }
    }
}
