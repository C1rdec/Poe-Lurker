//-----------------------------------------------------------------------
// <copyright file="ClipboardLurker.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.Core;

using System;
using PoeLurker.Core.Helpers;
using PoeLurker.Core.Services;
using PoeLurker.Patreon.Events;
using WK.Libraries.SharpClipboardNS;

/// <summary>
/// The clipboard lurker.
/// </summary>
/// <seealso cref="System.IDisposable" />
public class ClipboardLurker : IDisposable
{
    #region Fields

    private readonly PoeKeyboardHelper _keyboardHelper;
    private readonly SettingsService _settingsService;
    private readonly SharpClipboard _clipboardMonitor;
    private string _lastClipboardText = string.Empty;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ClipboardLurker" /> class.
    /// </summary>
    public ClipboardLurker(PoeKeyboardHelper keyboardHelper, SettingsService settingsService)
    {
        ClipboardHelper.ClearClipboard();
        _clipboardMonitor = new SharpClipboard();
        _clipboardMonitor.ClipboardChanged += ClipboardMonitor_ClipboardChanged;
        _keyboardHelper = keyboardHelper;
        _settingsService = settingsService;
    }

    #endregion

    #region Properties

    public string LastClipboardText => _lastClipboardText;

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
        Dispose(true);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _clipboardMonitor.ClipboardChanged -= ClipboardMonitor_ClipboardChanged;
        }
    }

    /// <summary>
    /// Handles the ClipboardChanged event of the ClipboardMonitor control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="SharpClipboard.ClipboardChangedEventArgs"/> instance containing the event data.</param>
    private async void ClipboardMonitor_ClipboardChanged(object sender, SharpClipboard.ClipboardChangedEventArgs e)
    {
        if (e.ContentType != SharpClipboard.ContentTypes.Text)
        {
            return;
        }

        var currentText = e.Content as string;
        if (string.IsNullOrEmpty(currentText) ||
            _lastClipboardText == currentText ||
            Native.IsKeyPressed(Native.VirtualKeyStates.VK_LSHIFT))
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
            _lastClipboardText = currentText;
            NewOffer?.Invoke(this, currentText);

            if (_settingsService.AutoWhisperEnabled)
            {
                await _keyboardHelper.SendMessage(currentText);
            }
        }
    }

    #endregion
}