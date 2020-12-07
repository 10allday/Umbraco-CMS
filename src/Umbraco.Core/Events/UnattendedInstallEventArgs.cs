using System;

namespace Umbraco.Core.Events
{
    public class UnattendedInstallEventArgs : EventArgs
    {
        public UnattendedInstallEventArgs(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public bool Success { get; }
        public string Message { get; }
    }
}
