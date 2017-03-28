using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace ZFreeGo.Common.LogTrace
{
    public class CLog
    {
        public static void LogCritical(string strMessage)
        {
            _ts.TraceEvent(TraceEventType.Critical,
                           0,
                           "   [{0}{1}]: {2}", 
                           DateTime.Now.ToLongDateString(), 
                           DateTime.Now.ToLongTimeString(), 
                           strMessage);       
        }

        public static void LogError(string strMessage)
        {
            _ts.TraceEvent(TraceEventType.Error,
                           0,
                           "      [{0}{1}]: {2}",
                           DateTime.Now.ToLongDateString(),
                           DateTime.Now.ToLongTimeString(),
                           strMessage);            
        }

        public static void LogWarning(string strMessage)
        {
            _ts.TraceEvent(TraceEventType.Warning,
                           0,
                           "    [{0}{1}]: {2}",
                           DateTime.Now.ToLongDateString(),
                           DateTime.Now.ToLongTimeString(),
                           strMessage);            
        }

        public static void LogInformation(string strMessage)
        {
            _ts.TraceEvent(TraceEventType.Information,
                            0,
                           "[{0}{1}]: {2}",
                           DateTime.Now.ToLongDateString(),
                           DateTime.Now.ToLongTimeString(),
                           strMessage);             
        }       

        private static TraceSource _ts = new TraceSource("LogTrace");
    }
}
