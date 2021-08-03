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
            rootCommand.TreatUnmatchedTokensAsErrors = true;

            rootCommand.Handler = CommandHandler.Create<int, bool, ControllerType, FileInfo, FileInfo>(
                (magnification, fullScreen, controller, bootRom, gameRom) =>
                {
                    EmulatorClientOptions options = new EmulatorClientOptions(gameRom)
                    {
                        FullScreen = fullScreen,
                        Magnification = magnification,
                        BootRom = bootRom,
                        Controller = controller
                    };
                    StartEmulator(options);
                });
            //rootCommand.Handler = CommandHandler.Create<int, bool, ControllerType, FileInfo, FileInfo>(StartEmulator);
            //rootCommand.Handler = CommandHandler.Create<EmulatorClientOptions>(StartEmulator);

            // Arguments
            rootCommand.AddArgument(
                new Argument<FileInfo>("gamerom", "Game ROM file"));

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

            // Parse command-line
            Parser parser = new CommandLineBuilder(rootCommand).UseDefaults().Build();
            return parser.Invoke(args);
        }

        private static void StartEmulator(int magnification, bool fullScreen, ControllerType controller, FileInfo bootRom, FileInfo gameRom)
        {
            EmulatorClientOptions options = new EmulatorClientOptions(gameRom)
            {
                FullScreen = fullScreen,
                Magnification = magnification,
                BootRom = bootRom,
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
        static int Main(FileInfo gameRom, bool fullScreen = false, 
            int magnification = 4, ControllerType controller = ControllerType.Keyboard, 
            FileInfo bootRom = null)
        {
            StartEmulator(magnification, fullScreen, controller, bootRom, gameRom);
            return 0;
        }

        //private static void HandleException(Exception exception, InvocationContext context)
        //{
        //    context.Console.ResetTerminalForegroundColor();
        //    context.Console.SetTerminalForegroundColor(ConsoleColor.Red);

        //    if (exception is TargetInvocationException tie && tie.InnerException is object)
        //    {
        //        exception = tie.InnerException;
        //    }

        //    if (exception is OperationCanceledException)
        //    {
        //        context.Console.Error.WriteLine("Operation has been canceled.");
        //    }
        //    else if (exception is CommandException command)
        //    {
        //        context.Console.Error.WriteLine($"Command '{context.ParseResult.CommandResult.Command.Name}' failed:");
        //        context.Console.Error.WriteLine($"\t{command.Message}");

        //        if (command.InnerException != null)
        //        {
        //            context.Console.Error.WriteLine();
        //            context.Console.Error.WriteLine(command.InnerException.ToString());
        //        }
        //    }

        //    context.Console.ResetTerminalForegroundColor();
        //    context.ExitCode = 1;
        //}

    }
}
