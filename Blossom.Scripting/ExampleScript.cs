using System;
using Blossom.Deployment;
using System.Collections.Generic;

public class DeploymentConfig : IDeploymentConfig
{
    private List<Host> _hosts = new List<Host>
    {
        new Host
        {
            Hostname = "192.168.1.1",
            Username = "username",
            Password = "password"
        }
    };

    public List<Host> Hosts
    {
        get { return _hosts; }
        set { _hosts = value; }
    }
}

[Deployment]
public class Test
{
    [Task]
    public void Run(IDeploymentContext context)
    {
        context.Logger.Info(context.RemoteOps.RunCommand("ls"));
    }
}