using System.Diagnostics;

namespace Observablity.ConsoleApp
{
    internal class ServiceHelper
    {
        internal async Task Work1()
        {
            using var activity = ActivitySourceProvider.source.StartActivity();
            var serviceOne = new ServiceOne();
          
            Console.WriteLine($"google response length:{await serviceOne.MakeRequestToGoogle()}");
            Console.WriteLine("Work1 tamamlandı.");
        }
    }
}
