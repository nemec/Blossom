using System;
using Blossom.Deployment;
using System.Collections.Generic;

public class CustomDeploymentConfig : DeploymentConfig
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


    public bool DryRun { get; set; }
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