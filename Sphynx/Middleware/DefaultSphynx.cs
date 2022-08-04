using Fort;
using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;

namespace Sphynx.Middleware
{
    public class DefaultSphynx
    {
        private sealed class RequestTracker
        {
            public RequestTracker(Options options)
            {
                _remaining = options.Capacity;
                _lastAccess = DateTimeOffset.UtcNow;
                _gate = new SemaphoreSlim(1, 1);
            }

            private readonly SemaphoreSlim _gate;
            private Int32 _remaining;
            private DateTimeOffset _lastAccess;

            public async Task<Boolean> Check(Options options)
            {
                Boolean result = false;
                await _gate.WaitAsync();
                try
                {
                    var deltaT = DateTimeOffset.UtcNow - _lastAccess;
                    var recovered = (Int32)(deltaT / options.RecoveryTime);

                    _lastAccess += recovered * options.RecoveryTime;

                    _remaining += recovered;
                    _remaining = Math.Clamp(_remaining, 0, options.Capacity);
                    _remaining -= 1;

                    result = _remaining > -1;
                }
                finally
                {
                    _gate.Release();
                }
                return result;
            }
        }

        public DefaultSphynx(RequestDelegate next, IOptionsBuilder optionsBuilder)
        {
            next.ThrowIfDefault(nameof(next));
            optionsBuilder.ThrowIfDefault(nameof(optionsBuilder));

            _next = next;
            Options = optionsBuilder.BuildOptions();
        }

        private static readonly ConcurrentDictionary<String, RequestTracker> _trackers = new ConcurrentDictionary<String, RequestTracker>();

        protected Options Options { get; }
        private readonly RequestDelegate _next;

        public async Task InvokeAsync(HttpContext context)
        {
            context.ThrowIfDefault(nameof(context));

            var ip = context.Connection.RemoteIpAddress.GetAddressBytes();
            var ipHash = System.Security.Cryptography.SHA256.HashData(ip);
            var ipHashString = Convert.ToBase64String(ipHash);
            if (ip != null && await _trackers.GetOrAdd(ipHashString, new RequestTracker(Options)).Check(Options))
            {
                await _next(context);
            }
            else
            {
                await Reject(context);
            }
        }

        protected virtual Task Reject(HttpContext context)
        {
            context.ThrowIfDefault(nameof(context));

            context.Response.StatusCode = 429;
            var retryAfter = (Int32)Options.RecoveryTime.TotalSeconds;
            if (retryAfter > 0)
            {
                context.Response.Headers.Add("Retry-After", retryAfter.ToString());
            }
            return Task.CompletedTask;
        }
    }
}