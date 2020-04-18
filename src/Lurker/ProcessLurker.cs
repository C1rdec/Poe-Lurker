//-----------------------------------------------------------------------
// <copyright file="ProcessLurker.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class ProcessLurker : IDisposable
    {
        #region Fields

        private IEnumerable<string> _processNames;
        private CancellationTokenSource _tokenSource;
        private static readonly int WaitingTime = 5000;
        private Process _activeProcess;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessLurker"/> class.
        /// </summary>
        /// <param name="processName">Name of the process.</param>
        public ProcessLurker(string processName)
            : this(new string[] { processName })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessLurker"/> class.
        /// </summary>
        /// <param name="processNames">The process names.</param>
        public ProcessLurker(IEnumerable<string> processNames)
        {
            this._processNames = processNames;
            this._tokenSource = new CancellationTokenSource();
        }

        #endregion

        #region Events

        /// <summary>
        /// The poe ended
        /// </summary>
        public event EventHandler ProcessClosed;

        #endregion

        #region Methods

        /// <summary>
        /// Waits for process.
        /// </summary>
        /// <returns></returns>
        public async Task<Process> WaitForProcess()
        {
            var process = this.GetProcess();

            while (process == null)
            {
                await Task.Delay(WaitingTime);
                process = this.GetProcess();
            }

            this.WaitForExit();
            return this.WaitForWindowHandle();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._tokenSource.Cancel();

                if (this._activeProcess != null)
                {
                    this._activeProcess.Dispose();
                }
            }
        }

        /// <summary>
        /// Gets the process.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Path of Exile is not running</exception>
        private Process GetProcess()
        {
            if (this._activeProcess != null)
            {
                this._activeProcess.Dispose();
            }

            foreach (var processName in this._processNames)
            {
                var process = Process.GetProcessesByName(processName).FirstOrDefault();
                if (process != null)
                {
                    this._activeProcess = process;
                    return process;
                }
            }

            return null;
        }

        /// <summary>
        /// Waits for exit.
        /// </summary>
        private async void WaitForExit()
        {
            await Task.Run(() =>
            {
                try
                {
                    var token = this._tokenSource.Token;
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    var process = this.GetProcess();
                    while (process != null)
                    {
                        process.WaitForExit(WaitingTime);
                        process = this.GetProcess();
                    }
                }
                catch { }
                finally
                {
                    this.ProcessClosed?.Invoke(this, EventArgs.Empty);
                }
            });
        }

        /// <summary>
        /// Gets the window handle.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        private Process WaitForWindowHandle()
        {
            Process currentProcess;

            try
            {
                do
                {
                    var process = this.GetProcess();
                    Thread.Sleep(200);
                    if (process == null)
                    {
                        throw new System.InvalidOperationException();
                    }

                    currentProcess = process;
                }
                while (currentProcess.MainWindowHandle == IntPtr.Zero);
            }
            catch
            {
                currentProcess = this.WaitForWindowHandle();
            }

            return currentProcess;
        }

        #endregion
    }
}
