using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap.AutoMocking;
using Blossom.Deployment;
using System.IO;
using Rhino.Mocks;
using Blossom.Deployment.Ssh;
using Blossom.Deployment.Operations;
using System.Text;

namespace OperationsUnitTest
{
    [TestClass]
    public class PromptUnitTest
    {
        RhinoAutoMocker<ILocalOperations> MockLocalOperations { get; set; }
        IDeploymentContext MockContext { get; set; }
        IShell MockShell { get; set; }
        ISftp MockSftp { get; set; }

        [TestInitialize]
        public void Setup()
        {
            MockLocalOperations = new RhinoAutoMocker<ILocalOperations>();
            MockContext = MockLocalOperations.Get<IDeploymentContext>();
            MockShell = MockLocalOperations.Get<IShell>();
            MockSftp = MockLocalOperations.Get<ISftp>();
        }

        [TestMethod]
        [ExpectedException(typeof(NonInteractiveSessionException))]
        public void Prompt_InNonInteractiveEnvironment_ThrowsNonInteractiveSessionException()
        {
            // Arrange
            MockContext.Stub(r => r.Environment).Return(new Env
            {
                InteractionType = InteractionType.NonInteractive
            });
            var host = new Host();

            // Act
            ILocalOperations operations = new BasicOperations(MockContext, MockShell, MockSftp);
            var response = operations.Prompt("What?");
        }

        [TestMethod]
        public void Prompt_WithSampleResponseInInteractiveEnvironment_ReturnsResponse()
        {
            // Arrange
            MockContext.Stub(r => r.Environment).Return(new Env
            {
                InteractionType = InteractionType.AskForInput
            });
            var sampleResponse = "Response.";
            var host = new Host();
            var input = new StringReader(sampleResponse + "\n");

            // Act
            ILocalOperations operations = new BasicOperations(MockContext, MockShell, MockSftp);
            var response = operations.Prompt("What?", inputStream: input);

            // Assert
            Assert.AreEqual(sampleResponse, response);
        }

        [TestMethod]
        public void Prompt_WithDefaultResponseAndNormalInput_ReturnsNormalInput()
        {
            // Arrange
            MockContext.Stub(r => r.Environment).Return(new Env
            {
                InteractionType = InteractionType.AskForInput
            });
            var normalInput = "Response.";
            var defaultResponse = "Default.";
            var host = new Host();
            var input = new StringReader(normalInput + "\n");

            // Act
            ILocalOperations operations = new BasicOperations(MockContext, MockShell, MockSftp);
            var response = operations.Prompt("What?", 
                defaultResponse:defaultResponse, inputStream: input);

            // Assert
            Assert.AreEqual(normalInput, response);
            /*
            Console.WriteLine(Context.Operations.Prompt("Valid.",
                validateCallable: (r => r == "hello"),
                validationFailedMessage: "Please enter hello."));
            Console.WriteLine(Context.Operations.Prompt("Regex.",
                validateRegex: @"\d+", validationFailedMessage: "Please enter a number."));*/
        }

        [TestMethod]
        public void Prompt_WithDefaultResponseInUseDefaultsEnvironment_ReturnsDefaultValue()
        {
            // Arrange
            MockContext.Stub(r => r.Environment).Return(new Env
            {
                InteractionType = InteractionType.UseDefaults
            });
            var defaultResponse = "Default.";
            var host = new Host();

            // Act
            ILocalOperations operations = new BasicOperations(MockContext, MockShell, MockSftp);
            var response = operations.Prompt("What?",
                defaultResponse: defaultResponse);

            // Assert
            Assert.AreEqual(defaultResponse, response);
        }

        [TestMethod]
        [ExpectedException(typeof(NonInteractiveSessionException))]
        public void Prompt_WithNoDefaultResponseInUseDefaultsEnvironment_ThrowsNonInteractiveSessionException()
        {
            // Arrange
            MockContext.Stub(r => r.Environment).Return(new Env
            {
                InteractionType = InteractionType.UseDefaults
            });
            var host = new Host();

            // Act
            ILocalOperations operations = new BasicOperations(MockContext, MockShell, MockSftp);
            var response = operations.Prompt("What?");
        }

        [TestMethod]
        public void Prompt_WithStringValidationCallable_RequiresValidInput()
        {
            // Arrange
            MockContext.Stub(r => r.Environment).Return(new Env
            {
                InteractionType = InteractionType.AskForInput
            });
            var validText = "welcome";
            var input = new StringBuilder();
            input.AppendLine("invalid input");
            input.AppendLine(validText);

            Func<string, bool> validationCallable = (resp) => resp == validText;
            var host = new Host();

            // Act
            ILocalOperations operations = new BasicOperations(MockContext, MockShell, MockSftp);
            var response = operations.Prompt("What?",
                validateCallable: validationCallable,
                inputStream: new StringReader(input.ToString()));

            // Assert
            Assert.AreEqual(validText, response);
        }

        [TestMethod]
        public void Prompt_WithStringValidationRegex_RequiresValidInput()
        {
            // Arrange
            MockContext.Stub(r => r.Environment).Return(new Env
            {
                InteractionType = InteractionType.AskForInput
            });
            var validText = "92389";
            var input = new StringBuilder();
            input.AppendLine("invalid input");
            input.AppendLine(validText);

            var validationRegex = @"\d+";
            var host = new Host();

            // Act
            ILocalOperations operations = new BasicOperations(MockContext, MockShell, MockSftp);
            var response = operations.Prompt("What?",
                validateRegex: validationRegex,
                inputStream: new StringReader(input.ToString()));

            // Assert
            Assert.AreEqual(validText, response);
        }

        [TestMethod]
        public void Prompt_WithStringValidationRegex_StopsAfterValidInput()
        {
            // Arrange
            MockContext.Stub(r => r.Environment).Return(new Env
            {
                InteractionType = InteractionType.AskForInput
            });
            var validText = "92389";
            var input = new StringBuilder();
            input.AppendLine("invalid input");
            input.AppendLine(validText);
            input.AppendLine("more text");

            var validationRegex = @"\d+";
            var host = new Host();

            // Act
            ILocalOperations operations = new BasicOperations(MockContext, MockShell, MockSftp);
            var response = operations.Prompt("What?",
                validateRegex: validationRegex,
                inputStream: new StringReader(input.ToString()));

            // Assert
            Assert.AreEqual(validText, response);
        }
    }
}
