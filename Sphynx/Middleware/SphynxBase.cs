using Fort;
using Microsoft.AspNetCore.Http;

namespace Sphynx.Middleware
{
	public abstract class SphynxBase
	{
		public SphynxBase(RequestDelegate next)
		{
			next.ThrowIfDefault(nameof(next));

			_next = next;
		}

		private readonly RequestDelegate _next;

		public async Task InvokeAsync(HttpContext context)
		{
			context.ThrowIfDefault(nameof(context));

			if (MayPass(context))
			{
				await _next(context);
			}
			else
			{
				Reject(context);
			}
		}

		protected abstract Boolean MayPass(HttpContext context);
		protected abstract void Reject(HttpContext context);
	}
}