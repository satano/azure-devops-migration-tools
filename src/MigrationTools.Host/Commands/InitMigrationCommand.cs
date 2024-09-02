﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elmah.Io.Client;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MigrationTools._EngineV1.Configuration;
using MigrationTools.Endpoints.Infrastructure;
using MigrationTools.Host.Services;
using MigrationTools.Options;
using MigrationTools.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spectre.Console.Cli;

namespace MigrationTools.Host.Commands
{
    internal class InitMigrationCommand : CommandBase<InitMigrationCommandSettings>
    {

        private readonly ILogger _logger;

        public InitMigrationCommand(IHostApplicationLifetime appLifetime, IServiceProvider services, IDetectOnlineService detectOnlineService, IDetectVersionService2 detectVersionService, ILogger<CommandBase<InitMigrationCommandSettings>> logger, ITelemetryLogger telemetryLogger, IMigrationToolVersion migrationToolVersion, IConfiguration configuration, ActivitySource activitySource) : base(appLifetime, services, detectOnlineService, detectVersionService, logger, telemetryLogger, migrationToolVersion, configuration, activitySource)
        {
            _logger = logger;
        }

        internal override async Task<int> ExecuteInternalAsync(CommandContext context, InitMigrationCommandSettings settings)
        {
            int _exitCode;
            try
            {
                TelemetryLogger.TrackEvent(new EventTelemetry("InitCommand"));
                string configFile = settings.ConfigFile;
                if (string.IsNullOrEmpty(configFile))
                {
                    configFile = $"configuration-{settings.Options.ToString()}.json";
                }
                _logger.LogInformation("ConfigFile: {configFile}", configFile);
                if (File.Exists(configFile))
                {
                    if (settings.Overwrite)
                    {
                        File.Delete(configFile);
                    }
                    else
                    {
                        _logger.LogCritical($"The config file {configFile} already exists, pick a new name. Or Set --overwrite");
                        Environment.Exit(1);
                    }
                }
                if (!File.Exists(configFile))
                {
                    var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                    _logger.LogInformation("Populating config with {Options}", settings.Options.ToString());

                    OptionsConfiguration optionsBuilder = Services.GetService<OptionsConfiguration>();

                    switch (settings.Options)
                    {
                        case OptionsMode.Reference:
                            optionsBuilder.AddAllOptions();
                            break;
                        case OptionsMode.Basic:
                            optionsBuilder.AddOption("TfsWorkItemMigrationProcessor");
                            optionsBuilder.AddOption("FieldMappingTool");
                            optionsBuilder.AddOption("FieldLiteralMap");
                            optionsBuilder.AddOption("TfsTeamProjectEndpoint", "Source");
                            optionsBuilder.AddOption("TfsTeamProjectEndpoint", "Target");
                            break;
                        case OptionsMode.WorkItemTracking:
                            optionsBuilder.AddOption("TfsWorkItemMigrationProcessor");
                            optionsBuilder.AddOption("FieldMappingTool");
                            optionsBuilder.AddOption("FieldLiteralMap");
                            optionsBuilder.AddOption("TfsTeamProjectEndpoint", "Source");
                            optionsBuilder.AddOption("TfsTeamProjectEndpoint", "Target");
                            break;
                        case OptionsMode.PipelineProcessor:
                            optionsBuilder.AddOption("AzureDevOpsPipelineProcessor");
                            optionsBuilder.AddOption("AzureDevOpsEndpoint", "Source");
                            optionsBuilder.AddOption("AzureDevOpsEndpoint", "Target");
                            break;
                        default:
                            optionsBuilder.AddAllOptions();
                            break;
                    }

                    string json = optionsBuilder.Build();

                    File.WriteAllText(configFile, json);
                    _logger.LogInformation("New {configFile} file has been created", configFile);
                    _logger.LogInformation(json);

                }
                _exitCode = 0;
            }
            catch (Exception ex)
            {
                TelemetryLogger.TrackException(ex, null, null);
                _logger.LogError(ex, "Unhandled exception!");
                _exitCode = 1;
            }
            finally
            {
                // Stop the application once the work is done
                Lifetime.StopApplication();
            }
            return _exitCode;
        }
    }
}
