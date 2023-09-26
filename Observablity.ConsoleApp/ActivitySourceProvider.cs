using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observablity.ConsoleApp
{
    internal static class ActivitySourceProvider
    {
        public static ActivitySource source = new ActivitySource(OpenTelemetryConstants.ActivitySourceName);
    }
}
