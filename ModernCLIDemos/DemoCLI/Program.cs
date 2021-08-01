using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Help;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DemoCLI
{
    class Program
    {
        static void NormalMain()
        {
            new EmulatorClient().Run();
        }

        public static int Main(string[] args)
        {
            RootCommand rootCommand = new RootCommand("Atari Lynx Emulator");
            rootCommand.TreatUnmatchedTokensAsErrors = false;

            rootCommand.Handler = CommandHandler.Create<int, bool, ControllerType, FileInfo, FileInfo>(
                (magnification, fullScreen, controller, bootRom, gameRom) =>
                {
                    EmulatorClientOptions options = new EmulatorClientOptions()
                    {
                        FullScreen = fullScreen,
                        Magnification = magnification,
                        BootRom = bootRom,
                        GameRom = gameRom,
                        Controller = controller
                    };
                    StartEmulator(options);
                });
            //rootCommand.Handler = CommandHandler.Create<int, bool, ControllerType, FileInfo, FileInfo>(StartEmulator);
            //rootCommand.Handler = CommandHandler.Create<EmulatorClientOptions>(StartEmulator);

            // Arguments
            rootCommand.AddArgument(new Argument<FileInfo>("gamerom", "Game ROM file"));

            // Options
            rootCommand.AddOption(new Option<bool>(new[] { "--fullscreen", "-f" }, () => false, "Run full screen"));
            rootCommand.AddOption(
                new Option<ControllerType>(
                    new string[] { "--controller", "-c" },
                    () => ControllerType.Keyboard,
                    "Type of controller to use"
                )
            );

            Option<int> magnificationOption = new Option<int>("--magnification", "Magnification of screen");
            magnificationOption.AddAlias("-m");
            magnificationOption.AddValidator(option =>
            {
                // Dangerous to read value here, as it might not be a parseable value
                //int value = option.GetValueOrDefault<int>();

                if (Int32.TryParse(option.Tokens[0].Value, out int value) || value <= 0 || value > 20)
                    return "Magnification must be an integer value between 1 and 20";
                return null;
            });
            magnificationOption.SetDefaultValue(4);
            magnificationOption.IsRequired = true;
            rootCommand.AddOption(magnificationOption);

            Parser parser = new CommandLineBuilder(rootCommand).UseDefaults().Build();
            return parser.Invoke(args);
        }

        private static void StartEmulator(int magnification, bool fullScreen, ControllerType controller, FileInfo bootRom, FileInfo gameRom)
        {
            EmulatorClientOptions options = new EmulatorClientOptions()
            {
                FullScreen = fullScreen,
                Magnification = magnification,
                BootRom = bootRom,
                GameRom = gameRom,
                Controller = controller
            };

            StartEmulator(options);
        }

        private static void StartEmulator(EmulatorClientOptions options)
        {
            new EmulatorClient(options).Run();
        }

        /// <summary>
        /// Atari Lynx Emulator from DragonFruit
        /// </summary>
        /// <param name="gameRom">Game ROM filename</param>
        /// <param name="fullScreen">Run emulator fullscreen</param>
        /// <param name="magnification">Magnification of screen</param>
        /// <param name="controller">Controller type</param>
        /// <param name="bootRom">Optional replacement boot ROM</param>
        /// <returns>Always zero</returns>
        static int MainDragonFruit(FileInfo gameRom, bool fullScreen = false, 
            int magnification = 4, ControllerType controller = ControllerType.Keyboard, 
            FileInfo bootRom = null)
        {
            StartEmulator(magnification, fullScreen, controller, bootRom, gameRom);
            return 0;
        }
    }
}
