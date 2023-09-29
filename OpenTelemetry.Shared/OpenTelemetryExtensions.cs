using MassTransit.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;

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
                .AddSource(DiagnosticHeaders.DefaultListenerName)
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

                    aspNetCoreOptions.EnrichWithException = (activity, exception) =>
                    {
                        // Bilerek boş bırakıldı. Örnek göstermek için. elave datlar yazmaq ucun istifade olunur.
                        //bir Action delegatdir. eger ki exceptionnin yaninda basqa datalarda gostermek isdeyirikse 
                        //bu delegatin vasitesi ile edirik. Datani zengilesdirmek ucun istifade olunur.
                    };

                });
                options.AddConsoleExporter();
                options.AddOtlpExporter(); //Jaeger
                options.AddEntityFrameworkCoreInstrumentation(efCoreOptions =>
                {
                    efCoreOptions.SetDbStatementForText = true;
                    efCoreOptions.SetDbStatementForStoredProcedure = true;
                    efCoreOptions.EnrichWithIDbCommand = (activity, dbCommand) =>
                    {
                        //bilerek bos buraxildi numune ucun
                    };
                });
                options.AddHttpClientInstrumentation(httpOptions =>
                {
                    // burada yazilan kodlar bizim programimizda basqa programlara gondermis oldugumuz sorgulari Trace edir.
                    // yeni A serviceden B service sorgu atdigimiz zaman requestin body hissesini ve gonderdiyimiz sorgudan 
                    // ne netice elde etdik onu gore gilirik yeni response-un body hissesin.
                    httpOptions.EnrichWithHttpRequestMessage = async (activity, request) =>
                    {
                        var requestContent = "empty";

                        if (request.Content != null)
                            requestContent = await request.Content.ReadAsStringAsync();

                        activity?.SetTag("http.request.body", requestContent);
                    };

                    httpOptions.EnrichWithHttpResponseMessage = async (activity, response) =>
                    {
                        if (response.Content != null)
                            activity.SetTag("http.response.body", await response.Content.ReadAsStringAsync());
                    };
                });
                options.AddRedisInstrumentation(redisOptions =>
                {
                    //database ile elaqeli statmentleri detalli sekilde save et.
                    redisOptions.SetVerboseDatabaseStatements = true;
                });
            });
        }
    }
}

