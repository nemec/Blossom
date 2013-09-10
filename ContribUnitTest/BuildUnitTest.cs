using System.Collections.Generic;
using Blossom.Contrib;
using Blossom.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ContribUnitTest
{
    [TestClass]
    public class BuildUnitTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var build = new Build(new SimpleConsoleLogger
                {
                    DisplayLogLevel = LogLevel.Verbose
                });

            var actual = build.BuildProject(
                @"C:\Users\nemecd\prg\HotCornerBarrier\HotCornerBarrier\HotCornerBarrier.csproj",
                @"C:\Users\nemecd\tmp\buildout", new Dictionary<string, string>
                    {
                        { "Configuration", "Debug" },
                        { "Platform", "x86" }
                    });

            Assert.IsTrue(actual);
        }
    }
}
