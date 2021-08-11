//-----------------------------------------------------------------------
// <copyright file="HotkeyViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System;
    using System.Threading.Tasks;
    using Lurker.Models;
    using Winook;

    /// <summary>
    /// Represents a hotkey.
    /// </summary>
    /// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
    public class HotkeyViewModel : Caliburn.Micro.PropertyChangedBase
    {
        #region Fields

        private Hotkey _hotkey;
        private string _name;
        private Func<string, Task<KeyboardMessageEventArgs>> _getKey;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HotkeyViewModel" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="hotkey">The hotkey.</param>
        /// <param name="getKey">The get key.</param>
        public HotkeyViewModel(string name, Hotkey hotkey, Func<string, Task<KeyboardMessageEventArgs>> getKey)
        {
            this._hotkey = hotkey;
            this._name = name;
            this._getKey = getKey;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether [not defined].
        /// </summary>
        public bool NotDefined => !this._hotkey.IsDefined();

        /// <summary>
        /// Gets a value indicating whether this instance has modifier.
        /// </summary>
        public bool HasModifier => this.Modifier != Modifiers.None;

        /// <summary>
        /// Gets a value indicating whether this instance has key code.
        /// </summary>
        public bool HasKeyCode => this.KeyCode != KeyCode.None;

        /// <summary>
        /// Gets the modifier.
        /// </summary>
        public Winook.Modifiers Modifier
        {
            get
            {
                return this._hotkey.Modifier;
            }

            private set
            {
                this._hotkey.Modifier = value;
                this.NotifyOfPropertyChange();
                this.NotifyOfPropertyChange(() => this.HasModifier);
            }
        }

        /// <summary>
        /// Gets the key code.
        /// </summary>
        public Winook.KeyCode KeyCode
        {
            get
            {
                return this._hotkey.KeyCode;
            }

            private set
            {
                this._hotkey.KeyCode = value;
                this.NotifyOfPropertyChange();
                this.NotifyOfPropertyChange(() => this.HasKeyCode);
                this.NotifyOfPropertyChange(() => this.NotDefined);
            }
        }

        /// <summary>
        /// Gets the name value.
        /// </summary>
        public string NameValue => this._name;

        #endregion

        #region Methods

        /// <summary>
        /// Sets the key code.
        /// </summary>
        public async void SetKeyCode()
        {
            var key = await this._getKey(this.NameValue);
            var code = (KeyCode)key.KeyValue;
            if (code == KeyCode.Escape)
            {
                return;
            }

            this.KeyCode = code;

            if (key.Control)
            {
                this.Modifier = Modifiers.Control;
            }
            else if (key.Alt)
            {
                this.Modifier = Modifiers.Alt;
            }
            else if (key.Shift)
            {
                this.Modifier = Modifiers.Shift;
            }
        }

        /// <summary>
        /// Removes this instance.
        /// </summary>
        public void Remove()
        {
            this.KeyCode = KeyCode.None;
            this.Modifier = Modifiers.None;
        }

        #endregion
    }
}