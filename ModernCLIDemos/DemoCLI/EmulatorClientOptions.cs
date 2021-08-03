using System.IO;

namespace DemoCLI
{
    public class EmulatorClientOptions2
    {
        public EmulatorClientOptions2(FileInfo gameRom)
        {
            this.GameRom = gameRom;
        }
        public int Magnification { get; set; }
        public FileInfo BootRom { get; set; }
        public FileInfo GameRom { get; private set; }
        public bool FullScreen { get; set; }
        public ControllerType Controller { get; set; }
    }

    public record EmulatorClientOptions3(
        int Magnification,
        bool FullScreen,
        ControllerType Controller,
        FileInfo BootRom,
        FileInfo GameRom
    );

    public record EmulatorClientOptions(FileInfo GameRom)
    {
        public FileInfo BootRom;
        public int Magnification { get; init; } = EmulatorClient.DEFAULT_MAGNIFICATION;
        public bool FullScreen { get; init; } = false;
        public ControllerType Controller { get; init; } = ControllerType.Keyboard;
    }
}