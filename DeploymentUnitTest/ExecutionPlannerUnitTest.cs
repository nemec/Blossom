using System.Reflection;
using Blossom;
using Blossom.Attributes;
using Blossom.Manager;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DeploymentUnitTest
{
    [TestClass]
    public class ExecutionPlannerUnitTest
    {
        public class SomeClass
        {
            [Task]
            public void Task1()
            {
            }

            [Task]
            public void Task2()
            {
            }
        }

        [TestMethod]
        public void GetExecutionPlan_ForClassWithTwoTasks_RunsTwoTasks()
        {
            var tasks = typeof (SomeClass)
                .GetMethods()
                .Where(m => m.GetCustomAttributes(typeof (TaskAttribute), false).Any());

            var config = new DeploymentConfig
                {
                    Hosts = new[] {new Host()}
                };

            var plans = ExecutionPlanner.GetExecutionPlans(
                config,
                Enumerable.Empty<MethodInfo>(),
                tasks,
                Enumerable.Empty<MethodInfo>(),
                false);

            Assert.AreEqual(2, plans.First().TaskOrder.Count());
        }

        public class SomeClassWithDependency
        {
            [Task]
            public void Task1()
            {
            }

            [Task]
            [Depends("Task1")]
            public void Task2()
            {
            }
        }

        [TestMethod]
        public void GetExecutionPlan_ForClassWithTwoTasksNotIgnoringDependencies_RunsBothTasks()
        {
            var tasks = typeof (SomeClassWithDependency)
                .GetMethods()
                .Where(m => CustomAttributeExtensions.GetCustomAttribute<TaskAttribute>(m) != null);

            var config = new DeploymentConfig
                {
                    Hosts = new[] {new Host()}
                };

            var plans = ExecutionPlanner.GetExecutionPlans(
                config,
                Enumerable.Empty<MethodInfo>(),
                tasks,
                Enumerable.Empty<MethodInfo>(),
                false);

            Assert.AreEqual(2, plans.First().TaskOrder.Count());
        }

        [TestMethod]
        public void GetExecutionPlan_ForClassWithTwoTasksIgnoringDependencies_RunsLastTask()
        {
            var tasks = typeof (SomeClassWithDependency)
                .GetMethods()
                .Where(m => CustomAttributeExtensions.GetCustomAttribute<TaskAttribute>(m) != null &&
                            CustomAttributeExtensions.GetCustomAttribute<DependsAttribute>(m) != null);

            var config = new DeploymentConfig
                {
                    Hosts = new[] {new Host()}
                };

            var plans = ExecutionPlanner.GetExecutionPlans(
                config,
                Enumerable.Empty<MethodInfo>(),
                tasks,
                Enumerable.Empty<MethodInfo>(),
                true);

            Assert.AreEqual(1, plans.First().TaskOrder.Count());
        }
    }
}
