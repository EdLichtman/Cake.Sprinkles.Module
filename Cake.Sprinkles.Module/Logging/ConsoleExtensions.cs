using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;

namespace Cake.Sprinkles.Module.Logging
{
    internal static class ConsoleExtensions
    {
        public static void WriteLine(this IConsole console, string message, ConsoleColor foregroundColor)
        {
            console.ResetColor();
            console.ForegroundColor = foregroundColor;
            console.WriteLine(message);
            console.ResetColor();
        }
    }
}
