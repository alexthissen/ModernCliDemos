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
        /// <returns>Always zero</returns>
        static int Main(FileInfo gameRom, bool fullScreen = false,
            int magnification = 4, ControllerType controller = ControllerType.Keyboard)
        {
            StartEmulator(magnification, fullScreen, controller, gameRom);
            return 0;
        }

        private static void StartEmulator(int magnification, bool fullScreen, ControllerType controller, FileInfo gameRom)
        {
            EmulatorClientOptions options = new EmulatorClientOptions(gameRom)
            {
                FullScreen = fullScreen,
                Magnification = magnification,
                Controller = controller
            };
            new EmulatorClient(options).Run();
        }
    }
}
