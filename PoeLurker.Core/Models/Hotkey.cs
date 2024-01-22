//-----------------------------------------------------------------------
// <copyright file="Hotkey.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.Core.Models;

using System;
using Winook;

/// <summary>
/// Represents a hotkey.
/// </summary>
public sealed class Hotkey
{
    #region Fields

    private Action<KeyboardMessageEventArgs> _callback;
    private KeyCode _registeredKeycode;
    private Modifiers _registeredModifier;
    private bool _isHoldHotkey;
    private KeyboardHook _hook;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the modifier.
    /// </summary>
    public Modifiers Modifier { get; set; }

    /// <summary>
    /// Gets or sets the key code.
    /// </summary>
    public KeyCode KeyCode { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// Determines whether this instance is defined.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if this instance is defined; otherwise, <c>false</c>.
    /// </returns>
    public bool IsDefined()
    {
        return KeyCode != KeyCode.None;
    }

    /// <summary>
    /// Determines whether this instance is hooked.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if this instance is hooked; otherwise, <c>false</c>.
    /// </returns>
    public bool IsHooked()
    {
        return _registeredKeycode != KeyCode.None;
    }

    /// <summary>
    /// Installs the specified hook.
    /// </summary>
    /// <param name="hook">The hook.</param>
    /// <param name="handler">The handler.</param>
    /// <param name="hold">if set to <c>true</c> [hold].</param>
    public void Install(KeyboardHook hook, Action<KeyboardMessageEventArgs> handler, bool hold = false)
    {
        if (!IsDefined() || handler == null)
        {
            return;
        }

        hook.AddHandler(KeyCode, Modifier, KeyDirection.Up, KeyboardHandler);

        if (hold)
        {
            hook.AddHandler(KeyCode, Modifier, KeyDirection.Down, KeyboardHandler);
        }

        _hook = hook;
        _callback = handler;
        _registeredKeycode = KeyCode;
        _registeredModifier = Modifier;
        _isHoldHotkey = true;
    }

    /// <summary>
    /// Uninstalls the specified hook.
    /// </summary>
    public void Uninstall()
    {
        if (!IsHooked() || _callback == null || _hook == null)
        {
            return;
        }

        _hook.RemoveHandler(_registeredKeycode, _registeredModifier, KeyDirection.Up, KeyboardHandler);

        if (_isHoldHotkey)
        {
            _hook.RemoveHandler(_registeredKeycode, _registeredModifier, KeyDirection.Down, KeyboardHandler);
        }

        _hook = null;
        _callback = null;
        _isHoldHotkey = false;
        _registeredKeycode = KeyCode.None;
        _registeredModifier = Modifiers.None;
    }

    private void KeyboardHandler(object sender, Winook.KeyboardMessageEventArgs e)
    {
        _callback?.Invoke(e);
    }

    #endregion
}