﻿using Blossom.Deployment.Logging;
using System.Collections.Generic;

namespace Blossom.Deployment
{
    public interface IDeploymentContext
    {
        ILogger Logger { get; set; }

        Env Environment { get; set; }

        IOperations Operations { get; }

        void BeginDeployment(string[] args, object deploymentInstance);

        void BeginDeployment(string[] args, IEnumerable<object> deploymentInstances);
    }
}