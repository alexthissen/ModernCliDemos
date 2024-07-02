using Spectre.Console;
using System.CommandLine.Binding;

namespace AdvancedCLI
{
    internal class AnsiConsoleBinder : BinderBase<IAnsiConsole>
    {
        protected override IAnsiConsole GetBoundValue(BindingContext bindingContext)
        {
            return (IAnsiConsole)bindingContext.GetService(typeof(IAnsiConsole));
        }
    }
}
