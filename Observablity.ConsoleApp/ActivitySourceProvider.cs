﻿using System.Diagnostics;

namespace Observablity.ConsoleApp
{
    internal static class ActivitySourceProvider
    {
        internal static ActivitySource source = new ActivitySource(OpenTelemetryConstants.ActivitySourceName);
    }
}
