using System;
using Blossom.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap.AutoMocking;
using Blossom;
using System.IO;
using Rhino.Mocks;
using Blossom.Operations;
using System.Text;

namespace OperationsUnitTest
{
    [TestClass]
    public class PromptUnitTest
    {
        RhinoAutoMocker<ILocalOperations> MockLocalOperations { get; set; }
        IDeploymentContext MockContext { get; set; }

        [TestInitialize]
        public void Setup()
        {
            MockLocalOperations = new RhinoAutoMocker<ILocalOperations>();
            MockContext = MockLocalOperations.Get<IDeploymentContext>();
        }

        [TestMethod]
        [ExpectedException(typeof(NonInteractiveSessionException))]
        public void Prompt_InNonInteractiveEnvironment_ThrowsNonInteractiveSessionException()
        {
            // Arrange
            MockContext.Stub(r => r.InteractionType).Return(
                InteractionType.NonInteractive
            );

            // Act
            ILocalOperations operations = new BasicLocalOperations(MockContext);
            operations.PromptWithNoValidation("What?");
        }

        [TestMethod]
        public void Prompt_WithSampleResponseInInteractiveEnvironment_ReturnsResponse()
        {
            // Arrange
            MockContext.Stub(r => r.InteractionType).Return(
                InteractionType.AskForInput
            );
            const string sampleResponse = "Response.";
            var input = new StringReader(sampleResponse + "\n");

            // Act
            ILocalOperations operations = new BasicLocalOperations(MockContext);
            var response = operations.PromptWithNoValidation("What?", inputStream: input);

            // Assert
            Assert.AreEqual(sampleResponse, response);
        }

        [TestMethod]
        public void Prompt_WithDefaultResponseAndNormalInput_ReturnsNormalInput()
        {
            // Arrange
            MockContext.Stub(r => r.InteractionType).Return(
                InteractionType.AskForInput
            );
            const string normalInput = "Response.";
            const string defaultResponse = "Default.";
            var input = new StringReader(normalInput + "\n");

            // Act
            ILocalOperations operations = new BasicLocalOperations(MockContext);
            var response = operations.PromptWithNoValidation("What?", 
                defaultResponse, inputStream: input);

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
            MockContext.Stub(r => r.InteractionType).Return(
                InteractionType.UseDefaults
            );
            const string defaultResponse = "Default.";

            // Act
            ILocalOperations operations = new BasicLocalOperations(MockContext);
            var response = operations.PromptWithNoValidation("What?", defaultResponse);

            // Assert
            Assert.AreEqual(defaultResponse, response);
        }

        [TestMethod]
        [ExpectedException(typeof(NonInteractiveSessionException))]
        public void Prompt_WithNoDefaultResponseInUseDefaultsEnvironment_ThrowsNonInteractiveSessionException()
        {
            // Arrange
            MockContext.Stub(r => r.InteractionType).Return(
                InteractionType.UseDefaults
            );

            // Act
            ILocalOperations operations = new BasicLocalOperations(MockContext);
            operations.PromptWithNoValidation("What?");
        }

        [TestMethod]
        public void Prompt_WithStringValidationCallable_RequiresValidInput()
        {
            // Arrange
            MockContext.Stub(r => r.InteractionType).Return(
                InteractionType.AskForInput
            );
            const string validText = "welcome";
            var input = new StringBuilder();
            input.AppendLine("invalid input");
            input.AppendLine(validText);

            Func<string, bool> validationCallback = (resp => resp == validText);

            // Act
            ILocalOperations operations = new BasicLocalOperations(MockContext);
            var response = operations.PromptWithCallbackValidation("What?",
                validationCallback, inputStream: new StringReader(input.ToString()));

            // Assert
            Assert.AreEqual(validText, response);
        }

        [TestMethod]
        public void Prompt_WithStringValidationRegex_RequiresValidInput()
        {
            // Arrange
            MockContext.Stub(r => r.InteractionType).Return(
                InteractionType.AskForInput
            );
            const string validText = "92389";
            var input = new StringBuilder();
            input.AppendLine("invalid input");
            input.AppendLine(validText);

            const string validationRegex = @"\d+";

            // Act
            ILocalOperations operations = new BasicLocalOperations(MockContext);
            var response = operations.PromptWithRegexValidation("What?",
                validationRegex, inputStream: new StringReader(input.ToString()));

            // Assert
            Assert.AreEqual(validText, response);
        }

        [TestMethod]
        public void Prompt_WithStringValidationRegex_StopsAfterValidInput()
        {
            // Arrange
            MockContext.Stub(r => r.InteractionType).Return(
                InteractionType.AskForInput
            );
            const string validText = "92389";
            var input = new StringBuilder();
            input.AppendLine("invalid input");
            input.AppendLine(validText);
            input.AppendLine("more text");

            const string validationRegex = @"\d+";

            // Act
            ILocalOperations operations = new BasicLocalOperations(MockContext);
            var response = operations.PromptWithRegexValidation("What?",
                validationRegex, inputStream: new StringReader(input.ToString()));

            // Assert
            Assert.AreEqual(validText, response);
        }
    }
}
