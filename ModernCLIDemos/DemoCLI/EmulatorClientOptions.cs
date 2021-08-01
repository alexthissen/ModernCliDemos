using System.IO;

namespace DemoCLI
{
    public class EmulatorClientOptions
    {
        public int Magnification { get; set; }
        public FileInfo BootRom { get; set; }
        public FileInfo GameRom { get; set; }
        public bool FullScreen { get; set; }
        public ControllerType Controller { get; set; }

        public static EmulatorClientOptions Default
        {
            get => new()
            {
                Magnification = EmulatorClient.DEFAULT_MAGNIFICATION,
                FullScreen = false,
                Controller = ControllerType.Keyboard
            };
        }

        public record EmulatorClientOptions2(
            int Magnification,
            bool FullScreen,
            ControllerType Controller,
            FileInfo BootRom,
            FileInfo GameRom
        );
    }
}