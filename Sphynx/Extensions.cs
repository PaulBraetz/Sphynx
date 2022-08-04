using Fort;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Sphynx.Middleware;

namespace Sphynx
{
    public static class Extensions
    {
        public static IApplicationBuilder UseDefaultSphynx(this IApplicationBuilder app)
        {
            app.ThrowIfDefault(nameof(app));

            return app.UseMiddleware<DefaultSphynx>();
        }
        public static IApplicationBuilder UseLoggingSphynx(this IApplicationBuilder app)
        {
            app.ThrowIfDefault(nameof(app));

            return app.UseMiddleware<LoggingSphynx>();
        }
        public static IServiceCollection ConfigureSphynx(this IServiceCollection services, Action<OptionsBuilder> configureBuilder)
        {
            services.ThrowIfDefault(nameof(services));
            configureBuilder.ThrowIfDefault(nameof(configureBuilder));

            var builder = new OptionsBuilder();
            configureBuilder.Invoke(builder);
            return services.AddSingleton<IOptionsBuilder>(builder);
        }
        public static IServiceCollection ConfigureSphynx(this IServiceCollection services, Func<IOptionsBuilder> builderFactory)
        {
            services.ThrowIfDefault(nameof(services));
            builderFactory.ThrowIfDefault(nameof(builderFactory));

            var builder = builderFactory.Invoke();
            return services.AddSingleton(builder);
        }
    }
}
