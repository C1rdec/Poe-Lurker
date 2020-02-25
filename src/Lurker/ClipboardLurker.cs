//-----------------------------------------------------------------------
// <copyright file="ClipboardLurker.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------


namespace Lurker
{
    using Gma.System.MouseKeyHook;
    using Lurker.Events;
    using Lurker.Helpers;
    using Lurker.Models;
    using Lurker.Parser;
    using Lurker.Services;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;

    public class ClipboardLurker: IDisposable
    {
        #region Fields

        private static readonly IKeyboardMouseEvents GlobalHook = Hook.GlobalEvents();
        private const char CtrlD = '\u0004';
        private ItemParser _itemParser = new ItemParser();
        private SettingsService _settingsService;
        private PoeKeyboardHelper _keyboardHelper;
        private CancellationTokenSource _tokenSource;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ClipboardLurker"/> class.
        /// </summary>
        public ClipboardLurker(SettingsService settingsService, PoeKeyboardHelper keyboardHelper)
        {
            this._tokenSource = new CancellationTokenSource();
            this._keyboardHelper = keyboardHelper;
            this._settingsService = settingsService;
            GlobalHook.KeyPress += this.GlobalHookKeyPress;
            this.MonitorClipboard();
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a new item is in the clipboard.
        /// </summary>
        public event EventHandler<PoeItem> Newitem;

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
                GlobalHook.KeyPress -= this.GlobalHookKeyPress;
                this._tokenSource.Cancel();
            }
        }

        /// <summary>
        /// Gets the clipboard data.
        /// </summary>
        /// <returns>The clipboard text.</returns>
        private string GetClipboardText()
        {
            var clipboardText = string.Empty;
            Thread thread = new Thread(() => { clipboardText = Clipboard.GetText(); });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            return clipboardText;
        }

        /// <summary>
        /// Globals the hook key press.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="KeyPressEventArgs" /> instance containing the event data.</param>
        private void GlobalHookKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            switch(e.KeyChar)
            {
                case CtrlD:
                    if (this._settingsService.SearchEnabled)
                    {
                        System.Windows.Forms.SendKeys.SendWait("^C");
                        var item = this._itemParser.Parse(this.GetClipboardText());
                        if (item != null)
                        {
                            this.Newitem?.Invoke(this, item);
                            Clipboard.Clear();
                        }
                    }

                    break;
            }
        }

        /// <summary>
        /// Monitors the clipboard.
        /// </summary>
        private async void MonitorClipboard()
        {
            Clipboard.Clear();
            var token = this._tokenSource.Token;
            var lastText = string.Empty;
            while(true)
            {
                await Task.Delay(800);
                if (token.IsCancellationRequested)
                {
                    return;
                }

                var currentText = this.GetClipboardText();
                if (lastText == currentText)
                {
                    continue;
                }

                lastText = currentText;
                if (TradeEvent.IsTradeMessage(currentText))
                {
                    this._keyboardHelper.SendMessage(currentText);
                }
            }
        }

        #endregion
    }
}
