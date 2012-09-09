using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blossom.Deployment.ContextManagers
{
    /// <summary>
    /// Implementation of Python's Context Managers ('with' statement)
    /// in C#, taking advantage of C#'s own 'using' statement for
    /// IDisposible to setup and teardown the context.
    /// </summary>
    /// <see cref="http://www.python.org/dev/peps/pep-0343/"/>
    public abstract class ContextManager : IDisposable
    {
        private bool _entered;

        protected readonly DeploymentContext Context;

        /// <summary>
        /// A function that manages "entering" the context.
        /// This is the set up for the context.
        /// </summary>
        protected abstract void Enter();

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
        public ContextManager(DeploymentContext context)
        {
            Context = context;
            _entered = false;
        }

        ~ContextManager()
        {
            Dispose(false);
        }

        protected void Begin()
        {
            _entered = true;
            Enter();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposeManaged)
        {
            if (_entered)
            {
                Exit();
            }
        }
    }
}
