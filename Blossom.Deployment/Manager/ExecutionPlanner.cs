﻿using Blossom.Deployment.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blossom.Deployment.Manager
{
    internal static class ExecutionPlanner
    {
        internal static IEnumerable<ExecutionPlan> GetExecutionPlans<T>(
            DeploymentConfig<T> config,
            IEnumerable<MethodInfo> initialization,
            IEnumerable<MethodInfo> tasks,
            IEnumerable<MethodInfo> cleanup,
            IEnumerable<string> forRoles)
        {
            var roleMap = CreateRolemap(config);
            return roleMap.Select(
                m => CreateExecutionPlan(m.Key, m.Value,
                        initialization, tasks, cleanup)).ToArray();
        }

        private static Dictionary<Host, HashSet<string>> CreateRolemap<T>(
            DeploymentConfig<T> config)
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
            Host host, HashSet<string> roles,
            IEnumerable<MethodInfo> initialization,
            IEnumerable<MethodInfo> tasks,
            IEnumerable<MethodInfo> cleanup)
        {
            var tasksForHost = new HashSet<MethodInfo>();
            foreach (var task in tasks)
            {
                var hostAttrs = task.GetCustomAttributes<HostAttribute>()
                    .Select(h => h.Host).Distinct();
                var roleAttrs = task.GetCustomAttributes<RoleAttribute>()
                    .Select(r => r.Role).Distinct();

                if ((!hostAttrs.Any() && !roleAttrs.Any()) ||
                    hostAttrs.Contains(host.Hostname) ||
                    hostAttrs.Contains(host.Alias) ||
                    roleAttrs.Any(roles.Contains))
                {
                    tasksForHost.Add(task);
                    break;
                }
            }

            // If there are no defined roles or tasks, run every task.
            if(!tasksForHost.Any())
            {
                tasksForHost.UnionWith(tasks);
            }

            var resolver = new DependencyResolver(tasksForHost, tasks);
            return new ExecutionPlan(host,
                initialization.
                    Concat(resolver.OrderTasks()).
                    Concat(cleanup).ToArray());
        }
    }
}
