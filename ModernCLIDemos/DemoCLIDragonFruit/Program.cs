using Emulator;
using System;
using System.IO;

namespace DragonFruitCLI
{
    class Program
    {
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

        private static void StartEmulator(int magnification, bool fullScreen, ControllerType controller, FileInfo bootRom, FileInfo gameRom)
        {
            EmulatorClientOptions options = new EmulatorClientOptions(gameRom)
            {
                FullScreen = fullScreen,
                Magnification = magnification,
                BootRom = bootRom,
                Controller = controller
            };
            new EmulatorClient(options).Run();
        }
    }
}
