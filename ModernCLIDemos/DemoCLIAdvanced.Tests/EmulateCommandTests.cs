using AdvancedCLI;
using Emulator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spectre.Console;
using Spectre.Console.Testing;
using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;

namespace DemoCLIAdvanced.Tests
{
    [TestClass]
    public class EmulateCommandTests
    {
        [TestMethod]
        public void EmulateCommandCanCancelOnConfirmation1()
        {
            // Arrange
            var console = new TestConsole();
            console.Profile.Capabilities.Interactive = true;
            console.Input.PushCharacter('n');
            console.Input.PushKey(ConsoleKey.Enter);

            var command = new EmulateCommand();
            var options = new EmulatorClientOptions(new FileInfo("game.rom"));

            ParseResult result = command.Parse("-m 8");
            var context = new InvocationContext(result, null);
            context.BindingContext.AddService<IAnsiConsole>(_ => console); 

            // Act
            command.Emulate(options, false, context.BindingContext);

            // Assert
            Assert.AreEqual(2, console.Lines.Count);
        }

        [TestMethod]
        public void EmulateCommandCanCancelOnConfirmation2()
        {
            // Arrange
            var console = new TestConsole();
            console.Profile.Capabilities.Interactive = true;
            console.Input.PushCharacter('n');
            console.Input.PushKey(ConsoleKey.Enter);

            var command = new EmulateCommand();
            var options = new EmulatorClientOptions(new FileInfo("game.rom"));

            // Parse command-line
            Parser parser = new CommandLineBuilder(command)
                .UseDefaults()
                .AddMiddleware(async (context, next) =>
                {
                    context.BindingContext.AddService<IAnsiConsole>(provider => console);
                    await next(context);
                }).Build();

            var c = new System.CommandLine.IO.TestConsole();
            var output = parser.Parse("-m 8 zarlor.bin").Invoke(c);

            // Assert
            Assert.AreEqual(2, console.Lines.Count);
        }
    }
}