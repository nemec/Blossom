using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap.AutoMocking;
using Blossom.Deployment;
using System.IO;
using Rhino.Mocks;
using Blossom.Deployment.Ssh;

namespace OperationsUnitTest
{
    [TestClass]
    public class PromptUnitTest
    {
        RhinoAutoMocker<IOperations> MockOperations { get; set; }
        IDeploymentContext MockContext { get; set; }
        IShell MockShell { get; set; }
        ISftp MockSftp { get; set; }

        [TestInitialize]
        public void Setup()
        {
            MockOperations = new RhinoAutoMocker<IOperations>();
            MockContext = MockOperations.Get<IDeploymentContext>();
            MockShell = MockOperations.Get<IShell>();
            MockSftp = MockOperations.Get<ISftp>();
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
            IOperations operations = new Operations(MockContext, MockShell, MockSftp);
            var response = operations.Prompt("What?");
        }

        [TestMethod]
        public void Prompt_WithSampleResponseInInteractiveEnvironment_ReturnsResponse()
        {
            MockContext.Stub(r => r.Environment).Return(new Env
            {
                InteractionType = InteractionType.AskForInput
            });
            // Arrange
            var sampleResponse = "Response.";
            var host = new Host();
            var input = new StringReader(sampleResponse + "\n");

            // Act
            IOperations operations = new Operations(MockContext, MockShell, MockSftp);
            var response = operations.Prompt("What?", inputStream: input);

            // Assert
            Assert.AreEqual(sampleResponse, response);
        }

        [TestMethod]
        public void Prompt_WithDefaultResponseAndNormalInput_ReturnsNormalInput()
        {
            Assert.Inconclusive();
            /*Console.WriteLine(Context.Operations.Prompt("Okay.", "Nothing."));
            Console.WriteLine(Context.Operations.Prompt("Valid.",
                validateCallable: (r => r == "hello"),
                validationFailedMessage: "Please enter hello."));
            Console.WriteLine(Context.Operations.Prompt("Regex.",
                validateRegex: @"\d+", validationFailedMessage: "Please enter a number."));*/
        }

        [TestMethod]
        public void Prompt_WithDefaultResponseInLowInteractiveEnvironmentAndNoInput_ReturnsDefaultValue()
        {
            Assert.Inconclusive();
        }
    }
}
