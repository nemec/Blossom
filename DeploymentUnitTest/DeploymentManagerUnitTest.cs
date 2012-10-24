using System;
using Blossom.Deployment.Attributes;
using Blossom.Deployment.Manager;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Blossom.Deployment;
using System.Collections.Generic;
using System.Linq;

namespace DeploymentUnitTest
{
    internal static class TaskExtensions
    {
        internal static IEnumerable<Invokable> GetObjMethods(this object obj, params string[] name)
        {
            return obj.GetType().GetMethods().Where(m => name.Contains(m.Name)).Select(m => new Invokable
                {
                    Base = obj,
                    Method = m
                }).ToArray();
        }
    }

    internal class HostAttrs
    {
        [Task]
        [Host(DeploymentManagerUnitTest.SomeHostname)]
        public void Task1(IDeploymentContext context) { }
    }

    internal class RoleAttrs
    {
        [Task]
        [Role(DeploymentManagerUnitTest.SomeRole)]
        public void Task1(IDeploymentContext context) { }
    }

    internal class RoleMultipleHosts
    {
        [Task]
        [Role(DeploymentManagerUnitTest.SomeRole)]
        public void Task1(IDeploymentContext context) { }
    }

    internal class MultipleRolesSingleHost
    {
        [Task]
        [Role(DeploymentManagerUnitTest.SomeRole)]
        public void Task1(IDeploymentContext context) { }

        [Task]
        [Role(DeploymentManagerUnitTest.SomeRole2)]
        public void Task2(IDeploymentContext context) { }
    }

    internal class MultipleRolesMultipleHosts
    {
        [Task]
        [Role(DeploymentManagerUnitTest.SomeRole)]
        [Role(DeploymentManagerUnitTest.SomeRole2)]
        public void Task1(IDeploymentContext context) { }
    }

    internal class MultipleExplicitHosts
    {
        [Task]
        [Host(DeploymentManagerUnitTest.SomeHostname)]
        [Host(DeploymentManagerUnitTest.SomeHostname2)]
        public void Task1(IDeploymentContext context) { }
    }

    internal class DuplicateExplicitHosts
    {
        [Task]
        [AllowMultipleExecution]  // Just to smoke out tasks that may be duplicated too
        [Host(DeploymentManagerUnitTest.SomeHostname)]
        [Host(DeploymentManagerUnitTest.SomeHostname)]
        public void Task1(IDeploymentContext context) { }
    }

    internal class DuplicateHostAndAlias
    {
        [Task]
        [AllowMultipleExecution]  // Just to smoke out tasks that may be duplicated too
        [Host(DeploymentManagerUnitTest.SomeHostname)]
        [Host(DeploymentManagerUnitTest.SomeAlias)]
        public void Task1(IDeploymentContext context) { }
    }

    internal class DuplicateRoles
    {
        [Task]
        [AllowMultipleExecution]  // Just to smoke out tasks that may be duplicated too
        [Role(DeploymentManagerUnitTest.SomeRole)]
        [Role(DeploymentManagerUnitTest.SomeRole)]
        public void Task1(IDeploymentContext context) { }
    }

    internal class OverlappingHostAndRole
    {
        [Task]
        [AllowMultipleExecution]  // Just to smoke out tasks that may be duplicated too
        [Host(DeploymentManagerUnitTest.SomeHostname)]
        [Role(DeploymentManagerUnitTest.SomeRole)]
        public void Task1(IDeploymentContext context) { }
    }

    internal class InheritDependencies
    {
        [Task]
        public void Task1(IDeploymentContext context) { }

        [Task]
        [Host(DeploymentManagerUnitTest.SomeHostname)]
        [Depends("Task1")]
        public void Task2(IDeploymentContext context) { }
    }

    internal class InheritRecursiveDependencies
    {
        [Task]
        public void Task1(IDeploymentContext context) { }

        [Task]
        [Depends("Task1")]
        public void Task2(IDeploymentContext context) { }

        [Task]
        [Host(DeploymentManagerUnitTest.SomeHostname)]
        [Depends("Task2")]
        public void Task3(IDeploymentContext context) { }
    }

    internal class UnusedTask
    {
        [Task]
        public void Task1(IDeploymentContext context) { }

        [Task]
        public void Task2(IDeploymentContext context) { }

        [Task]
        [Host(DeploymentManagerUnitTest.SomeHostname)]
        [Depends("Task2")]
        public void Task3(IDeploymentContext context) { }
    }

    internal class InvalidTask
    {
        [Task]
        public void Task1(IDeploymentContext context) { }
    }

    internal class InitializationAndCleanup
    {
        [DeploymentInitialize]
        public void Init(IDeploymentContext context) { }

