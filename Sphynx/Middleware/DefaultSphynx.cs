using Fort;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Sphynx.Middleware
{
	public sealed class DefaultSphynx : LoggingSphynxBaseAsync
	{
		private sealed class RequestTracker
		{
			public RequestTracker(DefaultSphynxOptions options)
			{
				_capacity = options.InitialCapacity;
				_lastAccess = DateTimeOffset.UtcNow;
				_gate = new SemaphoreSlim(1, 1);
			}

			private readonly SemaphoreSlim _gate;
			private Int32 _capacity;
			private DateTimeOffset _lastAccess;

			public async Task<Boolean> Check(DefaultSphynxOptions options)
			{
				Boolean result = false;
				await _gate.WaitAsync();
				try
				{
					var deltaT = DateTimeOffset.UtcNow - _lastAccess;
					var recovered = (Int32)(deltaT / options.RecoveryTime);

					_lastAccess += recovered * options.RecoveryTime;

					_capacity += recovered;
					_capacity = Math.Clamp(_capacity, 0, options.InitialCapacity);
					_capacity -= 1;

					result = _capacity > -1;
				}
				finally
				{
					_gate.Release();
				}
				return result;
			}
		}

		public DefaultSphynx(RequestDelegate next, ILogger<DefaultSphynx> logger, ISphynxOptionsBuilder<DefaultSphynxOptions> optionsBuilder) : base(next, logger)
		{
			optionsBuilder.ThrowIfDefault(nameof(optionsBuilder));

			_options = optionsBuilder.Build();
		}

		private static readonly ConcurrentDictionary<String, RequestTracker> _trackers = new ConcurrentDictionary<String, RequestTracker>();

		private readonly DefaultSphynxOptions _options;

		protected override async Task<Boolean> MayPassAsync(HttpContext context)
		{
			context.ThrowIfDefault(nameof(context));

			var ip = context.Connection.RemoteIpAddress.GetAddressBytes();
			if (ip.Length > 0)
			{
				var tracker = GetTracker(ip);
				return await tracker.Check(_options);
			}
			return false;
		}

		private RequestTracker GetTracker(Byte[] ip)
		{
			ip.ThrowIfDefault(nameof(ip));

			var ipHash = System.Security.Cryptography.SHA256.HashData(ip);
			var ipHashString = Convert.ToBase64String(ipHash);
			return _trackers.GetOrAdd(ipHashString, new RequestTracker(_options));
		}

		protected override Task RejectAsync(HttpContext context)
		{
			context.ThrowIfDefault(nameof(context));

			updateContext();
			LogRejection(context);

			return Task.CompletedTask;

			void updateContext()
			{
				context.Response.StatusCode = 429;
				var retryAfter = (Int32)_options.RecoveryTime.TotalSeconds;
				if (retryAfter > 0)
				{
					context.Response.Headers.Add("Retry-After", retryAfter.ToString());
				}
			}
		}

		protected override void LogRejection(HttpContext context)
		{
			if (Logger.IsEnabled(LogLevel.Warning))
			{
				Logger.LogWarning($"Rejected {context.Connection.Id}. Options: {_options}");
			}
		}
	}
}