using Fort;
using Microsoft.AspNetCore.Http;

namespace Sphynx.Middleware
{
	public abstract class SphynxBaseAsync
	{
		public SphynxBaseAsync(RequestDelegate next)
		{
			next.ThrowIfDefault(nameof(next));

			_next = next;
		}

		private readonly RequestDelegate _next;

		public async Task InvokeAsync(HttpContext context)
		{
			context.ThrowIfDefault(nameof(context));

			if (await MayPassAsync(context))
			{
				await _next(context);
			}
			else
			{
				await RejectAsync(context);
			}
		}

		protected abstract Task<Boolean> MayPassAsync(HttpContext context);
		protected abstract Task RejectAsync(HttpContext context);
	}
}