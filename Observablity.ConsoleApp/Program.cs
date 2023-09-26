// See https://aka.ms/new-console-template for more information
using Observablity.ConsoleApp;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

Console.WriteLine("Hello, World!");
Console.WriteLine();


var traceProvider = Sdk.CreateTracerProviderBuilder()
    .AddSource(OpenTelemetryConstants.ActivitySourceName)
    .ConfigureResource(configure =>
    {
        configure.AddService(OpenTelemetryConstants.ServiceName, serviceVersion: OpenTelemetryConstants.ServiceVersion)
          .AddAttributes(new List<KeyValuePair<string, object>>()
                {
                    new KeyValuePair<string, object>("host.machineName", Environment.MachineName),
                    new KeyValuePair<string, object>("host.environment", "dev"),
                });
    }).AddConsoleExporter().Build();



var serviceHelper = new ServiceHelper();
await serviceHelper.Work1();
