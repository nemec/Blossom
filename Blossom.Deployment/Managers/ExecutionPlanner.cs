using Blossom.Deployment.Dependencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Blossom.Deployment
{
    internal static class ExecutionPlanner
    {
        internal static IEnumerable<ExecutionPlan> GetExecutionPlans(
            DeploymentConfig config,
            IEnumerable<Invokable> initialization,
            IEnumerable<Invokable> tasks,
            IEnumerable<Invokable> cleanup)
        {
            var roleMap = CreateRolemap(config);
            return roleMap.Select(
                m => CreateExecutionPlan(m.Key, m.Value,
                        initialization, tasks, cleanup)).ToArray();
        }

        private static Dictionary<Host, HashSet<string>> CreateRolemap(DeploymentConfig config)
        {
            var map = new Dictionary<Host, HashSet<string>>();

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
                roles.UnionWith(host.Roles.Split(' '));
            }
            return map;
        }

        private static ExecutionPlan CreateExecutionPlan(
            Host host, HashSet<string> roles,
            IEnumerable<Invokable> initialization,
            IEnumerable<Invokable> tasks,
            IEnumerable<Invokable> cleanup)
        {
            var tasksForHost = new HashSet<Invokable>();
            foreach (var task in tasks)
            {
                var hostAttrs = task.Method.GetCustomAttributes<HostAttribute>().Select(h => h.Host).Distinct();
                var roleAttrs = task.Method.GetCustomAttributes<RoleAttribute>().Select(r => r.Role).Distinct();

                if (hostAttrs.Contains(host.Hostname) || hostAttrs.Contains(host.Alias))
                {
                    tasksForHost.Add(task);
                    continue;
                }
                foreach (var role in roleAttrs)
                {
                    if (roles.Contains(role))
                    {
                        tasksForHost.Add(task);
                        continue;  // Only add the task once per host
                    }
                }
                if (!hostAttrs.Any() && !roleAttrs.Any())
                {
                    tasksForHost.Add(task);
                    continue;
                }
            }
            var resolver = new DependencyResolver(tasksForHost, tasks);
            return new ExecutionPlan(host,
                initialization.
                    Concat(resolver.OrderTasks()).
                    Concat(cleanup).ToArray());
        }
    }
}
