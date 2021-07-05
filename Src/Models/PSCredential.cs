using System.Net;
using System.Security;

namespace Hangfire.PowerShellExecutor.Models
{
    public class PSCredential
    {
        public string Domain { get; private set; }
        public string UserName { get; private set; }
        public SecureString Password { get; private set; }

        public PSCredential(string userName, string password)
        {
            UserName = userName;
            Password = new NetworkCredential("", password).SecurePassword;
        }

        public PSCredential(string domain, string userName, string password)
        {
            Domain = domain;
            UserName = userName;
            Password = new NetworkCredential("", password).SecurePassword;
        }
    }
}