using Hangfire.Server;

namespace Hangfire.PowerShellExecutor
{
    /// <summary>
    /// Used to hook into the Hangfire lifetime events and store the context.
    /// </summary>
    public interface IPerformingContextAccessor
    {
        PerformingContext Get();
    }

    /// <summary>
    /// Used to hook into the Hangfire lifetime events and store the context.
    /// </summary>
    public class PerformingContextAccessor : IPerformingContextAccessor
    {
        public PerformingContext Get()
        {
            return HangfireSubscriber.Value;
        }
    }
}