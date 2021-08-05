using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.CommandLine
{
    public class CommandException : Exception
    {
        public CommandException(string message)
            : base(message)
        { }

        public CommandException(string message, Exception inner)
        : base(message, inner)
        { }
    }
}