        [Task]
        [Host(DeploymentManagerUnitTest.SomeHostname)]
        public void Task1(IDeploymentContext context) { }

        [DeploymentCleanup]
        public void Cleanup(IDeploymentContext context) { }
    }

    [TestClass]
    public class DeploymentManagerUnitTest
    {
        public const string SomeHostname = "sonic";
        public const string SomeHostname2 = "tails";
        public const string SomeAlias = "alias";
        public const string SomeRole = "thing";
        public const string SomeRole2 = "otherthing";

        [TestMethod]
        public void GetExecutionPlan_WhereTaskHasOneHostAttributeAndHostProvided_ReturnsOnePlan()
        {
            // Arrange
            var taskblock = new HostAttrs();
            var host = new Host
            {
                Hostname = SomeHostname
            };
            var config = new DeploymentConfig
            {
                Hosts = new List<Host>
                {
                    host
                }
            };

            var expected = new List<ExecutionPlan>
            {
                new ExecutionPlan(host, taskblock.GetObjMethods("Task1"))
            };

            // Act
            var manager = new DeploymentManager(config, taskblock);
            var actual = manager.GetExecutionPlans();

            // Assert
            CollectionAssert.AreEqual(
                expected.OrderBy(p => p.Host.Hostname).ToList(),
                actual.OrderBy(p => p.Host.Hostname).ToList());
        }

        [TestMethod]
        public void GetExecutionPlan_WhereTaskHasOneHostAttributeAndAliasProvided_ReturnsOnePlan()
        {
            // Arrange
            var taskblock = new HostAttrs();
            var host = new Host
            {
                Alias = SomeHostname,
                Hostname = "something else"
            };
            var config = new DeploymentConfig
            {
                Hosts = new List<Host>
                {
                    host
                }
            };

            var expected = new List<ExecutionPlan>
            {
                new ExecutionPlan(host, taskblock.GetObjMethods("Task1"))
            };

            // Act
            var manager = new DeploymentManager(config, taskblock);
            var actual = manager.GetExecutionPlans();

            // Assert
            CollectionAssert.AreEqual(
                expected.OrderBy(p => p.Host.Hostname).ToList(),
                actual.OrderBy(p => p.Host.Hostname).ToList());
        }

        [TestMethod]
        public void GetExecutionPlan_WhereTaskHasOneRoleAttributeAndHostProvided_ReturnsOnePlan()
        {
            // Arrange
            var taskblock = new RoleAttrs();
            var host = new Host
            {
                Hostname = SomeHostname,
                Roles = SomeRole
            };
            var config = new DeploymentConfig
            {
                Hosts = new List<Host>
                {
                    host
                }
            };

            var expected = new List<ExecutionPlan>
            {
                new ExecutionPlan(host, taskblock.GetObjMethods("Task1"))
            };

            // Act
            var manager = new DeploymentManager(config, taskblock);
            var actual = manager.GetExecutionPlans();

            // Assert
            CollectionAssert.AreEqual(
                expected.OrderBy(p => p.Host.Hostname).ToList(),
                actual.OrderBy(p => p.Host.Hostname).ToList());
        }
        
        [TestMethod]
        public void GetExecutionPlan_WhereTaskHasOneRoleAttributeAndMultipleHostsProvided_ReturnsMultiplePlans()
        {
            // Arrange
            var taskblock = new RoleMultipleHosts();
            var host = new Host
            {
                Hostname = SomeHostname,
                Roles = SomeRole
            };
            var host2 = new Host
            {
                Hostname = SomeHostname2,
                Roles = SomeRole
            };
            var config = new DeploymentConfig
            {
                Hosts = new List<Host>
                {
                    host,
                    host2
                }
            };

            var expected = new List<ExecutionPlan>
            {
                new ExecutionPlan(host, taskblock.GetObjMethods("Task1")),
                new ExecutionPlan(host2, taskblock.GetObjMethods("Task1"))
            };

            // Act
            var manager = new DeploymentManager(config, taskblock);
            var actual = manager.GetExecutionPlans();

            // Assert
            CollectionAssert.AreEqual(
                expected.OrderBy(p => p.Host.Hostname).ToList(),
                actual.OrderBy(p => p.Host.Hostname).ToList());
        }

        [TestMethod]
        public void GetExecutionPlan_WhereTaskHasMultipleRoleAttributesAndOneHostProvided_ReturnsOnePlan()
        {
            // Arrange
            var taskblock = new MultipleRolesSingleHost();
            var host = new Host
            {
                Hostname = SomeHostname,
                Roles = String.Format("{0} {1}", SomeRole, SomeRole2)
            };
            var config = new DeploymentConfig
            {
                Hosts = new List<Host>
                {
                    host
                }
            };

            var expected = new List<ExecutionPlan>
            {
                new ExecutionPlan(host, taskblock.GetObjMethods("Task1", "Task2"))
            };

            // Act
            var manager = new DeploymentManager(config, taskblock);
            var actual = manager.GetExecutionPlans();

            // Assert
            CollectionAssert.AreEqual(
                expected.OrderBy(p => p.Host.Hostname).ToList(),
                actual.OrderBy(p => p.Host.Hostname).ToList());
        }

