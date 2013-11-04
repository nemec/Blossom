using Blossom.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blossom.Manager
{
    internal static class ExecutionPlanner
    {
        internal static IEnumerable<ExecutionPlan> GetExecutionPlans<T>(
            DeploymentConfig<T> config,
            IEnumerable<MethodInfo> initialization,
            IEnumerable<MethodInfo> tasks,
            IEnumerable<MethodInfo> cleanup,
            bool ignoreDependencies)
        {
            var roleMap = CreateRolemap(config);
            return roleMap.Select(
                m => CreateExecutionPlan(
                    m.Key,
                    m.Value,
                    initialization, 
                    tasks.ToList(), 
                    cleanup, 
                    ignoreDependencies)).ToArray();
        }

        private static Dictionary<IHost, HashSet<string>> CreateRolemap<T>(
            DeploymentConfig<T> config)
        {
            var map = new Dictionary<IHost, HashSet<string>>();

            if (config == null || config.Hosts == null)
            {
                return map;
            }

            foreach (var host in config.Hosts)
            {
                HashSet<string> roles;
                if (!map.TryGetValue(host, out roles))
                {
                    roles = new HashSet<string>();
                    map[host] = roles;
                }
                if (host.Roles == null) continue;
                if (config.Roles != null && config.Roles.Any())
                {
                    roles.UnionWith(
                        host.Roles.Split(';')
                            .Intersect(config.Roles));
                }
                else
                {
                    roles.UnionWith(host.Roles.Split(';'));
                }
            }
            return map;
        }

        private static ExecutionPlan CreateExecutionPlan(
            IHost host, ICollection<string> roles,
            IEnumerable<MethodInfo> initialization,
            List<MethodInfo> tasks,
            IEnumerable<MethodInfo> cleanup,
            bool ignoreDependencies)
        {
            var tasksForHost = new HashSet<MethodInfo>();
            foreach (var task in tasks)
            {
                var hostAttrs = task.GetCustomAttributes<HostAttribute>()
                    .Select(h => h.Host).Distinct().ToList();
                var roleAttrs = task.GetCustomAttributes<RoleAttribute>()
                    .Select(r => r.Role).Distinct().ToList();

                // If the task is neither tagged with a specific Host nor Role
                // If the task is tagged with a Host and we're that host
                // If the task is tagged with a Role and our host belongs to the role
                if ((!hostAttrs.Any() && !roleAttrs.Any()) ||
                    hostAttrs.Contains(host.Hostname) ||
                    hostAttrs.Contains(host.Alias) ||
                    roleAttrs.Any(roles.Contains))
                {
                    tasksForHost.Add(task);
                }
            }

            // If there are no defined roles or tasks, run every task.
            if(!tasksForHost.Any())
            {
                tasksForHost.UnionWith(tasks);
            }

            if (ignoreDependencies)
            {
                return new ExecutionPlan(host,
                    initialization
                        .Concat(tasksForHost)
                        .Concat(cleanup));
            }

            var resolver = new DependencyResolver(tasksForHost, tasks);
            return new ExecutionPlan(host,
                initialization.
                    Concat(resolver.OrderTasks()).
                    Concat(cleanup));
        }
    }
}
