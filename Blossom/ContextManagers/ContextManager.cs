using System;

namespace Blossom.ContextManagers
{
    /// <summary>
    /// Implementation of Python's Context Managers ('with' statement)
    /// in C#, taking advantage of C#'s own 'using' statement for
    /// IDisposible to setup and teardown the context.
    /// </summary>
    /// <see cref="http://www.python.org/dev/peps/pep-0343/"/>
    public abstract class ContextManager : IDisposable
    {
        private bool _disposed;

        protected readonly IDeploymentContext Context;

        /// <summary>
        /// A function that manages "exiting" the context.
        /// Used to clean up resources and undo the Enter()
        /// method.
        /// </summary>
        protected abstract void Exit();

        /// <summary>
        /// Create a ContextManager with the current
        /// DeploymentContext setup and teardown should apply
        /// to the given context only.
        /// </summary>
        /// <param name="context"></param>
        protected ContextManager(IDeploymentContext context)
        {
            Context = context;
        }

        ~ContextManager()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposeManaged)
        {
            if (!_disposed)
            {
                Exit();
                _disposed = true;
            }
        }
    }
}