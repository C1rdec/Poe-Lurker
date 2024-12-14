//-----------------------------------------------------------------------
// <copyright file="KeyboardLurker.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.Core;

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Desktop.Robot;
using Desktop.Robot.Extensions;
using PoeLurker.Core.Helpers;
using PoeLurker.Core.Models;
using PoeLurker.Core.Services;
using PoeLurker.Patreon.Parsers;
using PoeLurker.Patreon.Services;
using Winook;
using static Winook.KeyboardHook;

/// <summary>
/// Represents the keyboard lurker.
/// </summary>
public class KeyboardLurker
{
    #region Fields

    private readonly Robot _robot;
    private CancellationTokenSource _tokenSource;
    private readonly KeyboardHook _keyboardHook;
    private readonly ItemParser _itemParser;
    private readonly SettingsService _settingsService;
    private readonly HotkeyService _hotkeyService;
    private readonly PoeKeyboardHelper _keyboardHelper;
    private Task _currentHoldTask;
    private bool _disposed;
    private KeyCode _toggleBuildCode;
    private readonly int _processId;
    private readonly bool _hooked;
    private bool _disabled;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyboardLurker" /> class.
    /// </summary>
    /// <param name="processId">The process identifier.</param>
    /// <param name="settingsService">The settings service.</param>
    /// <param name="hotkeyService">The key code service.</param>
    /// <param name="keyboardHelper">The keyboard helper.</param>
    public KeyboardLurker(int processId, SettingsService settingsService, HotkeyService hotkeyService, PoeKeyboardHelper keyboardHelper)
    {
        _robot = new Robot();
        _processId = processId;
        _settingsService = settingsService;
        _hotkeyService = hotkeyService;
        _keyboardHelper = keyboardHelper;

        _itemParser = new ItemParser();
        if (settingsService.ConnectedToPatreon)
        {
            _itemParser.CheckPledgeStatus();
        }

        _settingsService.OnSave += SettingsService_OnSave;
        _keyboardHook = new KeyboardHook(_processId);
        _ = _keyboardHook.InstallAsync();
        _hooked = true;
    }

    #endregion

    #region Events

    /// <summary>
    /// Occurs when [build toggled].
    /// </summary>
    public event EventHandler BuildToggled;

    /// <summary>
    /// Occurs when [invite pressed].
    /// </summary>
    public event KeyboardEventHandler InvitePressed;

    /// <summary>
    /// Occurs when [whisper pressed].
    /// </summary>
    public event KeyboardEventHandler WhisperPressed;

    /// <summary>
    /// Occurs when [busy pressed].
    /// </summary>
    public event KeyboardEventHandler BusyPressed;

    /// <summary>
    /// Occurs when [dismiss pressed].
    /// </summary>
    public event KeyboardEventHandler DismissPressed;

    /// <summary>
    /// Occurs when [main action pressed].
    /// </summary>
    public event KeyboardEventHandler MainActionPressed;

    /// <summary>
    /// Occurs when [wiki action pressed].
    /// </summary>
    public event KeyboardEventHandler OpenWikiPressed;

    /// <summary>
    /// Occurs when [wiki action pressed].
    /// </summary>
    public event KeyboardEventHandler PoeTradePressed;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the hotkey.
    /// </summary>
    public Hotkey OpenWikiHotkey => _hotkeyService.OpenWiki;

    #endregion

    #region Methods

    /// <summary>
    /// Waits for next key asynchronous.
    /// </summary>
    /// <returns>The next key press.</returns>
    public Task<ushort> WaitForNextKeyAsync()
    {
        var taskCompletionSource = new TaskCompletionSource<ushort>();
        using (var hook = new KeyboardHook(Process.GetCurrentProcess().Id))
        {
            EventHandler<KeyboardMessageEventArgs> handler = default;
            handler = (object s, KeyboardMessageEventArgs e) =>
            {
                taskCompletionSource.SetResult(e.KeyValue);
                hook.MessageReceived -= handler;
            };

            hook.MessageReceived += handler;
            _ = hook.InstallAsync();

            return taskCompletionSource.Task;
        }
    }

    /// <summary>
    /// Toggles the build.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="KeyboardMessageEventArgs"/> instance containing the event data.</param>
    public void ToggleBuild(object sender, KeyboardMessageEventArgs e)
    {
        BuildToggled?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    public void Dispose() => Dispose(true);

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _keyboardHook.Dispose();
            }

