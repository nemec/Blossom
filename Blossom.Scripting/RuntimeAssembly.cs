using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blossom.Deployment;
using System.Reflection;

namespace Blossom.Scripting
{
    public static class RuntimeAssembly
    {
        public static string BuildAssembly(string sourcePath)
        {
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");

            string assemblyName = Path.GetTempFileName();

            CompilerParameters parameters = new CompilerParameters
            {
                OutputAssembly = assemblyName
            };
            parameters.ReferencedAssemblies.Add(typeof(TaskAttribute).Assembly.ManifestModule.Name);

            CompilerResults results = provider.CompileAssemblyFromFile(parameters, sourcePath);

            if (results.Errors.Count > 0)
            {
                foreach (CompilerError error in results.Errors)
                {
                    Console.Error.WriteLine(String.Format("{0}:{1}) {2} - {3}",
                        error.Line, error.Column, error.ErrorNumber, error.ErrorText));
                }
                return null;
            }
            else
            {
                return assemblyName;
            }
        }

        public static Assembly LoadAssembly(string assemblyPath)
        {
            byte[] assemblyBytes = File.ReadAllBytes(assemblyPath);
            try
            {
                File.Delete(assemblyPath);
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(String.Format("Could not delete assembly {0}: {1}", assemblyPath, exception));
            }
            return Assembly.Load(assemblyBytes);
        }

        public static DeploymentConfig GetDeploymentConfigFromAssembly(Assembly assembly)
        {
            var implementation = assembly.GetTypes().Where(t => 
                typeof(DeploymentConfig).IsAssignableFrom(t)).FirstOrDefault();
            var constructor = implementation.GetConstructor(Type.EmptyTypes);
            if (constructor == null)
            {
                Console.Error.WriteLine("Could not find empty constructor for IDeploymentConfig implementation in config file.");
            }
            return (DeploymentConfig)constructor.Invoke(null);
        }

        /// <summary>
        /// Creates an instance of all types in the assembly marked with a <see cref="DeploymentAttribute"/>.
        /// Type must have either a default constructor or 
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="contextInjection"></param>
        /// <returns></returns>
        public static IEnumerable<object> LoadTaskInstancesFromAssembly(Assembly assembly, IDeploymentContext contextInjection)
        {
            var objects = new List<object>();
            foreach (var type in assembly.GetTypes().Where(
                t => t.GetCustomAttribute(typeof(DeploymentAttribute)) != null))
            {
                ConstructorInfo constructor = null;
                if ((constructor = type.GetConstructor(Type.EmptyTypes))!= null)
                {
                    objects.Add(constructor.Invoke(null));
                }
                else if ((constructor = type.GetConstructor(new Type[] { typeof(IDeploymentContext) })) != null)
                {
                    objects.Add(constructor.Invoke(new object[] { contextInjection }));
                }
                else
                {
                    throw new ArgumentException(String.Format(
                        "Type {0} must have either a default constructor or take an IDeploymentContext as its sole argument.",
                        type.Name));
                }
            }
            return objects;
        }
    }
}
