using Emulator;
using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.IO;
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
            var gameRomArgument = new Argument<FileInfo>("gamerom", "Game ROM file");
            rootCommand.AddArgument(gameRomArgument);

            // Options
            var fullScreenOption = new Option<bool>(new[] { "--fullscreen", "-f" }, () => false, "Run full screen");
            var controllerTypeOption = new Option<ControllerType>(
                new string[] { "--controller", "-c" },
                () => ControllerType.Keyboard,
                "Type of controller to use");
            rootCommand.AddOption(fullScreenOption);
            rootCommand.AddOption(controllerTypeOption);

            Option<int> magnificationOption = new Option<int>("--magnification", "Magnification of screen");
            magnificationOption.AddAlias("-m");
            magnificationOption.SetDefaultValue(4);
            magnificationOption.IsRequired = false;
            magnificationOption.AddValidator(option =>
            {
                // Dangerous to read value here, as it might not be a parseable value
                //int value = option.GetValueOrDefault<int>();

                if (option.Token is null) return;
                if (!Int32.TryParse(option.Tokens[0].Value, out int value) || value <= 0 || value > 20)
                    option.ErrorMessage = "Magnification must be an integer value between 1 and 20";
            });
            rootCommand.AddOption(magnificationOption);

            // Order of arguments and options must match method signature
            rootCommand.SetHandler(
                (FileInfo gameRom, bool fullScreen, ControllerType controller, int magnification) =>
                {
                    EmulatorClientOptions options = new EmulatorClientOptions(gameRom)
                    {
                        FullScreen = fullScreen,
                        Magnification = magnification,
                        Controller = controller
                    };
                    Emulate(options);
                    return Task.FromResult(0);
                },
                gameRomArgument, fullScreenOption, controllerTypeOption, magnificationOption);

            // Alternatively, provide separate function instead of lambda
            //rootCommand.SetHandler(Emulate, magnificationOption, fullScreenOption, controllerTypeOption, gameRomArgument);

            // Create command-line parser
            Parser parser = new CommandLineBuilder(rootCommand)
                .UseDefaults()
                .Build();

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
