using System;
using System.Collections.Generic;

namespace Hangfire.PowerShellExecutor
{
    public class PsExecutorConfig
    {
        /// <summary>
        /// List of parameters for the execution of the powershell script.
        /// </summary>
        public Dictionary<string, string> Parameters { get; private set; }

        /// <summary>
        /// List sensitive information to hide it during the process.
        /// </summary>
        public List<string> Secrets { get; private set; }

        /// <summary>
        /// Execution policy for the current session.
        /// </summary>
        public ExecutionPolicy ExecutionPolicy { get; private set; }

        /// <summary>
        /// Location of the PowerShell script.
        /// </summary>
        public string ScriptPath { get; private set; }

        /// <summary>
        /// Arguments to use when starting the application.
        /// </summary>
        public string Arguments { get; private set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PsExecutorConfig()
        {
            Parameters = new Dictionary<string, string>();
            Secrets = new List<string>();
            ExecutionPolicy = ExecutionPolicy.Default;
        }

        /// <summary>
        /// Add sensitive information to the parameters.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        public PsExecutorConfig AddSecret(string parameterName, string parameterValue)
        {
            if (Parameters.ContainsKey(parameterName))
                throw new ApplicationException($"Cannot add secret '{parameterName}' because it already exists");

            Parameters.Add(parameterName, parameterValue);
            if (!Secrets.Contains(parameterValue))
                Secrets.Add(parameterValue);

            return this;
        }

        /// <summary>
        /// Add a parameter.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        public PsExecutorConfig AddParameter(string parameterName, string parameterValue)
        {
            if (Parameters.ContainsKey(parameterName))
                throw new ApplicationException($"Cannot add parameter '{parameterName}' because it already exists");

            Parameters.Add(parameterName, parameterValue);

            return this;
        }

        /// <summary>
        /// Add a list of parameters.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        public PsExecutorConfig AddParameters(Dictionary<string, string> parameters)
        {
            foreach (var parameter in parameters)
            {
                if (Parameters.ContainsKey(parameter.Key))
                    throw new ApplicationException($"Cannot add parameter '{parameter.Key}' because it already exists");
                Parameters.Add(parameter.Key, parameter.Value);
            }
            return this;
        }

        /// <summary>
        /// Set the execution policy for the current session.
        /// </summary>
        /// <param name="executionPolicy"></param>
        /// <returns></returns>
        public PsExecutorConfig SetExecutionPolicy(ExecutionPolicy executionPolicy)
        {
            ExecutionPolicy = executionPolicy;
            UpdateArguments();

            return this;
        }

        /// <summary>
        /// Set the location of the PowerShell script.
        /// </summary>
        /// <param name="scriptPath"></param>
        /// <returns></returns>
        public PsExecutorConfig SetScriptPath(string scriptPath)
        {
            if (!System.IO.File.Exists(scriptPath))
                throw new ApplicationException($"Cannot find following script: '{scriptPath}'");

            ScriptPath = scriptPath;
            UpdateArguments();

            return this;
        }

        /// <summary>
        /// Update the arguments to use when starting the application.
        /// </summary>
        private void UpdateArguments()
        {
            Arguments = string.Format("-NoLogo -NoProfile -NonInteractive -ExecutionPolicy {0} -File {1}", ExecutionPolicy, ScriptPath);
        }
    }
}