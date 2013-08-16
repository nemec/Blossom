using System;
using System.Reflection;
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
        internal static IEnumerable<MethodInfo> GetObjMethods(this object obj, params string[] name)
        {
            return obj.GetType().GetMethods().Where(m => name.Contains(m.Name)).ToArray();
        }
    }

    internal class HostAttrs : IDeployment
    {
        public IDeploymentContext Context { get; set; }
        public NullConfig Config { get; set; }

        public void InitializeDeployment(IDeploymentContext context, NullConfig config)
        {
            Config = config;
            Context = context;
        }

        [Task]
        [Host(DeploymentManagerUnitTest.SomeHostname)]
        public void Task1() { }
    }

    internal class RoleAttrs : IDeployment
    {
        public IDeploymentContext Context { get; set; }
        public NullConfig Config { get; set; }


        public void InitializeDeployment(IDeploymentContext context, NullConfig config)
        {
            Config = config;
            Context = context;
        }


        [Task]
        [Role(DeploymentManagerUnitTest.SomeRole)]
        public void Task1() { }
    }

    internal class RoleMultipleHosts : IDeployment
    {
        public IDeploymentContext Context { get; set; }
        public NullConfig Config { get; set; }

        public void InitializeDeployment(IDeploymentContext context, NullConfig config)
        {
            Config = config;
            Context = context;
        }

        [Task]
        [Role(DeploymentManagerUnitTest.SomeRole)]
        public void Task1() { }
    }

    internal class MultipleRolesSingleHost : IDeployment
    {
        public IDeploymentContext Context { get; set; }
        public NullConfig Config { get; set; }

        public void InitializeDeployment(IDeploymentContext context, NullConfig config)
        {
            Config = config;
            Context = context;
        }

        [Task]
        [Role(DeploymentManagerUnitTest.SomeRole)]
        public void Task1() { }

        [Task]
        [Role(DeploymentManagerUnitTest.SomeRole2)]
        public void Task2() { }
    }

    internal class MultipleRolesMultipleHosts : IDeployment
    {
        public IDeploymentContext Context { get; set; }
        public NullConfig Config { get; set; }

        public void InitializeDeployment(IDeploymentContext context, NullConfig config)
        {
            Config = config;
            Context = context;
        }

        [Task]
        [Role(DeploymentManagerUnitTest.SomeRole)]
        [Role(DeploymentManagerUnitTest.SomeRole2)]
        public void Task1() { }
    }

    internal class MultipleExplicitHosts : IDeployment
    {
        public IDeploymentContext Context { get; set; }
        public NullConfig Config { get; set; }

        public void InitializeDeployment(IDeploymentContext context, NullConfig config)
        {
            Config = config;
            Context = context;
        }

        [Task]
        [Host(DeploymentManagerUnitTest.SomeHostname)]
        [Host(DeploymentManagerUnitTest.SomeHostname2)]
        public void Task1() { }
    }

    internal class DuplicateExplicitHosts : IDeployment
    {
        public IDeploymentContext Context { get; set; }
        public NullConfig Config { get; set; }

        public void InitializeDeployment(IDeploymentContext context, NullConfig config)
        {
            Config = config;
            Context = context;
        }

        [Task]
        [AllowMultipleExecution]  // Just to smoke out tasks that may be duplicated too
        [Host(DeploymentManagerUnitTest.SomeHostname)]
        [Host(DeploymentManagerUnitTest.SomeHostname)]
        public void Task1() { }
    }

    internal class DuplicateHostAndAlias : IDeployment
    {
        public IDeploymentContext Context { get; set; }
        public NullConfig Config { get; set; }

        public void InitializeDeployment(IDeploymentContext context, NullConfig config)
        {
            Config = config;
            Context = context;
        }

        [Task]
        [AllowMultipleExecution]  // Just to smoke out tasks that may be duplicated too
        [Host(DeploymentManagerUnitTest.SomeHostname)]
        [Host(DeploymentManagerUnitTest.SomeAlias)]
        public void Task1() { }
    }

    internal class DuplicateRoles : IDeployment
    {
        public IDeploymentContext Context { get; set; }
        public NullConfig Config { get; set; }

        public void InitializeDeployment(IDeploymentContext context, NullConfig config)
        {
            Config = config;
            Context = context;
        }

        [Task]
        [AllowMultipleExecution]  // Just to smoke out tasks that may be duplicated too
        [Role(DeploymentManagerUnitTest.SomeRole)]
        [Role(DeploymentManagerUnitTest.SomeRole)]
        public void Task1() { }
    }

    internal class OverlappingHostAndRole : IDeployment
    {
        public IDeploymentContext Context { get; set; }
        public NullConfig Config { get; set; }

        public void InitializeDeployment(IDeploymentContext context, NullConfig config)
        {
            Config = config;
            Context = context;
        }

        [Task]
        [AllowMultipleExecution]  // Just to smoke out tasks that may be duplicated too
        [Host(DeploymentManagerUnitTest.SomeHostname)]
        [Role(DeploymentManagerUnitTest.SomeRole)]
        public void Task1() { }
    }

    internal class InheritDependencies : IDeployment
    {
        public IDeploymentContext Context { get; set; }
        public NullConfig Config { get; set; }

        public void InitializeDeployment(IDeploymentContext context, NullConfig config)
        {
            Config = config;
            Context = context;
        }

        [Task]
        public void Task1() { }

        [Task]
        [Host(DeploymentManagerUnitTest.SomeHostname)]
        [Depends("Task1")]
        public void Task2() { }
    }

    internal class InheritRecursiveDependencies : IDeployment
    {
        public IDeploymentContext Context { get; set; }
        public NullConfig Config { get; set; }

        public void InitializeDeployment(IDeploymentContext context, NullConfig config)
        {
            Config = config;
            Context = context;
        }

        [Task]
        public void Task1() { }

        [Task]
        [Depends("Task1")]
        public void Task2() { }

        [Task]
        [Host(DeploymentManagerUnitTest.SomeHostname)]
        [Depends("Task2")]
        public void Task3() { }
    }

    internal class UnusedTask : IDeployment
    {
        public IDeploymentContext Context { get; set; }
        public NullConfig Config { get; set; }

        public void InitializeDeployment(IDeploymentContext context, NullConfig config)
        {
            Config = config;
            Context = context;
        }

        [Task]
        public void Task1() { }

        [Task]
        public void Task2() { }

        [Task]
        [Host(DeploymentManagerUnitTest.SomeHostname)]
        [Depends("Task2")]
        public void Task3() { }
    }

    internal class InvalidTask : IDeployment
    {
        public IDeploymentContext Context { get; set; }
        public NullConfig Config { get; set; }

        public void InitializeDeployment(IDeploymentContext context, NullConfig config)
        {
            Config = config;
            Context = context;
        }

        [Task]
        public void Task1() { }
    }

    internal class InitializationAndCleanup : IDeployment
    {
        public IDeploymentContext Context { get; set; }
        public NullConfig Config { get; set; }

        public void InitializeDeployment(IDeploymentContext context, NullConfig config)
        {
            Config = config;
            Context = context;
        }

        [DeploymentInitialize]
        public void Init() { }

        [Task]
        [Host(DeploymentManagerUnitTest.SomeHostname)]
        public void Task1() { }

        [DeploymentCleanup]
        public void Cleanup() { }
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
            var taskblock = typeof(HostAttrs);
            var host = new Host
            {
                Hostname = SomeHostname
            };
            var config = new DeploymentConfig
            {
                Hosts = new[]
                {
                    host
                }
            };

            var expected = new List<ExecutionPlan>
            {
                new ExecutionPlan(host, taskblock.GetObjMethods("Task1"))
            };

            // Act
            var manager = new DeploymentManager<HostAttrs>(config);
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
            var taskblock = typeof(HostAttrs);
            var host = new Host
            {
                Alias = SomeHostname,
                Hostname = "something else"
            };
            var config = new DeploymentConfig
            {
                Hosts = new[]
                {
                    host
                }
            };

            var expected = new List<ExecutionPlan>
            {
                new ExecutionPlan(host, taskblock.GetObjMethods("Task1"))
            };

            // Act
            var manager = new DeploymentManager<HostAttrs>(config);
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
            var taskblock = typeof(RoleAttrs);
            var host = new Host
            {
                Hostname = SomeHostname,
                Roles = SomeRole
            };
            var config = new DeploymentConfig
            {
                Hosts = new[]
                {
                    host
                }
            };

            var expected = new List<ExecutionPlan>
            {
                new ExecutionPlan(host, taskblock.GetObjMethods("Task1"))
            };

            // Act
            var manager = new DeploymentManager<RoleAttrs>(config);
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
            var taskblock = typeof(RoleMultipleHosts);
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
                Hosts = new[]
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
            var manager = new DeploymentManager<RoleMultipleHosts>(config);
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
            var taskblock = typeof(MultipleRolesSingleHost);
            var host = new Host
            {
                Hostname = SomeHostname,
                Roles = String.Format("{0} {1}", SomeRole, SomeRole2)
            };
            var config = new DeploymentConfig
            {
                Hosts = new[]
                {
                    host
                }
            };

            var expected = new List<ExecutionPlan>
            {
                new ExecutionPlan(host, taskblock.GetObjMethods("Task1", "Task2"))
            };

            // Act
            var manager = new DeploymentManager<MultipleRolesSingleHost>(config);
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
            var taskblock = typeof(MultipleRolesMultipleHosts);
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
                Hosts = new[]
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
            var manager = new DeploymentManager<MultipleRolesMultipleHosts>(config);
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
            var taskblock = typeof(MultipleExplicitHosts);
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
                Hosts = new[]
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
            var manager = new DeploymentManager<MultipleExplicitHosts>(config);
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
            var taskblock = typeof(DuplicateExplicitHosts);
            var host = new Host
            {
                Hostname = SomeHostname
            };
            var config = new DeploymentConfig
            {
                Hosts = new[]
                {
                    host
                }
            };

            var expected = new List<ExecutionPlan>
            {
                new ExecutionPlan(host, taskblock.GetObjMethods("Task1"))
            };

            // Act
            var manager = new DeploymentManager<DuplicateExplicitHosts>(config);
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
            var taskblock = typeof(DuplicateHostAndAlias);
            var host = new Host
            {
                Hostname = SomeHostname,
                Alias = SomeAlias
            };
            var config = new DeploymentConfig
            {
                Hosts = new[]
                {
                    host
                }
            };

            var expected = new List<ExecutionPlan>
            {
                new ExecutionPlan(host, taskblock.GetObjMethods("Task1"))
            };

            // Act
            var manager = new DeploymentManager<DuplicateHostAndAlias>(config);
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
            var taskblock = typeof(DuplicateRoles);
            var host = new Host
            {
                Hostname = SomeHostname,
                Roles = SomeRole
            };
            var config = new DeploymentConfig
            {
                Hosts = new[]
                {
                    host
                }
            };

            var expected = new List<ExecutionPlan>
            {
                new ExecutionPlan(host, taskblock.GetObjMethods("Task1"))
            };

            // Act
            var manager = new DeploymentManager<DuplicateRoles>(config);
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
            var taskblock = typeof(OverlappingHostAndRole);
            var host = new Host
            {
                Hostname = SomeHostname,
                Roles = SomeRole
            };
            var config = new DeploymentConfig
            {
                Hosts = new[]
                {
                    host
                }
            };

            var expected = new List<ExecutionPlan>
            {
                new ExecutionPlan(host, taskblock.GetObjMethods("Task1"))
            };

            // Act
            var manager = new DeploymentManager<OverlappingHostAndRole>(config);
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
            var taskblock = typeof(InheritDependencies);
            var host = new Host
            {
                Hostname = SomeHostname
            };
            var config = new DeploymentConfig
            {
                Hosts = new[]
                {
                    host
                }
            };

            var expected = new List<ExecutionPlan>
            {
                new ExecutionPlan(host, taskblock.GetObjMethods("Task1", "Task2"))
            };

            // Act
            var manager = new DeploymentManager<InheritDependencies>(config);
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
            var taskblock = typeof(InheritRecursiveDependencies);
            var host = new Host
            {
                Hostname = SomeHostname
            };
            var config = new DeploymentConfig
            {
                Hosts = new[]
                {
                    host
                }
            };

            var expected = new List<ExecutionPlan>
            {
                new ExecutionPlan(host, taskblock.GetObjMethods("Task1", "Task2", "Task3"))
            };

            // Act
            var manager = new DeploymentManager<InheritRecursiveDependencies>(config);
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
            var taskblock = typeof(UnusedTask);
            var host = new Host
            {
                Hostname = SomeHostname
            };
            var config = new DeploymentConfig
            {
                Hosts = new[]
                {
                    host
                }
            };

            var expected = new List<ExecutionPlan>
            {
                new ExecutionPlan(host, taskblock.GetObjMethods("Task2", "Task3"))
            };

            // Act
            var manager = new DeploymentManager<UnusedTask>(config);
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
            var taskblock = typeof(InitializationAndCleanup);
            var host = new Host
            {
                Hostname = SomeHostname
            };
            var config = new DeploymentConfig
            {
                Hosts = new[]
                {
                    host
                }
            };

            var expected = new List<ExecutionPlan>
            {
                new ExecutionPlan(host, taskblock.GetObjMethods("Init", "Task1", "Cleanup"))
            };

            // Act
            var manager = new DeploymentManager<InitializationAndCleanup>(config);
            var actual = manager.GetExecutionPlans();

            // Assert
            CollectionAssert.AreEqual(expected.ToList(), actual.ToList());
        }

    }
}
