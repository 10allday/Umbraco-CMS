using System;
using Umbraco.Core.Composing;
using Umbraco.Core.Events;
using Umbraco.Core.Exceptions;
using Umbraco.Core.Logging;
using Umbraco.Core.Migrations.Install;

namespace Umbraco.Core.Compose
{
    public class UnattendedInstallComponent : IComponent
    {
        private readonly IRuntimeState _runtimeState;
        private readonly ILogger _logger;
        private readonly DatabaseBuilder _databaseBuilder;

        public UnattendedInstallComponent(IRuntimeState runtimeState, ILogger logger, DatabaseBuilder databaseBuilder)
        {
            _runtimeState = runtimeState ?? throw new ArgumentNullException(nameof(runtimeState));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _databaseBuilder = databaseBuilder ?? throw new ArgumentNullException(nameof(databaseBuilder));
        }

        public static event EventHandler<UnattendedInstallEventArgs> InstallCompleted;

        public void Initialize()
        {
            // check if we are doing an unattended install
            if (_runtimeState.Reason != RuntimeLevelReason.InstallEmptyDatabase) return;

            _logger.Info<UnattendedInstallComponent>("Installing Umbraco.");
            var result = _databaseBuilder.CreateSchemaAndData();
            _logger.Info<UnattendedInstallComponent>("Umbraco Installed.");

            if(InstallCompleted != null)
                InstallCompleted.Invoke(this, new UnattendedInstallEventArgs(result.Success, result.Message));

            if (result.Success == false)
                throw new UnattendedInstallException(result.Message);
        }

        public void Terminate()
        {
        }
    }
}
