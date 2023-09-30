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
                        //Controller "api" olanlarin trace etmese ucun. eks halda program ayaga qalxan oz datalarinida trace edir. 
                        if (!string.IsNullOrEmpty(context.Request.Path.Value))
                            return context.Request.Path.Value.Contains("api", StringComparison.InvariantCulture);

                        return false;
                    });

                    // RecordException true oldugu zaman program bir exaception atarsa exception save ediler.
                    //Eger sen exceptionlri loglamada gormey isdeyirsense false edib Serilogun vasitesi ile exceptionlari tuta bilersen.
                    aspNetCoreOptions.RecordException = true;

                    // Serilog üzerinden elasticsearch db'ye hatalar gönderildiği için kapatıldı.Eger xetalari Trace data da saxlamaq
                    //isd'yirikse asagida ki kodu yazmaq lazimdi. Ama loglar daha cox NoSql database de saxlandigina gore
                    //loglari elasticsearch db de ve ya basqa NoSql database saxlamaq daha duzgun yoldur.
                    //aspNetCoreOptions.EnrichWithException = (activity, exception) =>
                    //{
                    //    // Bilerek boş bırakıldı. Örnek göstermek için. elave datlar yazmaq ucun istifade olunur.
                    //    //bir Action delegatdir. eger ki exceptionnin yaninda basqa datalarda gostermek isdeyirikse 
                    //    //bu delegatin vasitesi ile edirik. Datani zengilesdirmek ucun istifade olunur.
                    //};
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
                    httpOptions.FilterHttpRequestMessage = (request) =>
                    {
                        //pathin sonun da "9200" var ise  ornek "http://localhost:9200" buna aid trace data istehsal etmesin.
                        return !request.RequestUri!.AbsolutePath.Contains("9200", StringComparison.InvariantCulture);
                    };

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

