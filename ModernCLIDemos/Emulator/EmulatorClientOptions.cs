using System.IO;

namespace Emulator
{ 
    public record EmulatorClientOptions(FileInfo GameRom)
    {
        public int Magnification { get; init; } = EmulatorClient.DEFAULT_MAGNIFICATION;
        public bool FullScreen { get; init; } = false;
        public ControllerType Controller { get; init; } = ControllerType.Keyboard;
    }
}