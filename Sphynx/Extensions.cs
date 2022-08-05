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
        public static IServiceCollection ConfigureDefaultSphynx(this IServiceCollection services, Action<DefaultSphynxOptionsBuilder> configureBuilder)
        {
            services.ThrowIfDefault(nameof(services));
            configureBuilder.ThrowIfDefault(nameof(configureBuilder));

            var builder = new DefaultSphynxOptionsBuilder();
            configureBuilder.Invoke(builder);
            return services.AddSingleton<ISphynxOptionsBuilder<DefaultSphynxOptions>>(builder);
        }
        public static IServiceCollection ConfigureSphynx<TOptions>(this IServiceCollection services, Func<ISphynxOptionsBuilder<TOptions>> builderFactory)
        {
            services.ThrowIfDefault(nameof(services));
            builderFactory.ThrowIfDefault(nameof(builderFactory));

            var builder = builderFactory.Invoke();
            return services.AddSingleton(builder);
        }
    }
}
