using System.Diagnostics;

namespace Observablity.ConsoleApp
{
    internal class ServiceHelper
    {
        internal async Task Work1()
        {
            using var activity = ActivitySourceProvider.source.StartActivity();
            var serviceOne = new ServiceOne();

            activity?.SetTag("work 1 tag", "work 1 tag value");
            activity?.AddEvent(new ActivityEvent("work 1 event"));

            Console.WriteLine($"google response length:{await serviceOne.MakeRequestToGoogle()}");
            Console.WriteLine("Work1 tamamlandı.");
        }

        internal async Task Work2()
        {
            using var activity = ActivitySourceProvider.SourceFile.StartActivity();
      
            activity?.SetTag("work 2 tag", "work 2 tag value");
            activity?.AddEvent(new ActivityEvent("work 2 event"));
        }
    }
}
