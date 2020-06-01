//-----------------------------------------------------------------------
// <copyright file="ClipboardLurker.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using Gma.System.MouseKeyHook;
    using Lurker.Helpers;
    using Lurker.Patreon.Events;
    using Lurker.Patreon.Models;
    using Lurker.Patreon.Parsers;
    using Lurker.Services;
    using WindowsInput;
    using WK.Libraries.SharpClipboardNS;

    /// <summary>
    /// The clipboard lurker.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class ClipboardLurker : IDisposable
    {
        #region Fields

        private InputSimulator _simulator;
        private PoeKeyboardHelper _keyboardHelper;
        private ItemParser _itemParser = new ItemParser();
        private SettingsService _settingsService;
        private SharpClipboard _clipboardMonitor;
        private string _lastClipboardText = string.Empty;
        private IKeyboardMouseEvents _keyboardEvent;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ClipboardLurker" /> class.
        /// </summary>
        /// <param name="settingsService">The settings service.</param>
        /// <param name="keyboardHelper">The keyboard helper.</param>
        public ClipboardLurker(SettingsService settingsService, PoeKeyboardHelper keyboardHelper)
        {
            ClipboardHelper.ClearClipboard();
            this._keyboardHelper = keyboardHelper;
            this._simulator = new InputSimulator();
            this._clipboardMonitor = new SharpClipboard();
            this._settingsService = settingsService;

            this._settingsService.OnSave += this.SettingsService_OnSave;
            this._clipboardMonitor.ClipboardChanged += this.ClipboardMonitor_ClipboardChanged;
            this._itemParser.CheckPledgeStatus();
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
                this._keyboardEvent.Dispose();
                this._clipboardMonitor.ClipboardChanged -= this.ClipboardMonitor_ClipboardChanged;
                this._settingsService.OnSave -= this.SettingsService_OnSave;
            }
        }

        /// <summary>
        /// Handles the OnSave event of the SettingsService control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private async void SettingsService_OnSave(object sender, EventArgs e)
        {
            await this._itemParser.CheckPledgeStatus();
        }

        /// <summary>
        /// Handles the ClipboardChanged event of the ClipboardMonitor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SharpClipboard.ClipboardChangedEventArgs"/> instance containing the event data.</param>
        private void ClipboardMonitor_ClipboardChanged(object sender, SharpClipboard.ClipboardChangedEventArgs e)
        {
            if (e.ContentType == SharpClipboard.ContentTypes.Text)
            {
                var currentText = e.Content as string;
                if (string.IsNullOrEmpty(currentText))
                {
                    return;
                }

                if (this._lastClipboardText == currentText)
                {
                    return;
                }

                if (Keyboard.IsKeyDown(Key.LeftShift))
                {
                    return;
                }

                var isTradeMessage = false;
                this._lastClipboardText = currentText;
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
                    this.NewOffer?.Invoke(this, currentText);
                }
            }
        }

        #endregion
    }
}