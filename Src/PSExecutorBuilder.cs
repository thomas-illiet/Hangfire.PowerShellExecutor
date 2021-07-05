using Hangfire.PowerShellExecutor.Models;
using Hangfire.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;

namespace Hangfire.PowerShellExecutor
{
    public class PSExecutorBuilder
    {
        public PSExecutionPolicy ExecutionPolicy { get; private set; }
        public List<string> SensitiveDatas { get; private set; }
        public PSCredential Credential { get; private set; }
        public Dictionary<string, string> Parameters { get; private set; }
        public PSExecutionType ExecutionType { get; private set; }
        public string ExecutionData { get; private set; }
        public string Arguments { get; private set; }
        public string WorkingDirectory { get; private set; }
        public Action<string> CaptureOutput { get; private set; }

        private readonly IPerformingContextAccessor _performingContextAccessor;
        private readonly PerformContext _performContext;

        public PSExecutorBuilder(IPerformingContextAccessor performingContextAccessor)
        {
            _performingContextAccessor = performingContextAccessor;
            SensitiveDatas = new List<string>();
            Parameters = new Dictionary<string, string>();
        }

        public PSExecutorBuilder(PerformContext performContext)
        {
            _performContext = performContext;
            SensitiveDatas = new List<string>();
            Parameters = new Dictionary<string, string>();
        }

        public PSExecutorBuilder AddParameters(Dictionary<string, string> parameters)
        {
            foreach (var parameter in parameters)
            {
                if (Parameters.ContainsKey(parameter.Key))
                    throw new DuplicateNameException($"Configuration already has a parameter with the following name: {parameter.Key}");
                Parameters.Add(parameter.Key, parameter.Value);
            }
            return this;
        }

        public PSExecutorBuilder AddParameter(string name, string value)
        {
            if (Parameters.ContainsKey(name))
                throw new DuplicateNameException($"Configuration already has a parameter with the following name: {name}");
            Parameters.Add(name, value);
            return this;
        }

        public PSExecutorBuilder AddSecret(string name, string value)
        {
            if (Parameters.ContainsKey(name))
                throw new DuplicateNameException($"Configuration already has a parameter with the following name: {name}");
            Parameters.Add(name, value);
            if (!SensitiveDatas.Contains(value))
                SensitiveDatas.Add(value);
            return this;
        }

        public PSExecutorBuilder SetWorkingDirectory(string workingDirectory)
        {
            WorkingDirectory = workingDirectory;
            return this;
        }

        public PSExecutorBuilder SetCaptureOutput(Action<string> captureOutput)
        {
            CaptureOutput = captureOutput;
            return this;
        }

        public PSExecutorBuilder SetExecutionPolicy(PSExecutionPolicy executionPolicy)
        {
            ExecutionPolicy = executionPolicy;
            return this;
        }

        public PSExecutorBuilder SetFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Could not find following file: {filePath}");
            return SetExecutionData(PSExecutionType.File, filePath);
        }

        public PSExecutorBuilder SetCommand(string command)
        {
            var commandExpression = new Regex(@"^& {.*}$");
            if (!commandExpression.IsMatch(command))
                command = string.Format("& {{ {0} }}", command);
            return SetExecutionData(PSExecutionType.Command, command);
        }

        private PSExecutorBuilder SetExecutionData(PSExecutionType type, string data)
        {
            ExecutionType = type;
            ExecutionData = data;
            return this;
        }

        /// <summary>
        /// Generate the command-line arguments to use when starting the application.
        /// </summary>
        private void GenerateArguments()
        {
            if (ExecutionType == PSExecutionType.File)
                Arguments = string.Format("-NoLogo -NoProfile -NonInteractive -ExecutionPolicy {0} -File {1}", ExecutionPolicy, ExecutionData);
            else if (ExecutionType == PSExecutionType.Command)
                Arguments = string.Format("-NoLogo -NoProfile -NonInteractive -ExecutionPolicy {0} -Command \"{1}\"", ExecutionPolicy, ExecutionData);
        }

        public PSExecutor Build()
        {
            GenerateArguments();
            if (_performContext != null)
                return new PSExecutor(this, _performContext);
            else
                return new PSExecutor(this, _performingContextAccessor);
        }
    }
}