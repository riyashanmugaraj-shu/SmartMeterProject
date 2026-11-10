using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client
{
    public class ConsoleErrorObserver : ICommunicationErrorObserver
    {
        public void OnCommunicationError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine(
                $"[COMMUNICATION ERROR] {message}");

            Console.ResetColor();
        }
    }
}
