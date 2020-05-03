//-----------------------------------------------------------------------
// <copyright file="ClipboardLurker.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------


namespace Lurker
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Gma.System.MouseKeyHook;
    using Lurker.Helpers;
    using Lurker.Patreon.Events;
    using Lurker.Patreon.Parsers;
    using Lurker.Services;
    using WindowsInput;
    using WK.Libraries.SharpClipboardNS;

    public class ClipboardLurker: IDisposable
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
        /// Initializes a new instance of the <see cref="ClipboardLurker"/> class.
        /// </summary>
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
            this.LurkForAction();
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
                this._clipboardMonitor.ClipboardChanged -= ClipboardMonitor_ClipboardChanged;
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
        /// Starts the watcher.
        /// </summary>
        private void LurkForAction()
        {
            var search = Combination.FromString("Control+F");
            var remainingMonster = Combination.FromString("Control+R");
            var deleteItem = Combination.TriggeredBy(System.Windows.Forms.Keys.Delete);

            this._keyboardEvent = Hook.GlobalEvents();
            var assignment = new Dictionary<Combination, Action>
            {
                {search, this.Search},
                {remainingMonster, this.RemainingMonster},
                {deleteItem, this.DeleteItem},
            };

            this._keyboardEvent.OnCombination(assignment);
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

        /// <summary>
        /// Gets the item base type in clipboard.
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetItemSearchValueInClipboard()
        {
            try
            {
                this._simulator.Keyboard.ModifiedKeyStroke(WindowsInput.Native.VirtualKeyCode.CONTROL, WindowsInput.Native.VirtualKeyCode.VK_C);
                await Task.Delay(20);
                var text = ClipboardHelper.GetClipboardText();

                try
                {
                    return this._itemParser.GetSearchValue(text);
                }
                catch (System.InvalidOperationException)
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        private async void Search()
        {
            if (this._settingsService.ItemHighlightEnabled && Models.PoeApplicationContext.InForeground)
            {
                var baseType = await this.GetItemSearchValueInClipboard();
                if (string.IsNullOrEmpty(baseType))
                {
                    return;
                }

                this._keyboardHelper.Write(baseType);
            }
        }

        /// <summary>
        /// Remainings the monster.
        /// </summary>
        private void RemainingMonster()
        {
            if (this._settingsService.RemainingMonsterEnabled && Models.PoeApplicationContext.InForeground)
            {
                this._keyboardHelper.RemainingMonster();
            }
        }

        /// <summary>
        /// Deletes the item.
        /// </summary>
        private void DeleteItem()
        {
            if (this._settingsService.DeleteItemEnabled && Models.PoeApplicationContext.InForeground)
            {
                this._simulator.Mouse.LeftButtonClick();
                this._keyboardHelper.Destroy();
            }
        }

        #endregion
    }
}
