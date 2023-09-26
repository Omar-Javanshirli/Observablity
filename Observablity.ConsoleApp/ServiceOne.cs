using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observablity.ConsoleApp
{
    internal class ServiceOne
    {
        static HttpClient httpClient = new HttpClient();
        internal async Task<int> MakeRequestToGoogle()
        {
            using var activity = ActivitySourceProvider.source.StartActivity
                (kind: System.Diagnostics.ActivityKind.Producer, name: "CustomMakeRequestToGoogle");

            try
            {
                var eventTags = new ActivityTagsCollection();

                activity?.AddEvent(new("google'a istek basladi", tags: eventTags));
                activity?.AddTag("request.schema", "https");
                activity?.AddTag("request.method", "get");


                var result = await httpClient.GetAsync("https://www.google.com");
                var responseContent = await result.Content.ReadAsStringAsync();
                activity?.AddTag("response.lenght", responseContent.Length);

                eventTags.Add("google body lenght", responseContent.Length);
                activity?.AddEvent(new("google'a istek tamamlandi", tags: eventTags));

                var serviceTwo=new ServiceTwo();
                var fileLenght = serviceTwo.WriteToFile("Omar Javanshirli");

                return responseContent.Length;
            }
            catch (Exception ex)
            {

                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                return -1;
            }
        }
    }
}
