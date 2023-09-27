using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace OpenTelemetry.Shared
{
    public static class OpenTelemetryExtensions
    {
        public static void AddOpenTelemetryExt
            (this IServiceCollection services, IConfiguration Configuration)
        {
            services.Configure<OpenTelemetryConstants>(Configuration.GetSection("OpenTelemetry"));
            var openTelemetryConstants = Configuration.GetSection("OpenTelemetry").Get<OpenTelemetryConstants>();

            ActivitySourceProvider.Source = new System.Diagnostics.ActivitySource(openTelemetryConstants!.ActivitySourceName);

            services.AddOpenTelemetry().WithTracing(options =>
            {
                options.AddSource(openTelemetryConstants!.ActivitySourceName)
                .ConfigureResource(resource =>
                {
                    resource.AddService(openTelemetryConstants.ServiceName, openTelemetryConstants.ServiceVersion);
                });

                options.AddAspNetCoreInstrumentation(aspNetCoreOptions =>
                {
                    aspNetCoreOptions.Filter = (context =>
                    {
                        if (!string.IsNullOrEmpty(context.Request.Path.Value))
                            return context.Request.Path.Value.Contains("api", StringComparison.InvariantCulture);

                        return false;
                    });

                    //errorlari daha detalli gostermek ucun
                    aspNetCoreOptions.RecordException = true;
                });
                options.AddConsoleExporter();
                options.AddOtlpExporter(); //Jaeger
            });
        }
    }
}

