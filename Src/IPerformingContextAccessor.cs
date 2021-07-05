using Hangfire.Server;

namespace Hangfire.PowerShellExecutor
{
    public interface IPerformingContextAccessor
    {
        PerformingContext Get();
    }

    public class PerformingContextAccessor : IPerformingContextAccessor
    {
        public PerformingContext Get()
        {
            return HangfireSubscriber.Value;
        }
    }
}