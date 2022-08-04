using Fort;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Sphynx.Middleware
{
    public class LoggingSphynx : DefaultSphynx
    {
        public LoggingSphynx(RequestDelegate next, IOptionsBuilder optionsBuilder, ILogger<LoggingSphynx> logger) : base(next, optionsBuilder)
        {
            logger.ThrowIfDefault(nameof(logger));

            Logger = logger;
        }
        protected ILogger Logger { get; }
        protected override Task Reject(HttpContext context)
        {
            context.ThrowIfDefault(nameof(context));

            if (Logger.IsEnabled(LogLevel.Warning))
            {
                Logger.LogWarning($"Rejected {context.Connection.RemoteIpAddress}. Options: {Options}");
            }
            return base.Reject(context);
        }
    }
}