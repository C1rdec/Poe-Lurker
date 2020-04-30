//-----------------------------------------------------------------------
// <copyright file="MouseLurker.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker
{
    using System;
    using System.Diagnostics;

    public class MouseLurker : IDisposable
    {
        #region Fields

        private MouseHookService _mouseHookService;
        private bool _disposed = false;

        #endregion

        #region Events

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
            this._mouseHookService = new MouseHookService(process);
            this._mouseHookService.MouseLeftButtonUp += MouseHookService_MouseLeftButtonUp;
        }

        private void MouseHookService_MouseLeftButtonUp(object sender, EventArgs e)
        {
            this.MouseLeftButtonUp?.Invoke(this, EventArgs.Empty);
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
                        this._mouseHookService.Dispose();
                    }
                    catch
                    {
                    }
                }

                this._disposed = true;
            }
        }

        #endregion
    }
}