        [TestMethod]
        public void GetExecutionPlan_WhereTaskHasMultipleRoleAttributesAndMultipleHostsProvided_ReturnsMultiplePlans()
        {
            // Arrange
            var taskblock = new MultipleRolesMultipleHosts();
            var host = new Host
            {
                Hostname = SomeHostname,
                Roles = SomeRole
            };
            var host2 = new Host
            {
                Hostname = SomeHostname2,
                Roles = SomeRole2
            };
            var config = new DeploymentConfig
            {
                Hosts = new List<Host>
                {
                    host,
                    host2
                }
            };

            var expected = new List<ExecutionPlan>
            {
                new ExecutionPlan(host, taskblock.GetObjMethods("Task1")),
                new ExecutionPlan(host2, taskblock.GetObjMethods("Task1"))
            };

            // Act
            var manager = new DeploymentManager(config, taskblock);
            var actual = manager.GetExecutionPlans();

            // Assert
            CollectionAssert.AreEqual(
                expected.OrderBy(p => p.Host.Hostname).ToList(),
                actual.OrderBy(p => p.Host.Hostname).ToList());
        }

        [TestMethod]
        public void GetExecutionPlan_WhereTaskHasMultipleExplicitHostsProvided_ReturnsMultiplePlans()
        {
            // Arrange
            var taskblock = new MultipleExplicitHosts();
            var host = new Host
            {
                Hostname = SomeHostname
            };
            var host2 = new Host
            {
                Hostname = SomeHostname2
            };
            var config = new DeploymentConfig
            {
                Hosts = new List<Host>
                {
                    host,
                    host2
                }
            };

            var expected = new List<ExecutionPlan>
            {
                new ExecutionPlan(host, taskblock.GetObjMethods("Task1")),
                new ExecutionPlan(host2, taskblock.GetObjMethods("Task1"))
            };

            // Act
            var manager = new DeploymentManager(config, taskblock);
            var actual = manager.GetExecutionPlans();

            // Assert
            CollectionAssert.AreEqual(
                expected.OrderBy(p => p.Host.Hostname).ToList(),
                actual.OrderBy(p => p.Host.Hostname).ToList());
        }

        [TestMethod]
        public void GetExecutionPlan_WhereTaskHasDuplicateExplicitHostsProvided_ReturnsOnePlan()
        {
            // Arrange
            var taskblock = new DuplicateExplicitHosts();
            var host = new Host
            {
                Hostname = SomeHostname
            };
            var config = new DeploymentConfig
            {
                Hosts = new List<Host>
                {
                    host
                }
            };

            var expected = new List<ExecutionPlan>
            {
                new ExecutionPlan(host, taskblock.GetObjMethods("Task1"))
            };

            // Act
            var manager = new DeploymentManager(config, taskblock);
            var actual = manager.GetExecutionPlans();

            // Assert
            CollectionAssert.AreEqual(
                expected.OrderBy(p => p.Host.Hostname).ToList(),
                actual.OrderBy(p => p.Host.Hostname).ToList());
        }

        [TestMethod]
        public void GetExecutionPlan_WhereTaskHasDuplicateHostAndAliasProvided_ReturnsOnePlan()
        {
            // Arrange
            var taskblock = new DuplicateHostAndAlias();
            var host = new Host
            {
                Hostname = SomeHostname,
                Alias = SomeAlias
            };
            var config = new DeploymentConfig
            {
                Hosts = new List<Host>
                {
                    host
                }
            };

            var expected = new List<ExecutionPlan>
            {
                new ExecutionPlan(host, taskblock.GetObjMethods("Task1"))
            };

            // Act
            var manager = new DeploymentManager(config, taskblock);
            var actual = manager.GetExecutionPlans();

            // Assert
            CollectionAssert.AreEqual(
                expected.OrderBy(p => p.Host.Hostname).ToList(),
                actual.OrderBy(p => p.Host.Hostname).ToList());
        }

