using Hangfire.Console;
using Hangfire.Server;
using System.Collections.Generic;

namespace Hangfire.PowerShellExecutor
{
    internal static class ConsoleHelpers
    {
        /// <summary>
        ///  Adds a string to the console without sensitive data.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="secrets"></param>
        /// <param name="value"></param>
        /// <param name="isError"></param>
        internal static void WriteSecureLine(this PerformContext context, List<string> secrets, string value, bool isError)
        {
            if (value != null)
            {
                if (isError)
                    context.SetTextColor(ConsoleTextColor.Red);
                context.WriteLine(RemoveSensitiveData(secrets, value));
                if (isError)
                    context.ResetTextColor();
            }
        }

        private static string RemoveSensitiveData(List<string> secrets, string value)
        {
            foreach (var secret in secrets)
            {
                value = value.Replace(secret, "*********");
            }
            return value;
        }
    }
}