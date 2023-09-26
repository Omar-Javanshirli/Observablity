using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observablity.ConsoleApp
{
    internal class ServiceTwo
    {
        internal async Task<int> WriteToFile(string text)
        {
            using var activity = ActivitySourceProvider.source.StartActivity();
            await File.WriteAllTextAsync("myFile.txt", text);
            return (await File.ReadAllTextAsync("myFile.txt")).Length;
        }
    }
}
