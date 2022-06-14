# <img align="center" style="height:80px;margin-right:10px" src="https://raw.githubusercontent.com/thomas-illiet/Hangfire.PowerShellExecutor/main/Assets/Icon.png"/> PowerShellExecutor

[![NuGet](https://img.shields.io/nuget/vpre/Hangfire.PowerShellExecutor)](https://www.nuget.org/packages/Hangfire.PowerShellExecutor/)
[![NuGet](https://img.shields.io/nuget/dt/Hangfire.PowerShellExecutor)](https://www.nuget.org/packages/Hangfire.PowerShellExecutor/)
[![Tests](https://github.com/thomas-illiet/Hangfire.PowerShellExecutor/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/thomas-illiet/Hangfire.PowerShellExecutor/actions/workflows/build-and-test.yml)
[![CodeQL](https://github.com/thomas-illiet/Hangfire.PowerShellExecutor/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/thomas-illiet/Hangfire.PowerShellExecutor/actions/workflows/codeql-analysis.yml)
![MIT License](https://img.shields.io/badge/license-MIT-orange.svg)

A plugin for Hangfire that allows you to easily launch your powershell scripts.

![dashboard](Assets/Dashboard.png)

*In this screenshot i'm using [MissionControl](https://github.com/ahydrax/Hangfire.MissionControl) to facilitate the creation of jobs.*

Read about hangfire here: <https://github.com/HangfireIO/Hangfire> here: <http://hangfire.io>

## Please show the value

Choosing a project dependency could be difficult. We need to ensure stability and maintainability of our projects.
Surveys show that GitHub stars count play an important factor when assessing library quality.

⭐ Please give this repository a star. It takes seconds and help thousands of developers! ⭐

## Support development

It doesn't matter if you are a professional developer, creating a startup or work for an established company.
All of us care about our tools and dependencies, about stability and security, about time and money we can safe, about quality we can offer.
Please consider sponsoring to give me an extra motivational push to develop the next great feature.

> If you represent a company, want to help the entire community and show that you care, please consider sponsoring using one of the higher tiers.
Your company logo will be shown here for all developers, building a strong positive relation.

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
        config.UseMemoryStorage();
        config.UseConsole();
    });
    services.AddPSExecutorExtensions();
}
```

Otherwise,

```c#
GlobalConfiguration.Configuration
    .UseMemoryStorage()
    .UseConsole();
services.AddPSExecutorExtensions();
```

**NOTE**: If you have Dashboard and Server running separately, you'll need to call `UseConsole()` on both.

## Example

```C#
using Hangfire.MissionControl;
using Hangfire.Server;
using System.Threading;

namespace Hangfire.PowerShellExecutor.Test
{
    [MissionLauncher(CategoryName = "TestSuite")]
    public class TestSuite
    {
        private readonly PSExecutorBuilder _executorBuilder;

        public TestSuite (PSExecutorBuilder executorBuilder)
        {
            _executorBuilder = executorBuilder;
        }

        public void WithInjection(PerformContext context, CancellationToken cancellationToken)
        {
            var localProcess = _executorBuilder
                .SetCommand("Get-ChildItem")
                .SetExecutionPolicy(PSExecutionPolicy.Bypass)
                .Build();
            localProcess.Start(cancellationToken);
        }

        public void WithoutInjection(PerformContext context, CancellationToken cancellationToken)
        {
            var localProcess = new PSExecutorBuilder(context)
                .SetCommand("Get-ChildItem")
                .SetExecutionPolicy(PSExecutionPolicy.Bypass)
                .Build();
            localProcess.Start(cancellationToken);
        }

        public void WithParameter(PerformContext context, CancellationToken cancellationToken)
        {
            var localProcess = new PSExecutorBuilder(context)
                .AddParameter("PE_TEST01", "TEST01")
                .AddParameter("PE_TEST02", "TEST02")
                .SetCommand("Get-ChildItem -Path Env:")
                .SetExecutionPolicy(PSExecutionPolicy.Bypass)
                .Build();
            localProcess.Start(cancellationToken);
        }

        public void WithSecret(PerformContext context, CancellationToken cancellationToken)
        {
            var localProcess = new PSExecutorBuilder(context)
                .AddSecret("PE_TEST01", "8SE2vsPH$AZrGHq4u*t")
                .AddSecret("PE_TEST02", "xr^!!DjA7C**7YyH%AF")
                .SetCommand("Get-ChildItem -Path Env:")
                .SetExecutionPolicy(PSExecutionPolicy.Bypass)
                .Build();
            localProcess.Start(cancellationToken);
        }

        public void WithExternalFile(PerformContext context, CancellationToken cancellationToken)
        {
            var localProcess = new PSExecutorBuilder(context)
                .SetFile(@"D:\Scripts\FileScript.ps1")
                .SetExecutionPolicy(PSExecutionPolicy.Bypass)
                .Build();
            localProcess.Start(cancellationToken);
        }

        public void WithError(PerformContext context, CancellationToken cancellationToken)
        {
            var localProcess = new PSExecutorBuilder(context)
                .SetCommand("Write-Error 'Test''")
                .SetExecutionPolicy(PSExecutionPolicy.Bypass)
                .Build();
            localProcess.Start(cancellationToken);
        }
    }
}

```

## How to Contribute

Everyone is welcome to contribute to this project! Feel free to contribute with pull requests, bug reports or enhancement suggestions.

## Bugs and Feedback

For bugs, questions and discussions please use the [GitHub Issues](https://github.com/thomas-illiet/Hangfire.PowerShellExecutor/issues).

## License

This project is licensed under [MIT License](https://github.com/thomas-illiet/Hangfire.PowerShellExecutor/blob/main/LICENSE).
