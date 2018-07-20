using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lib.Log
{
    public static class NLoggingServicesExtensions
    {
        public static IServiceCollection AddNLogging(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerFactory, NLoggerFactory>();
            services.AddSingleton<ILogger, NLogger>();
            return services;
        }
    }
}