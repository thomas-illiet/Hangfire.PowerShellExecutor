using Hangfire.Console;
using Hangfire.Server;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Hangfire.PowerShellExecutor
{
    public class PSExecutor
    {
        private readonly PSExecutorBuilder _builder;
        private readonly IPerformingContextAccessor _performingContextAccessor;
        private readonly PerformContext _performContext;

        /// <summary>
        /// Default constructor for dependency injection.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="performingContextAccessor"></param>
        public PSExecutor(PSExecutorBuilder builder, IPerformingContextAccessor performingContextAccessor)
        {
            _builder = builder;
            _performingContextAccessor = performingContextAccessor;
        }

        /// <summary>
        /// Manufacturer for manual use.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="performContext"></param>
        public PSExecutor(PSExecutorBuilder builder, PerformContext performContext)
        {
            _builder = builder;
            _performContext = performContext;
        }

        /// <summary>
        /// Starts (or reuses) the PowerShell process.
        /// </summary>
        /// <param name="cancellationToken"></param>
        public void Start(CancellationToken cancellationToken = default)
        {
            // Retrieve perform context
            var context = _performContext != null
                ? _performContext
                : _performingContextAccessor.Get();

            // Create the process
            var process = new Process();
            process.StartInfo.FileName = "PowerShell.exe";
            process.StartInfo.Arguments = _builder.Arguments;
            process.StartInfo.WorkingDirectory = _builder.WorkingDirectory;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            // Set the parameters to run the process.
            if (_builder.Parameters.Count > 0)
                AddParameters(process);

            // Set the credentials to run the process.
            if (_builder.Credential != null)
                SetCredential(process);

            // Configure output messages
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            process.StartInfo.StandardErrorEncoding = Encoding.UTF8;
            process.OutputDataReceived += (_, args) => WriteMessage(context, args.Data);
            process.ErrorDataReceived += (_, args) => WriteError(context, args.Data);

            // Kill the process if the cancellationToken is propagated
            if (cancellationToken != default(CancellationToken))
                cancellationToken.Register(() => process.Kill());

            // Start process and handlers
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            // Close the process
            process.Close();
        }

        /// <summary>
        /// Set the parameters to run the process.
        /// </summary>
        /// <param name="process"></param>
        private void AddParameters(Process process)
        {
            foreach (var parameter in _builder.Parameters)
            {
                process.StartInfo.Environment[parameter.Key] = parameter.Value;
            }
        }

        /// <summary>
        /// Set the credentials to run the process.
        /// </summary>
        /// <param name="process"></param>
        private void SetCredential(Process process)
        {
            if (_builder.Credential.Password == null)
                throw new ApplicationException("Password must not be empty");

            if (_builder.Credential.Domain != null)
                process.StartInfo.Domain = _builder.Credential.Domain;

            process.StartInfo.UserName = _builder.Credential.UserName;
            process.StartInfo.Password = _builder.Credential.Password;
        }

        /// <summary>
        /// Adds a message to console.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="message"></param>
        private void WriteMessage(PerformContext context, string message)
        {
            if (message != null)
            {
                _builder.CaptureOutput?.Invoke(message);
                context.WriteLine(RemoveSensitiveData(message));
            }
        }

        /// <summary>
        /// Adds a error to console.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="message"></param>
        private void WriteError(PerformContext context, string message)
        {
            if (message != null)
            {
                _builder.CaptureOutput?.Invoke(message);
                context.SetTextColor(ConsoleTextColor.Red);
                context.WriteLine(RemoveSensitiveData(message));
                context.ResetTextColor();
            }
        }

        /// <summary>
        /// Remove sensitive data from the message.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private string RemoveSensitiveData(string message)
        {
            foreach (var sensitiveData in _builder.SensitiveDatas)
                message = message.Replace(sensitiveData, new string('*', sensitiveData.Length));
            return message;
        }
    }
}