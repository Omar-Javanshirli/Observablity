using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Elasticsearch;


namespace Logging.Shared
{
    public  static class Logging
    {
        public static void AddOpenTelemtryLog(this WebApplicationBuilder builder)
        {
            builder.Logging.AddOpenTelemetry(cfg =>
            {
                var serviceName = builder.Configuration.GetSection("OpenTelemetry")["ServiceName"];
                var serviceVersion = builder.Configuration.GetSection("OpenTelemetry")["ServiceVersion"];

                cfg.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName!, serviceVersion: serviceVersion));
                cfg.AddOtlpExporter();
            });
        }

        public static Action<HostBuilderContext, LoggerConfiguration> ConfigureLogging => (builderContext, loggerConfiguration) =>
        {
            var environment = builderContext.HostingEnvironment;

            //FromLogContext ==> log atildigi zaman trace id (correlation id) vernek ucun merkezlesdirilmis trace id vermek ucun yaziriq.
            //WithExceptionDetails ==> eger ki bu exception atarsa o exceptionin detaylarini da yazsin.
            //WithProperty ==> her log atanda hardan geldiyi bildirilsin
            loggerConfiguration
            .ReadFrom.Configuration(builderContext.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .Enrich.WithProperty("Env", environment.EnvironmentName)
            .Enrich.WithProperty("AppName", environment.ApplicationName);

            var ElasticsearchBaseUrl = builderContext.Configuration.GetSection("Elasticsearch")["BaseUrl"];
            var ElasticsearchUsername = builderContext.Configuration.GetSection("Elasticsearch")["Username"];
            var ElasticsearchPassword = builderContext.Configuration.GetSection("Elasticsearch")["Password"];
            var ElasticsearchIndexName = builderContext.Configuration.GetSection("Elasticsearch")["IndexName"];

            loggerConfiguration.WriteTo.Elasticsearch(new(new Uri(ElasticsearchBaseUrl!))
            {
                AutoRegisterTemplate = true,
                AutoRegisterTemplateVersion = Serilog.Sinks.Elasticsearch.AutoRegisterTemplateVersion.ESv8,
                IndexFormat = $"{ElasticsearchIndexName}-{environment.EnvironmentName}-logs-" + "{0:yyy.MM.dd}",
                ModifyConnectionSettings = x => x.BasicAuthentication(ElasticsearchUsername, ElasticsearchPassword),
                CustomDurableFormatter = new ElasticsearchJsonFormatter()
            }) ;

        };
    }
}
