﻿using System;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Web.Install.Models;

namespace Umbraco.Web.Install.InstallSteps
{
    /// <summary>
    /// This step is purely here to show the button to commence the upgrade
    /// </summary>
    [InstallSetupStep(InstallationType.Upgrade, "Upgrade", "upgrade", 1, "Upgrading Umbraco to the latest and greatest version.")]
    internal class UpgradeStep : InstallSetupStep<object>
    {
        public override bool RequiresExecution(object model) => true;
        private readonly IUmbracoVersion _umbracoVersion;
        private readonly IRuntimeState _runtimeState;

        public UpgradeStep(IUmbracoVersion umbracoVersion, IRuntimeState runtimeState)
        {
            _umbracoVersion = umbracoVersion;
            _runtimeState = runtimeState;
        }

        public override Task<InstallSetupResult> ExecuteAsync(object model) => Task.FromResult<InstallSetupResult>(null);

        public override object ViewModel
        {
            get
            {
                // TODO: if UmbracoVersion.Local is null?
                // it means that there is a database but the web.config version is cleared
                // that was a "normal" way to force the upgrader to execute, and we would detect the current
                // version via the DB like DatabaseSchemaResult.DetermineInstalledVersion - magic, do we really
                // need this now?
                var currentVersion = (_umbracoVersion.LocalVersion ?? new Semver.SemVersion(0)).ToString();

                var newVersion = _umbracoVersion.SemanticVersion.ToString();

                string FormatGuidState(string value)
                {
                    if (string.IsNullOrWhiteSpace(value)) value = "unknown";
                    else if (Guid.TryParse(value, out var currentStateGuid))
                        value = currentStateGuid.ToString("N").Substring(0, 8);
                    return value;
                }


                var currentState = FormatGuidState(_runtimeState.CurrentMigrationState);
                var newState = FormatGuidState(_runtimeState.FinalMigrationState);

                var reportUrl = $"https://our.umbraco.com/contribute/releases/compare?from={currentVersion}&to={newVersion}&notes=1";

                return new { currentVersion, newVersion, currentState, newState, reportUrl };
            }
        }
    }
}
