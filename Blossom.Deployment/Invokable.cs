using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Blossom.Deployment
{
    public sealed class Invokable : IEquatable<Invokable>
    {
        public object Base { get; set; }
        public MethodInfo Method { get; set; }

        public void Invoke(IDeploymentContext context)
        {
            Method.Invoke(Base, new[] { context });
        }

        public bool Equals(Invokable other)
        {
            if (other == null)
            {
                return false;
            }
            return this.Method == other.Method;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Invokable);
        }

        public override int GetHashCode()
        {
            return Method.GetHashCode();
        }
    }
}
