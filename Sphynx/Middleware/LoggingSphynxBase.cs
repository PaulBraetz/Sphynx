using Fort;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Sphynx.Middleware
{
	public abstract class LoggingSphynxBase : SphynxBase
    {
        public LoggingSphynxBase(RequestDelegate next, ILogger logger) : base(next)
        {
            logger.ThrowIfDefault(nameof(logger));

            Logger = logger;
        }
        protected ILogger Logger { get; }

		protected override void Reject(HttpContext context)
		{
            context.ThrowIfDefault(nameof(context));

            LogRejection(context);
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