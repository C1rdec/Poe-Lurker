//-----------------------------------------------------------------------
// <copyright file="MouseLurker.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker
{
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
        #region Fields

        private const int MsgCodeMouseLeftButtonUp = 0x0202; // WM_LBUTTONUP

        private const string HookHelperExeBaseName = "Lurker.HookHelper";
        private const int MouseHookMessageSizeInBytes = 20;

        private SocketMessageService _socketMessageService;
        private bool _disposed = false;
        private Mutex _hookHelperMutex;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when [mouse message received].
        /// </summary>
        public event EventHandler<MouseMessageReceivedEventArgs> MouseMessageReceived;

        /// <summary>
        /// Occurs when [mouse left button up].
        /// </summary>
        public event EventHandler MouseLeftButtonUp;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseLurker"/> class.
        /// </summary>
        /// <param name="process">The process.</param>
        public MouseLurker(Process process)
        {
            this._socketMessageService = new SocketMessageService(MouseHookMessageSizeInBytes);
            this._socketMessageService.SocketMessageReceived += SocketMessageService_SocketMessageReceived;
            this._socketMessageService.StartListening();

            var allowEveryoneRule = new MutexAccessRule(
                new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                MutexRights.FullControl,
                AccessControlType.Allow);
            var securitySettings = new MutexSecurity();
            securitySettings.AddAccessRule(allowEveryoneRule);
            var hookHelperMutexGuid = Guid.NewGuid().ToString();
            var hookHelperMutex = $"Global\\{hookHelperMutexGuid}";
            this._hookHelperMutex = new Mutex(true, hookHelperMutex, out bool createdNew, securitySettings);

            var is64Process = process.ProcessName.Contains("x64") || process.ProcessName.Contains("X64");
            var hookHelperExtension = (is64Process ? ".x64" : ".x86") + ".exe";
            var hookHelperExeName = $"{HookHelperExeBaseName}{hookHelperExtension}";
            var hookHelperExePath = Path.Combine(GetExecutingAssemblyDirectory(), hookHelperExeName);

            // Lurker.HookLib.*.dll and Lurker.HookHelper.*.exe are located in Lurker.UI's folder
            Process.Start(hookHelperExePath, $"{this._socketMessageService.Port} {process.Id} {hookHelperMutexGuid}");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose() => this.Dispose(true);

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    try
                    {                        
                        this._hookHelperMutex.ReleaseMutex(); // Let exe unhook and terminate
                        this._hookHelperMutex.Dispose();
                        this._socketMessageService.Dispose();
                    }
                    catch
                    {
                    }
                }

                this._disposed = true;
            }
        }

        /// <summary>
        /// Ges the executing assembly directory.
        /// </summary>
        /// <returns></returns>
        private string GetExecutingAssemblyDirectory()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);

            return Path.GetDirectoryName(path);
        }

        /// <summary>
        /// Handles the SocketMessageReceived event of the SocketMessageService control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SocketMessageReceivedEventArgs"/> instance containing the event data.</param>
        private void SocketMessageService_SocketMessageReceived(object sender, SocketMessageReceivedEventArgs e)
        {
            var messageCode = BitConverter.ToInt32(e.MessageBytes, 0);
            if (messageCode == MsgCodeMouseLeftButtonUp)
            {
                this.MouseLeftButtonUp?.Invoke(this, EventArgs.Empty);
            }

            Debug.WriteLine($"Mouse Message Code: {messageCode}; X: {BitConverter.ToInt32(e.MessageBytes, 4)}; Y: {BitConverter.ToInt32(e.MessageBytes, 8)}");

            MouseMessageReceived?.Invoke(this, new MouseMessageReceivedEventArgs
            {
                MessageCode = messageCode,
                X = BitConverter.ToInt32(e.MessageBytes, 4),
                Y = BitConverter.ToInt32(e.MessageBytes, 8),
                Handle = BitConverter.ToInt32(e.MessageBytes, 12),
                HitTestCode = BitConverter.ToInt32(e.MessageBytes, 16),
            });
        }

        #endregion
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
