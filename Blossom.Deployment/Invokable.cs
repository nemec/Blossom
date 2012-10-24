﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace Blossom.Deployment
{
    public sealed class Invokable : IEquatable<Invokable>
    {
        public object Base { get; set; }
        public MethodInfo Method { get; set; }

        public void Invoke(IDeploymentContext context)
        {
            Method.Invoke(Base, new object[] { context });
        }

        public bool Equals(Invokable other)
        {
            if (other == null)
            {
                return false;
            }
            return Method == other.Method;
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