            _disposed = true;
        }
    }

    /// <summary>
    /// Mains the action toggled.
    /// </summary>
    /// <param name="e">The <see cref="KeyboardMessageEventArgs"/> instance containing the event data.</param>
    private async void MainActionToggled(KeyboardMessageEventArgs e)
    {
        if (_disabled)
        {
            return;
        }

        if (e.Direction == KeyDirection.Down)
        {
            if (_currentHoldTask != null)
            {
                return;
            }

            try
            {
                _tokenSource = new CancellationTokenSource();
                _currentHoldTask = Task.Delay(500);
                await _currentHoldTask;
                if (_tokenSource.IsCancellationRequested)
                {
                    _currentHoldTask = null;
                    return;
                }

                InvitePressed?.Invoke(this, e);
            }
            finally
            {
                _tokenSource.Dispose();
                _tokenSource = null;
            }

            return;
        }

        if (e.Direction == KeyDirection.Up)
        {
            if (_currentHoldTask == null)
            {
                return;
            }

            if (_currentHoldTask.IsCompleted)
            {
                _tokenSource = new CancellationTokenSource();
                _currentHoldTask = null;
                return;
            }

            _tokenSource.Cancel();
            MainActionPressed?.Invoke(this, e);

            return;
        }
    }

    /// <summary>
    /// Handles the OnSave event of the SettingsService control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void SettingsService_OnSave(object sender, EventArgs e)
    {
        UninstallHandlers();
        InstallHandlers();
    }

    /// <summary>
    /// Installs the hook handlers.
    /// </summary>
    public void InstallHandlers()
    {
        if (_settingsService.BuildHelper)
        {
            _toggleBuildCode = _hotkeyService.ToggleBuild;
            _keyboardHook.AddHandler(_toggleBuildCode, ToggleBuild);
        }

        _keyboardHook.MessageReceived += KeyboardHook_MessageReceived;
        _keyboardHook.AddHandler(KeyCode.Delete, DeleteItem);

        // Hotkeys
        _hotkeyService.Main.Install(_keyboardHook, MainActionToggled, true);
        _hotkeyService.Whisper.Install(_keyboardHook, (e) => HandleKeyboardMessage(e, WhisperPressed));
        _hotkeyService.Busy.Install(_keyboardHook, (e) => HandleKeyboardMessage(e, BusyPressed));
        _hotkeyService.Dismiss.Install(_keyboardHook, (e) => HandleKeyboardMessage(e, DismissPressed));
        _hotkeyService.OpenWiki.Install(_keyboardHook, (e) => HandleKeyboardMessage(e, OpenWikiPressed));
        _hotkeyService.JoinGuildHideout.Install(_keyboardHook, (e) => HandleKeyboardMessage(e, JoinGuildHideout));
        _hotkeyService.JoinHideout.Install(_keyboardHook, (e) => HandleKeyboardMessage(e, JoinHideout));
        _hotkeyService.RemainingMonster.Install(_keyboardHook, (e) => HandleKeyboardMessage(e, RemainingMonsters));
        _hotkeyService.SearchItem.Install(_keyboardHook, SearchItem);
        _hotkeyService.PoeTrade.Install(_keyboardHook, OpenPoeTrade);
    }

    private void HandleKeyboardMessage(KeyboardMessageEventArgs message, KeyboardEventHandler handler)
    {
        if (_disabled)
        {
            return;
        }

        handler?.Invoke(this, message);
    }

    private async void KeyboardHook_MessageReceived(object sender, KeyboardMessageEventArgs e)
    {
        var keyCode = (KeyCode)e.KeyValue;
        if (!_disabled && keyCode == KeyCode.Enter)
        {
            _disabled = true;
            await Task.Delay(2500);
            _disabled = false;
        }
    }

    /// <summary>
    /// Uninstalls the hook handlers.
    /// </summary>
    private void UninstallHandlers()
    {
        if (!_hooked)
        {
            return;
        }

        _keyboardHook.RemoveAllHandlers();

        _hotkeyService.Main.Uninstall();
        _hotkeyService.Busy.Uninstall();
        _hotkeyService.Dismiss.Uninstall();
        _hotkeyService.OpenWiki.Uninstall();
        _hotkeyService.RemainingMonster.Uninstall();
        _hotkeyService.JoinGuildHideout.Uninstall();
        _hotkeyService.JoinHideout.Uninstall();
    }

    /// <summary>
    /// Searches the item.
    /// </summary>
    /// <param name="e">The <see cref="KeyboardMessageEventArgs"/> instance containing the event data.</param>
    private async void SearchItem(KeyboardMessageEventArgs e)
    {
        var baseType = await GetItemSearchValueInClipboard();
        if (string.IsNullOrEmpty(baseType))
        {
            return;
        }

        await _keyboardHelper.Search(baseType);
    }

    private async void OpenPoeTrade(KeyboardMessageEventArgs e)
    {
        var itemText = await GetClipboardTextAsync();

        PoeTradeService.Open(itemText);
    }

    /// <summary>
    /// Join the Guild Hideout.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event.</param>
    private async void JoinGuildHideout(object sender, KeyboardMessageEventArgs e)
    {
        await _keyboardHelper.JoinGuildHideout();
    }

    /// <summary>
    /// Join the Hideout.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event.</param>
    private async void JoinHideout(object sender, KeyboardMessageEventArgs e)
    {
        await _keyboardHelper.JoinHideout();
    }

    /// <summary>
    /// Deletes the item.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="KeyboardMessageEventArgs" /> instance containing the event data.</param>
    private async void DeleteItem(object sender, KeyboardMessageEventArgs e)
    {
        if (_settingsService.DeleteItemEnabled)
        {
            _robot.Click();
            await Task.Delay(50);
            await _keyboardHelper.Destroy();
        }
    }

    /// <summary>
    /// Remainings the monsters.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="KeyboardMessageEventArgs"/> instance containing the event data.</param>
    private async void RemainingMonsters(object sender, KeyboardMessageEventArgs e)
    {
        await _keyboardHelper.RemainingMonster();
    }

    /// <summary>
    /// Gets the item base type in clipboard.
    /// </summary>
    /// <returns>The item search value.</returns>
    private async Task<string> GetItemSearchValueInClipboard()
    {
        try
        {
            var text = await GetClipboardTextAsync();
            ClipboardHelper.ClearClipboard();

            return _itemParser.GetSearchValue(text);
        }
        catch
        {
            return null;
        }
    }

    private async Task<string> GetClipboardTextAsync()
    {
        _robot.CombineKeys(Key.Control, Key.C);
        await Task.Delay(50);
        return ClipboardHelper.GetClipboardText();
    }

    #endregion
}