        [TestMethod]
        public void GetExecutionPlan_WhereTaskHasDuplicateRolesProvided_ReturnsOnePlan()
        {
            // Arrange
            var taskblock = new DuplicateHostAndAlias();
            var host = new Host
            {
                Hostname = SomeHostname,
                Roles = SomeRole
            };
            var config = new DeploymentConfig
            {
                Hosts = new List<Host>
                {
                    host
                }
            };

            var expected = new List<ExecutionPlan>
            {
                new ExecutionPlan(host, taskblock.GetObjMethods("Task1"))
            };

            // Act
            var manager = new DeploymentManager(config, taskblock);
            var actual = manager.GetExecutionPlans();

            // Assert
            CollectionAssert.AreEqual(
                expected.OrderBy(p => p.Host.Hostname).ToList(),
                actual.OrderBy(p => p.Host.Hostname).ToList());
        }
        
        [TestMethod]
        public void GetExecutionPlan_WhereTaskHasOverlappingHostAndRole_ReturnsOnePlan()
        {
            // Arrange
            var taskblock = new OverlappingHostAndRole();
            var host = new Host
            {
                Hostname = SomeHostname,
                Roles = SomeRole
            };
            var config = new DeploymentConfig
            {
                Hosts = new List<Host>
                {
                    host
                }
            };

            var expected = new List<ExecutionPlan>
            {
                new ExecutionPlan(host, taskblock.GetObjMethods("Task1"))
            };

            // Act
            var manager = new DeploymentManager(config, taskblock);
            var actual = manager.GetExecutionPlans();

            // Assert
            CollectionAssert.AreEqual(
                expected.OrderBy(p => p.Host.Hostname).ToList(),
                actual.OrderBy(p => p.Host.Hostname).ToList());
        }

        [TestMethod]
        public void GetExecutionPlan_WhereTaskHasDependencyNotSpecifiedForHost_AddsDependencyToPlan()
        {
            // Arrange
            var taskblock = new InheritDependencies();
            var host = new Host
            {
                Hostname = SomeHostname
            };
            var config = new DeploymentConfig
            {
                Hosts = new List<Host>
                {
                    host
                }
            };

            var expected = new List<ExecutionPlan>
            {
                new ExecutionPlan(host, taskblock.GetObjMethods("Task1", "Task2"))
            };

            // Act
            var manager = new DeploymentManager(config, taskblock);
            var actual = manager.GetExecutionPlans();

            // Assert
            CollectionAssert.AreEqual(
                expected.OrderBy(p => p.Host.Hostname).ToList(),
                actual.OrderBy(p => p.Host.Hostname).ToList());
        }

        [TestMethod]
        public void GetExecutionPlan_WhereTaskHasRecursiveDependencyNotSpecifiedForHost_AddsDependenciesToPlan()
        {
            // Arrange
            var taskblock = new InheritRecursiveDependencies();
            var host = new Host
            {
                Hostname = SomeHostname
            };
            var config = new DeploymentConfig
            {
                Hosts = new List<Host>
                {
                    host
                }
            };

            var expected = new List<ExecutionPlan>
            {
                new ExecutionPlan(host, taskblock.GetObjMethods("Task1", "Task2", "Task3"))
            };

            // Act
            var manager = new DeploymentManager(config, taskblock);
            var actual = manager.GetExecutionPlans();

            // Assert
            CollectionAssert.AreEqual(
                expected.OrderBy(p => p.Host.Hostname).ToList(),
                actual.OrderBy(p => p.Host.Hostname).ToList());
        }

        [TestMethod]
        public void GetExecutionPlan_WhereTaskNotIncludedAsDependency_IsNotPresentInPlan()
        {
            // Arrange
            var taskblock = new UnusedTask();
            var host = new Host
            {
                Hostname = SomeHostname
            };
            var config = new DeploymentConfig
            {
                Hosts = new List<Host>
                {
                    host
                }
            };

            var expected = new List<ExecutionPlan>
            {
                new ExecutionPlan(host, taskblock.GetObjMethods("Task2", "Task3"))
            };

            // Act
            var manager = new DeploymentManager(config, taskblock);
            var actual = manager.GetExecutionPlans();

            // Assert
            CollectionAssert.AreEqual(
                expected.OrderBy(p => p.Host.Hostname).ToList(),
                actual.OrderBy(p => p.Host.Hostname).ToList());
        }

        [TestMethod]
        public void GetExecutionPlan_WhereTaskHasInitializationAndCleanup_ReturnsInitializationFirstAndCleanupLast()
        {
            // Arrange
            var taskblock = new HostAttrs();
            var host = new Host
            {
                Hostname = SomeHostname
            };
            var config = new DeploymentConfig
            {
                Hosts = new List<Host>
                {
                    host
                }
            };

            var expected = new List<ExecutionPlan>
            {
                new ExecutionPlan(host, taskblock.GetObjMethods("Init", "Task1", "Cleanup"))
            };

            // Act
            var manager = new DeploymentManager(config, taskblock);
            var actual = manager.GetExecutionPlans();

            // Assert
            CollectionAssert.AreEqual(expected.ToList(), actual.ToList());
        }

    }
}
