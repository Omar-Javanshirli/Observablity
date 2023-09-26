// See https://aka.ms/new-console-template for more information
using Observablity.ConsoleApp;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;

Console.WriteLine("Hello, World!");
Console.WriteLine();


ActivitySource.AddActivityListener(new ActivityListener()
{
    ShouldListenTo = source => source.Name == OpenTelemetryConstants.ActivitySourceFileName,
    ActivityStarted= activity =>
    {
        Console.WriteLine("Activity basladi");
    },
    ActivityStopped= activity =>
    { 
        Console.WriteLine("Activity bitdi"); 
    }
});


using var traceProviderFile = Sdk.CreateTracerProviderBuilder()
    .AddSource(OpenTelemetryConstants.ActivitySourceFileName).Build();


using var traceProvider = Sdk.CreateTracerProviderBuilder()
    .AddSource(OpenTelemetryConstants.ActivitySourceName)
    .ConfigureResource(configure =>
    {
        configure.AddService(OpenTelemetryConstants.ServiceName, serviceVersion: OpenTelemetryConstants.ServiceVersion)
          .AddAttributes(new List<KeyValuePair<string, object>>()
                {
                    new KeyValuePair<string, object>("host.machineName", Environment.MachineName),
                    new KeyValuePair<string, object>("host.environment", "dev"),
                });
    }).AddConsoleExporter().AddOtlpExporter().Build();



var serviceHelper = new ServiceHelper();
await serviceHelper.Work1();
