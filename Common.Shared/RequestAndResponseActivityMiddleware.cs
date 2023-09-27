using Microsoft.AspNetCore.Http;
using Microsoft.IO;
using System.Diagnostics;

namespace Common.Shared
{
    public class RequestAndResponseActivityMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RecyclableMemoryStreamManager _memoryStreamManager;

        public RequestAndResponseActivityMiddleware(RequestDelegate next)
        {
            _next = next;
            _memoryStreamManager = new RecyclableMemoryStreamManager();
        }


        public async Task InvokeAsync(HttpContext context)
        {
            await AddRequestBodyContentToActivityTag(context);
            await AddResponseBodyContentToActivityTag(context);
        }

        private async Task AddRequestBodyContentToActivityTag(HttpContext context)
        {
            //memoryi effectic istifade ede bilmek ucun. cunki reqeust birden cox oxuna bilir basqa middlewarelar terefinden
            context.Request.EnableBuffering();

            var requestBodyStreamReader = new StreamReader(context.Request.Body);
            var requestBodyContent = await requestBodyStreamReader.ReadToEndAsync();

            Activity.Current?.SetTag("http.request.body", requestBodyContent);
            context.Request.Body.Position = 0;
        }


        private async Task AddResponseBodyContentToActivityTag(HttpContext context)
        {
            //middlewarelerde Next methodunnan evvel request icra olur. Nexten sonrada middlwarelere geder daha sonra 
            // qayidar Nexten sonraki hisse isleyer.

            //orginal response elde edirik 
            var orginalResponse = context.Response.Body;

            //Recyclable librarysi uzerinde bir stream acirix. burda hele middlewarelere requestler gelir.
            await using var responseBodyMemoryStream = _memoryStreamManager.GetStream();

            //daha sonra responseun bodysine elave edirk. yuxarida orginal response-in body hissesin saxlamisiq.
            //burada body-e bos bir dene stream veririk. cunki hele next methodu islemir. Next methodu isleyenden sonra
            //body dolacag. Memoryde ikiside eyni yeri isare edir (context.Response.Body ve responseBodyMemoryStream).
            context.Response.Body = responseBodyMemoryStream;

            //next methodu ile beraber artiq response elimizde var
            await _next(context);

            responseBodyMemoryStream.Position = 0;

            //response oxuyuruq.
            var responseBodyStreamReader = new StreamReader(responseBodyMemoryStream);
            var responseBodyContent = await responseBodyStreamReader.ReadToEndAsync();
            Activity.Current?.SetTag("http.response.body", responseBodyContent);

            responseBodyMemoryStream.Position = 0;

            //elde olunun response orginal response-a copyaliyirix. orginalResponse context.Response.Body-nin oldugu yeri isare edir.
            //yeni aftomatik olrak data  context.Response.Body-nin ic'risine gelecek.
            await responseBodyMemoryStream.CopyToAsync(orginalResponse);
        }
    }
}
