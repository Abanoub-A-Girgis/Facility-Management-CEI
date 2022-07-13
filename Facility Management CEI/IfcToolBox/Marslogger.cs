using System;
using System.Linq;
using System.Text;
using Xbim.Common;

namespace IfcToolbox.Core.Utilities
{
    public class Marslogger
    {
        public static void Mark(object info)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"[{DateTime.Now.ToString("HH:mm:ss")} MAK] {info} ");
            Console.ResetColor();
            Console.WriteLine();
        }

        public static void Action(string action, string className = null)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            if (className == null)
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")} ACT] {action}");
            else
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")} ACT] {action} >>> {className}");
            Console.ResetColor();
        }

        public static void Step(string action, bool finalStep = false)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")} STP] {action}");
            Console.ResetColor();
            if (finalStep)
            {
                while (Console.ReadKey().Key != ConsoleKey.Enter) { }
            }
        }
    }
}
