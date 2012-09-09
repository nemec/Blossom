using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blossom.Deployment
{
    public enum InteractionType
    {
        /// <summary>
        /// Defines a session as non-interactive.
        /// If a task causes the deployment to prompt
        /// for input (eg. from stdin) even if there is
        /// a default value, it will abort the
        /// deployment for that host.
        /// </summary>
        NonInteractive,

        /// <summary>
        /// Defines a session as fully interactive.
        /// All prompts will be displayed to the user and
        /// will block until information is entered.
        /// </summary>
        AskForInput,

        /// <summary>
        /// Defines a session as mostly non-interactive.
        /// Is there is a default value provided for the
        /// prompt, it will be automatically answered.
        /// Otherwise, deployment will be aborted for
        /// that host.
        /// </summary>
        UseDefaults
    }
}
