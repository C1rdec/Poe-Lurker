//-----------------------------------------------------------------------
// <copyright file="MouseLurker.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker
{
    using Lurker.Extensions;
    using Lurker.Services;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Security.AccessControl;
    using System.Security.Principal;
    using System.Threading;

    public class MouseLurker : IDisposable
    {
        private const string HookHelperExeBaseName = "Lurker.HookHelper";
        private const int MouseHookMessageSizeInBytes = 20;

        private SocketMessageService _socketMessageService;
        private bool _disposed = false;
        private Mutex _hookHelperMutex;

        public event EventHandler<MouseMessageReceivedEventArgs> MouseMessageReceived;

        public MouseLurker(Process process)
        {
            _socketMessageService = new SocketMessageService(MouseHookMessageSizeInBytes);
            _socketMessageService.SocketMessageReceived += SocketMessageService_SocketMessageReceived;
            _socketMessageService.StartListening();

            var allowEveryoneRule = new MutexAccessRule(
                new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                MutexRights.FullControl,
                AccessControlType.Allow);
            var securitySettings = new MutexSecurity();
            securitySettings.AddAccessRule(allowEveryoneRule);
            var hookHelperMutexGuid = Guid.NewGuid().ToString();
            var hookHelperMutex = $"Global\\{hookHelperMutexGuid}";
            _hookHelperMutex = new Mutex(true, hookHelperMutex, out bool createdNew, securitySettings);

            var is64Process = process.ProcessName.Contains("x64") || process.ProcessName.Contains("X64");
            var hookHelperExtension = (is64Process ? ".x64" : ".x86") + ".exe";
            var hookHelperExeName = $"{HookHelperExeBaseName}{hookHelperExtension}";
            var hookHelperExePath = Path.Combine(GeExecutingAssemblyDirectory(), hookHelperExeName);

            // Copy Lurker.HookLib.*.dll and Lurker.HookHelper.*.exe in Lurker.UI's folder
            Process.Start(hookHelperExePath, $"{_socketMessageService.Port} {process.Id} {hookHelperMutexGuid}");
        }

        private string GeExecutingAssemblyDirectory()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }

        private void SocketMessageService_SocketMessageReceived(object sender, SocketMessageReceivedEventArgs e)
        {
            MouseMessageReceived?.Invoke(this, new MouseMessageReceivedEventArgs
            {
                MessageCode = BitConverter.ToInt32(e.MessageBytes, 0),
                X = BitConverter.ToInt32(e.MessageBytes, 4),
                Y = BitConverter.ToInt32(e.MessageBytes, 8),
                Handle = BitConverter.ToInt32(e.MessageBytes, 12),
                HitTestCode = BitConverter.ToInt32(e.MessageBytes, 16),
            });
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Let exe unhook and terminate
                    _hookHelperMutex.ReleaseMutex();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }

    public class MouseMessageReceivedEventArgs : EventArgs
    {
        public int MessageCode;
        public int X;
        public int Y;
        public int Handle;
        public int HitTestCode;
    }
}
