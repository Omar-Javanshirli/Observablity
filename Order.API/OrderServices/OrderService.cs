using OpenTelemetry.Shared;
using System.Diagnostics;

namespace Order.API.OrderServices
{
    public class OrderService
    {
        public Task CreateAsync(OrderCreateRequestDto request)
        {
            Activity.Current?.SetTag("Asp .Net Core (Instrumentation) tag 1", "Asp .Net Core (Instrumentation) tag value");

            using var activity = ActivitySourceProvider.Source.StartActivity();
            activity?.AddEvent(new("Sifaris prosesi basladi"));

            //database save etdik

            activity?.SetTag("order user id: ", request.UserId);
            activity?.AddEvent(new("Sifaris prosesi bitdi"));

            return Task.CompletedTask;
        }
    }
}
