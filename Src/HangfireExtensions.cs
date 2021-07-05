using Hangfire.Server;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Hangfire.PowerShellExecutor
{
    public static partial class HangfireExtensions
    {
        public static IServiceCollection AddHangfirePSExecutorExtensions(this IServiceCollection services)
        {
            services.AddSingleton<IPerformingContextAccessor, PerformingContextAccessor>();
            services.AddTransient<PSExecutor>((ctx) => GetPSExecutor(ctx));
            services.AddTransient<PSExecutorBuilder>((ctx) => GetPSExecutorBuilder(ctx));
            services.AddTransient<IJobCancellationToken>(sp => sp.GetRequiredService<IPerformingContextAccessor>().Get().CancellationToken);
            services.AddTransient<PerformingContext>(sp => sp.GetRequiredService<IPerformingContextAccessor>().Get());

            GlobalJobFilters.Filters.Add(new HangfireSubscriber());

            return services;
        }

        private static PSExecutor GetPSExecutor(IServiceProvider ctx)
        {
            var builder = ctx.GetService<PSExecutorBuilder>();
            var contextAccesso = ctx.GetService<IPerformingContextAccessor>();
            return new PSExecutor(builder, contextAccesso);
        }

        private static PSExecutorBuilder GetPSExecutorBuilder(IServiceProvider ctx)
        {
            var contextAccesso = ctx.GetService<IPerformingContextAccessor>();
            return new PSExecutorBuilder(contextAccesso);
        }
    }
}