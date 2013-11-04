Blossom
=======

A Fabric-like deployment framework built in C# and tailored toward deployment on Windows machines.

NOTES:
-----

* The `IConfig` instance is shared between every host (even deployments
  running in parallel). If you need to keep state around between tasks, store
  in the `IDeploymentTasks` class.