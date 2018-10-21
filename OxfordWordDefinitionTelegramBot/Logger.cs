using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.Logging;

namespace OxfordWordDefinitionTelegramBot
{
    internal static class Logger
    {
        internal static ILogger logger;

        internal enum Method
        {
            Start,
            End,
            Info,
            Error
        }

        internal static void LogInformation(string className, string text, Method option)
        {
            switch (option)
            {
                case Method.Start: logger.LogInformation($"START: class: {className} > {text}"); break;
                case Method.End: logger.LogInformation($"END:   class: {className} > {text}"); break;
                case Method.Info: logger.LogInformation($"INFO:  class: {className} > {text}"); break;
                case Method.Error: logger.LogInformation($"ERROR: class: {className} > {text}"); break;
            }
            
        }
    }
}
