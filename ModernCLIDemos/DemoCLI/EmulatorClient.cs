﻿using System;

namespace DemoCLI
{
    public class EmulatorClient 
    {
        public const int DEFAULT_MAGNIFICATION = 8;
        private readonly EmulatorClientOptions options;

        public EmulatorClient(EmulatorClientOptions options = null)
        {
            this.options = options;
        }

        public void Run()
        {
            Console.WriteLine("Starting emulator...");
            Console.WriteLine($"\tMagnification {options.Magnification}");
            Console.WriteLine($"\tFull screen {options.Magnification}");
            Console.WriteLine($"\tGame ROM: '{options.GameRom}'");
            Console.WriteLine($"\tBoot ROM: '{options.BootRom}'");
            Console.WriteLine($"\tController type: {options.Controller}");
        }
    }
}