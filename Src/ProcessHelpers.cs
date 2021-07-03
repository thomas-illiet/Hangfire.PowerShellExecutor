using Hangfire.Server;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Hangfire.PowerShellExecutor
{
    public static class ProcessHelpers
    {
        /// <summary>
        /// Extension to run the powershell script.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="config"></param>
        /// <param name="cancellationToken"></param>
        public static void StartProcess(this PerformContext context, PsExecutorConfig config, CancellationToken cancellationToken = default)
        {
            try
            {
                // Create the process
                var process = new Process();
                process.StartInfo.FileName = "PowerShell.exe";
                process.StartInfo.Arguments = config.Arguments;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                process.StartInfo.StandardErrorEncoding = Encoding.UTF8;

                // Add parameters to the process
                foreach (var parameter in config.Parameters)
                    process.StartInfo.Environment[parameter.Key] = parameter.Value;

                // Set output and error ( asynchronous ) handlers
                process.OutputDataReceived += new DataReceivedEventHandler((s, e) => context.WriteSecureLine(config.Secrets, e.Data, false));
                process.ErrorDataReceived += new DataReceivedEventHandler((s, e) => context.WriteSecureLine(config.Secrets, e.Data, true));

                // Add cancellation support
                cancellationToken.Register(() => process.Kill());

                // Start process and handlers
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(string.Format("Error when attempting to execute {0}, {1]", config.ScriptPath, ex.Message), ex);
            }
        }
    }
}