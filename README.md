# <img align="center" style="height:80px;margin-right:10px" src="https://raw.githubusercontent.com/thomas-illiet/Hangfire.PowerShellExecutor/main/Icon.png"/> PowerShellExecutor

[![NuGet](https://img.shields.io/nuget/vpre/Hangfire.PowerShellExecutor)](https://www.nuget.org/packages/Hangfire.PowerShellExecutor/)
[![NuGet](https://img.shields.io/nuget/dt/Hangfire.PowerShellExecutor)](https://www.nuget.org/packages/Hangfire.PowerShellExecutor/)
[![CodeQL](https://github.com/thomas-illiet/Hangfire.PowerShellExecutor/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/thomas-illiet/Hangfire.PowerShellExecutor/actions/workflows/codeql-analysis.yml)
![MIT License](https://img.shields.io/badge/license-MIT-orange.svg)

A plugin for Hangfire that allows you to easily launch your powershell scripts.

Read about hangfire here: <https://github.com/HangfireIO/Hangfire> here: <http://hangfire.io>

## Features

* **Noob Friendly**: Allow to easily launch your powershell scripts.
* **100% Safe**: no Hangfire-managed data (e.g. jobs, states) is ever updated, hence there's no risk to corrupt it.
* *(blah-blah-blah)*

## Setup

Install NuGet package, and configure the [console](https://github.com/pieceofsummer/Hangfire.Console) in .NET Core's Startup.cs:

```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddHangfire(config =>
    {
        config.UseSqlServerStorage("connectionSting");
        config.UseConsole();
    });
}
```

Otherwise,

```c#
GlobalConfiguration.Configuration
    .UseSqlServerStorage("connectionSting")
    .UseConsole();
```

**NOTE**: If you have Dashboard and Server running separately, 
you'll need to call `UseConsole()` on both.

## Example

```C#
using Hangfire.Console;
using Hangfire.Server;
using System.Threading;
using System.Threading.Tasks;

namespace Hangfire.PowerShellExecutor.Jobs
{
    public class PowerShellJob
    {
        public async Task ExecuteAsync(PerformContext context, CancellationToken cancellationToken)
        {
            context.SetTextColor(ConsoleTextColor.Green);
            context.WriteLine("*****************************************************************");
            context.WriteLine("Starting: Hangfire Script");
            context.WriteLine("*****************************************************************");
            context.ResetTextColor();

            // Create an instance of the configuration
            var psConfig = new PsExecutorConfig();
            // Define the execution policy
            psConfig.SetExecutionPolicy(ExecutionPolicy.Bypass);
            // Add parameters
            psConfig.AddSecret("PE_SUPERSECRET", "P@ssw0rd");
            psConfig.AddParameter("PE_PARAMETER01", "parameter01");
            psConfig.AddParameter("PE_PARAMETER02", "parameter02");
            // Set the location of the powershell script
            psConfig.SetScriptPath(@"D:\Scripts\Get-ChildItem.ps1");
            // Execute the powershell script
            context.StartProcess(psConfig, cancellationToken);

            context.WriteLine("");
            context.SetTextColor(ConsoleTextColor.Green);
            context.WriteLine("*****************************************************************");
            context.WriteLine("Finishing: Hangfire Script");
            context.WriteLine("*****************************************************************");
        }
    }
}
```

## License

Authored by: Thomas ILLIET

This project is under MIT license. You can obtain the license copy [here](https://github.com/thomas-illiet/Hangfire.PowerShellExecutor/blob/master/LICENSE).
