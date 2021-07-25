using System;
using System.IO;

namespace DemoCLIDragonFruit
{
    class Program
    {
        /// <summary>
        /// Very cool tool
        /// </summary>
        /// <param name="verbose">Verbose output</param>
        /// <param name="count">Count of greetings</param>
        /// <param name="greeting">Actual greeting</param>
        /// <param name="file">File to send output to</param>
        /// <returns></returns>
        static int Main(bool verbose, int count = 1, string greeting = "Yo", FileInfo file = null)
        {
            for (int i = 0; i < count; i++)
            {
                Console.WriteLine($"Hello, {greeting}");
            }

            return 0;
        }
    }
}
