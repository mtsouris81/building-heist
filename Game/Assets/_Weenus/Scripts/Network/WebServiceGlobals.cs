using System;

namespace Weenus.Network
{
    public static class WebServiceGlobals
    {
        public static Action<string, string> GlobalErrorCallback { get; set; }
    }
}
