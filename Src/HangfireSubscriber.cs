using Hangfire.Server;
using System.Threading;

namespace Hangfire.PowerShellExecutor
{
    /// <summary>
    /// Singelton used to keep track of hangfire jobs
    /// </summary>
    internal class HangfireSubscriber : IServerFilter
    {
        private static readonly AsyncLocal<PerformingContext> localStorage = new AsyncLocal<PerformingContext>();

        public static PerformingContext Value => localStorage.Value;

        public void OnPerforming(PerformingContext filterContext)
        {
            localStorage.Value = filterContext;
        }

        public void OnPerformed(PerformedContext filterContext)
        {
            localStorage.Value = null;
        }
    }
}