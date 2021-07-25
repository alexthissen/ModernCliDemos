using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Help;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Linq;
using System.Threading.Tasks;

namespace DemoCLI
{
    class Program
    {
        public static async Task<int> Main(string[] args)
        {
            RootCommand rootCommand = new RootCommand("Steam Command-line Interface");
            rootCommand.AddArgument(new Argument("first"));
            rootCommand.AddArgument(new Argument("second"));
            rootCommand.TreatUnmatchedTokensAsErrors = false;
            rootCommand.Handler = CommandHandler.Create<InvocationContext, string, string>((context, first, second) =>
                {
                    context.Console.Out.Write($"Hello {first} {second}");
                });
            var builder = new CommandLineBuilder(rootCommand)
                .UseMiddleware(async (context, next) => {
                    context.Console.Out.Write(context.ParseResult.Directives.Count().ToString());
                    await next(context); })
                .UseDefaults();
            
            //builder.ConfigureConsole(console => console.AddModelBinder)
            var parser = builder.Build();
            return await parser.InvokeAsync(args);
        }
    }
}
