using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tamir.SharpSsh.jsch;

namespace Blossom.Deployment
{
    internal class ConsoleSessionUserInfo : UserInfo
    {
        private string Password { get; set; }
        private string Passphrase { get; set; }
        private AutoResponse AutoRespondYN { get; set; }

        public ConsoleSessionUserInfo(
            string password = null,
            string passphrase = null,
            AutoResponse autoRespondYN = AutoResponse.AlwaysAsk)
        {
            Password = password;
            Passphrase = passphrase;
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
                Console.Write(message);
                Password = Console.ReadLine().TrimEnd('\n');
            }
            return true;
        }

        public bool promptYesNo(string message)
        {
            switch (AutoRespondYN)
            {
                case AutoResponse.Yes:
                    return true;
                case AutoResponse.No:
                    return false;
                default:
                    Console.Write(message);
                    return Console.ReadLine().ToLower().StartsWith("y");
            }
        }

        public void showMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}
