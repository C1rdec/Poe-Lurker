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
    using WK.Libraries.SharpClipboardNS;

    public class ClipboardLurker: IDisposable
    {
        #region Fields

        private static readonly IKeyboardMouseEvents GlobalHook = Hook.GlobalEvents();
        private const char CtrlD = '\u0004';
        private ItemParser _itemParser = new ItemParser();
        private SettingsService _settingsService;
        private PoeKeyboardHelper _keyboardHelper;
        private SharpClipboard _clipboardMonitor;
        private string _lastClipboardText = string.Empty;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ClipboardLurker"/> class.
        /// </summary>
        public ClipboardLurker(SettingsService settingsService, PoeKeyboardHelper keyboardHelper)
        {
            this._clipboardMonitor = new SharpClipboard();
            this._keyboardHelper = keyboardHelper;
            this._settingsService = settingsService;

            GlobalHook.KeyPress += this.GlobalHookKeyPress;
            this._clipboardMonitor.ClipboardChanged += ClipboardMonitor_ClipboardChanged;
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
                this._clipboardMonitor.ClipboardChanged -= ClipboardMonitor_ClipboardChanged;
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
        /// Handles the ClipboardChanged event of the ClipboardMonitor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SharpClipboard.ClipboardChangedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="NotImplementedException"></exception>
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

                this._lastClipboardText = currentText;
                if (TradeEvent.IsTradeMessage(currentText))
                {
                    this._keyboardHelper.SendMessage(currentText);
                }
            }
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

        #endregion
    }
}
