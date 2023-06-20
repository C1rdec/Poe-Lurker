//-----------------------------------------------------------------------
// <copyright file="ClipboardLurker.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker
{
    using System;
    using System.Windows.Input;
    using Lurker.Helpers;
    using PoeLurker.Patreon.Events;
    using WK.Libraries.SharpClipboardNS;

    /// <summary>
    /// The clipboard lurker.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class ClipboardLurker : IDisposable
    {
        #region Fields

        private SharpClipboard _clipboardMonitor;
        private string _lastClipboardText = string.Empty;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ClipboardLurker" /> class.
        /// </summary>
        public ClipboardLurker()
        {
            ClipboardHelper.ClearClipboard();
            this._clipboardMonitor = new SharpClipboard();

            this._clipboardMonitor.ClipboardChanged += this.ClipboardMonitor_ClipboardChanged;
        }

        #endregion

        #region Events

        /// <summary>
        /// Creates new offer.
        /// </summary>
        public event EventHandler<string> NewOffer;

        #endregion

        #region Methods

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
                this._clipboardMonitor.ClipboardChanged -= this.ClipboardMonitor_ClipboardChanged;
            }
        }

        /// <summary>
        /// Handles the ClipboardChanged event of the ClipboardMonitor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SharpClipboard.ClipboardChangedEventArgs"/> instance containing the event data.</param>
        private void ClipboardMonitor_ClipboardChanged(object sender, SharpClipboard.ClipboardChangedEventArgs e)
        {
            if (e.ContentType != SharpClipboard.ContentTypes.Text)
            {
                return;
            }

            var currentText = e.Content as string;
            if (string.IsNullOrEmpty(currentText) ||
                this._lastClipboardText == currentText ||
                Keyboard.IsKeyDown(Key.LeftShift))
            {
                return;
            }

            var isTradeMessage = false;
            if (TradeEvent.IsTradeMessage(currentText))
            {
                isTradeMessage = true;
            }
            else if (TradeEventHelper.IsTradeMessage(currentText))
            {
                isTradeMessage = true;
            }

            if (isTradeMessage)
            {
                this._lastClipboardText = currentText;
                this.NewOffer?.Invoke(this, currentText);
            }
        }

        #endregion
    }
}