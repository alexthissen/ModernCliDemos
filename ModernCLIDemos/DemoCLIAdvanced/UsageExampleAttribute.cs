using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedCLI
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal class ExampleUsageAttribute: Attribute
    {
        public ExampleUsageAttribute(string example, string explanation)
        {
            Example = example;
            Explanation = explanation;
        }

        public string Example { get; set; }
        public string Explanation { get; }
    }
}
