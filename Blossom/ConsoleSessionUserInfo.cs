using System;
using Tamir.SharpSsh.jsch;

namespace Blossom.Deployment
{
    internal class ConsoleSessionUserInfo : UserInfo
    {
        private IDeploymentContext Context { get; set; }

        public string Password { get; set; }

        public string Passphrase { get; set; }

        public AutoResponse AutoRespondYN { get; set; }

        public ConsoleSessionUserInfo(IDeploymentContext context)
        {
            AutoRespondYN = AutoResponse.AlwaysAsk;
        }

        public string getPassphrase()
        {
            return Passphrase;
        }

        public string getPassword()
        {
            return Password;
        }

        public bool promptPassphrase(string message)
        {
            if (Passphrase == null)
            {
                Console.Write(message);
                Passphrase = Console.ReadLine().TrimEnd('\n');
            }
            return true;
        }

        public bool promptPassword(string message)
        {
            if (Password == null)
            {
                Console.Write(message + " ");
                if (Context.Environment.InteractionType == InteractionType.NonInteractive)
                {
                    throw new NonInteractiveSessionException(
                        "Task asked for prompt during non-interactive session.");
                }
                Password = Console.ReadLine().TrimEnd('\n');
            }
            return true;
        }

        public bool promptYesNo(string message)
        {
            Console.Write(message + " ");
            switch (AutoRespondYN)
            {
                case AutoResponse.Yes:
                    Console.WriteLine("[AutoResponse: Y]");
                    return true;
                case AutoResponse.No:
                    Console.WriteLine("[AutoResponse: N]");
                    return false;
                default:
                    if (Context.Environment.InteractionType == InteractionType.NonInteractive)
                    {
                        throw new NonInteractiveSessionException(
                            "Task asked for prompt during non-interactive session.");
                    }
                    return Console.ReadLine().ToLower().StartsWith("y");
            }
        }

        public void showMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}