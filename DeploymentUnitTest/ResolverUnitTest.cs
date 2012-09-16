using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Blossom.Deployment;
using Blossom.Deployment.Dependencies;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using Blossom.Deployment.Exceptions;

namespace DeploymentUnitTest
{
    public class InvalidTaskDependency
    {
        [Task]
        [Depends("ThisIsInvalidTask")]
        public void InvalidTask() { }
    }

    public class DirectCircularDependency
    {
        [Task]
        [Depends("Task2")]
        public void Task1() { }

        [Task]
        [Depends("Task1")]
        public void Task2() { }
    }

    public class IndirectCircularDependency
    {
        [Task]
        [Depends("Task2")]
        public void Task1() { }

        [Task]
        [Depends("Task3")]
        public void Task2() { }

        [Task]
        [Depends("Task1")]
        public void Task3() { }
    }

    public class SelfCircularDependency
    {
        [Task]
        [Depends("Task1")]
        public void Task1() { }
    }

    public class NoDependencies
    {
        [Task]
        public void ZTask() { }

        [Task]
        public void ATask() { }

        [Task]
        public void KTask() { }
    }

    public class WithMultipleExecutionStandalone
    {
        [Task]
        [AllowMultipleExecution(Standalone = true)]
        public void Task1() { }

        [Task]
        [Depends("Task1")]
        public void Task2() { }

        [Task]
        [Depends("Task1")]
        public void Task3() { }

        [Task]
        [Depends("Task2")]
        public void Task4() { }
    }

    public class WithMultipleExecutionNonStandalone
    {
        [Task]
        [AllowMultipleExecution(Standalone=false)]
        public void Task1() { }

        [Task]
        [Depends("Task1")]
        public void Task2() { }

        [Task]
        [Depends("Task1")]
        public void Task3() { }

        [Task]
        [Depends("Task2")]
        public void Task4() { }
    }

    public class Resolver
    {
        [Task]
        [Depends("Task2")]
        public void Task1() { }

        [Task]
        public static void Task2(IDeploymentContext context) { }

        [Task]
        public void Task3() { }

        [Task]
        [Depends("Task1")]
        [Depends("Task5")]
        public void Task4() { }

        [Task]
        [Depends("Task3")]
        public void Task5() { }
    }

    [TestClass]
    public class ResolverUnitTest
    {
        [TestMethod]
        [ExpectedException(typeof(UnknownTaskException))]
        public void GetTaskForName_WithInvalidTaskName_ThrowsUnknownTaskException()
        {
            // Arrange
            var resolver = new DependencyResolver(new InvalidTaskDependency());

            // Act
            resolver.OrderTasks();
        }

        [TestMethod]
        [ExpectedException(typeof(CircularTaskDependencyException))]
        public void Resolve_WhereTasksHaveDirectCircularDependencies_ThrowsCircularTaskDependencyException()
        {
            // Arrange
            var resolver = new DependencyResolver(new DirectCircularDependency());

            // Act
            resolver.OrderTasks();
        }

        [TestMethod]
        [ExpectedException(typeof(CircularTaskDependencyException))]
        public void Resolve_WhereTasksHaveIndirectCircularDependencies_ThrowsCircularTaskDependencyException()
        {
            // Arrange
            var resolver = new DependencyResolver(new IndirectCircularDependency());

            // Act
            resolver.OrderTasks();
        }

        [TestMethod]
        [ExpectedException(typeof(CircularTaskDependencyException))]
        public void Resolve_WhereTaskHasDependencyOnSelf_ThrowsCircularTaskDependencyException()
        {
            // Arrange
            var resolver = new DependencyResolver(new SelfCircularDependency());

            // Act
            resolver.OrderTasks();
        }

        [TestMethod]
        public void Resolve_WhereTasksHaveNoDependencies_OrdersLexigraphically()
        {
            // Arrange
            var t = typeof(NoDependencies);
            var resolver = new DependencyResolver(new NoDependencies());

            var expected = new List<MethodInfo>
            {
                t.GetMethod("ATask"),
                t.GetMethod("KTask"),
                t.GetMethod("ZTask")
            };

            // Act
            var actual = resolver.OrderTasks().ToList();

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Resolve_WhereTaskEnablesMultipleExecutionForStandaloneTask_IncludesTaskMultipleTimes()
        {
            // Arrange
            var t = typeof(WithMultipleExecutionStandalone);
            var resolver = new DependencyResolver(new WithMultipleExecutionStandalone());

            var expected = new List<MethodInfo>
            {
                t.GetMethod("Task1"),
                t.GetMethod("Task1"),
                t.GetMethod("Task2"),
                t.GetMethod("Task1"),
                t.GetMethod("Task3"),
                t.GetMethod("Task4")
            };

            // Act
            var actual = resolver.OrderTasks().ToList();

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Resolve_WhereTaskEnablesMultipleExecutionForNonStandaloneTask_ExcludesTaskAsNonDependency()
        {
            // Arrange
            var t = typeof(WithMultipleExecutionNonStandalone);
            var resolver = new DependencyResolver(new WithMultipleExecutionNonStandalone());

            var expected = new List<MethodInfo>
            {
                t.GetMethod("Task1"),
                t.GetMethod("Task2"),
                t.GetMethod("Task1"),
                t.GetMethod("Task3"),
                t.GetMethod("Task4")
            };

            // Act
            var actual = resolver.OrderTasks().ToList();

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void TestResolver()
        {
            // Arrange
            var t = typeof(Resolver);
            var resolver = new DependencyResolver(new Resolver());

            var expected = new List<MethodInfo>
            {
                t.GetMethod("Task2"),
                t.GetMethod("Task1"),
                t.GetMethod("Task3"),
                t.GetMethod("Task5"),
                t.GetMethod("Task4")
            };

            // Act
            var actual = resolver.OrderTasks().ToList();

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
