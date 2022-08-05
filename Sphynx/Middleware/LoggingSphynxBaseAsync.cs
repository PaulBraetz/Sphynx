using Fort;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Sphynx.Middleware
{
	public abstract class LoggingSphynxBaseAsync : SphynxBaseAsync
    {
        public LoggingSphynxBaseAsync(RequestDelegate next, ILogger logger) : base(next)
        {
            logger.ThrowIfDefault(nameof(logger));

            Logger = logger;
        }
        protected ILogger Logger { get; }

        protected override Task RejectAsync(HttpContext context)
        {
            context.ThrowIfDefault(nameof(context));

            LogRejection(context);

            return Task.CompletedTask;
        }

        protected virtual void LogRejection(HttpContext context)
        {
            context.ThrowIfDefault(nameof(context));

            if (Logger.IsEnabled(LogLevel.Warning))
            {
                Logger.LogWarning($"Rejected {context.Connection.Id}.");
            }
        }
    }
}