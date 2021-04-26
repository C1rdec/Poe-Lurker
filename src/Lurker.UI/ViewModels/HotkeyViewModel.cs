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
        private Func<string, Task<ushort>> _getKeyCode;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HotkeyViewModel" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="hotkey">The hotkey.</param>
        /// <param name="getKeyCode">The get key code.</param>
        public HotkeyViewModel(string name, Hotkey hotkey, Func<string, Task<ushort>> getKeyCode)
        {
            this._hotkey = hotkey;
            this._name = name;
            this._getKeyCode = getKeyCode;
        }

        #endregion

        #region Properties

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
            var code = await this._getKeyCode(this.NameValue);
            this.KeyCode = (KeyCode)code;
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