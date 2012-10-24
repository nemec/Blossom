﻿using Blossom.Deployment.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Blossom.Deployment;
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

    public static class TestExtensions
    {
        public static IEnumerable<Invokable> GetTasks(this object taskBlock)
        {
            return taskBlock.GetType().GetMethods().
                Where(t => t.GetCustomAttribute<TaskAttribute>() != null).Select(
                    m => new Invokable
                    {
                        Base = taskBlock,
                        Method = m
                    });
        }
    }

    [TestClass]
    public class ResolverUnitTest
    {

        [TestMethod]
        [ExpectedException(typeof(UnknownTaskException))]
        public void GetTaskForName_WithInvalidTaskName_ThrowsUnknownTaskException()
        {
            // Arrange
            var resolver = new DependencyResolver(new InvalidTaskDependency().GetTasks());

            // Act
            resolver.OrderTasks();
        }

        [TestMethod]
        [ExpectedException(typeof(CircularTaskDependencyException))]
        public void Resolve_WhereTasksHaveDirectCircularDependencies_ThrowsCircularTaskDependencyException()
        {
            // Arrange
            var resolver = new DependencyResolver(new DirectCircularDependency().GetTasks());

            // Act
            resolver.OrderTasks();
        }

        [TestMethod]
        [ExpectedException(typeof(CircularTaskDependencyException))]
        public void Resolve_WhereTasksHaveIndirectCircularDependencies_ThrowsCircularTaskDependencyException()
        {
            // Arrange
            var resolver = new DependencyResolver(new IndirectCircularDependency().GetTasks());

            // Act
            resolver.OrderTasks();
        }

        [TestMethod]
        [ExpectedException(typeof(CircularTaskDependencyException))]
        public void Resolve_WhereTaskHasDependencyOnSelf_ThrowsCircularTaskDependencyException()
        {
            // Arrange
            var resolver = new DependencyResolver(new SelfCircularDependency().GetTasks());

            // Act
            resolver.OrderTasks();
        }

        [TestMethod]
        public void Resolve_WhereTasksHaveNoDependencies_OrdersLexigraphically()
        {
            // Arrange
            var t = typeof(NoDependencies);
            var resolver = new DependencyResolver(new NoDependencies().GetTasks());

            var expected = new List<Invokable>
            {
                new Invokable{ Method = t.GetMethod("ATask") },
                new Invokable{ Method = t.GetMethod("KTask") },
                new Invokable{ Method = t.GetMethod("ZTask") }
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
            var resolver = new DependencyResolver(new WithMultipleExecutionStandalone().GetTasks());

            var expected = new List<Invokable>
            {
                new Invokable{ Method = t.GetMethod("Task1") },
                new Invokable{ Method = t.GetMethod("Task1") },
                new Invokable{ Method = t.GetMethod("Task2") },
                new Invokable{ Method = t.GetMethod("Task1") },
                new Invokable{ Method = t.GetMethod("Task3") },
                new Invokable{ Method = t.GetMethod("Task4") }
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
            var resolver = new DependencyResolver(new WithMultipleExecutionNonStandalone().GetTasks());

            var expected = new List<Invokable>
            {
                new Invokable{ Method = t.GetMethod("Task1") },
                new Invokable{ Method = t.GetMethod("Task2") },
                new Invokable{ Method = t.GetMethod("Task1") },
                new Invokable{ Method = t.GetMethod("Task3") },
                new Invokable{ Method = t.GetMethod("Task4") }
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
            var resolver = new DependencyResolver(new Resolver().GetTasks());

            var expected = new List<Invokable>
            {
                new Invokable{ Method = t.GetMethod("Task2") },
                new Invokable{ Method = t.GetMethod("Task1") },
                new Invokable{ Method = t.GetMethod("Task3") },
                new Invokable{ Method = t.GetMethod("Task5") },
                new Invokable{ Method = t.GetMethod("Task4") }
            };

            // Act
            var actual = resolver.OrderTasks().ToList();

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
