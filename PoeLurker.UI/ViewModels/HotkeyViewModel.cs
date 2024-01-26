//-----------------------------------------------------------------------
// <copyright file="HotkeyViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using System;
using System.Threading.Tasks;
using PoeLurker.Core.Models;
using Winook;

/// <summary>
/// Represents a hotkey.
/// </summary>
/// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
public class HotkeyViewModel : Caliburn.Micro.PropertyChangedBase
{
    #region Fields

    private readonly Hotkey _hotkey;
    private readonly string _name;
    private readonly Func<string, Task<(KeyCode Key, Modifiers Modifier)>> _getKey;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="HotkeyViewModel" /> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="hotkey">The hotkey.</param>
    /// <param name="getKey">The get key.</param>
    public HotkeyViewModel(string name, Hotkey hotkey, Func<string, Task<(KeyCode Key, Modifiers Modifier)>> getKey)
    {
        _hotkey = hotkey;
        _name = name;
        _getKey = getKey;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether [not defined].
    /// </summary>
    public bool NotDefined => !_hotkey.IsDefined();

    /// <summary>
    /// Gets a value indicating whether this instance has modifier.
    /// </summary>
    public bool HasModifier => Modifier != Modifiers.None;

    /// <summary>
    /// Gets a value indicating whether this instance has key code.
    /// </summary>
    public bool HasKeyCode => KeyCode != KeyCode.None;

    /// <summary>
    /// Gets the modifier.
    /// </summary>
    public Winook.Modifiers Modifier
    {
        get
        {
            return _hotkey.Modifier;
        }

        private set
        {
            _hotkey.Modifier = value;
            NotifyOfPropertyChange();
            NotifyOfPropertyChange(() => HasModifier);
        }
    }

    /// <summary>
    /// Gets the key code.
    /// </summary>
    public Winook.KeyCode KeyCode
    {
        get
        {
            return _hotkey.KeyCode;
        }

        private set
        {
            _hotkey.KeyCode = value;
            NotifyOfPropertyChange();
            NotifyOfPropertyChange(() => HasKeyCode);
            NotifyOfPropertyChange(() => NotDefined);
        }
    }

    /// <summary>
    /// Gets the name value.
    /// </summary>
    public string NameValue => _name;

    #endregion

    #region Methods

    /// <summary>
    /// Sets the key code.
    /// </summary>
    public async void SetKeyCode()
    {
        var value = await _getKey(NameValue);
        var code = value.Key;
        if (code == KeyCode.Escape)
        {
            return;
        }

        KeyCode = code;
        Modifier = value.Modifier;
    }

    /// <summary>
    /// Removes this instance.
    /// </summary>
    public void Remove()
    {
        KeyCode = KeyCode.None;
        Modifier = Modifiers.None;
    }

    #endregion
}