//-----------------------------------------------------------------------
// <copyright file="Hotkey.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Models
{
    using System;
    using ConfOxide;
    using Winook;
    using static Winook.KeyboardHook;

    /// <summary>
    /// Represents a hotkey.
    /// </summary>
    public sealed class Hotkey : SettingsBase<Hotkey>
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
            return this.KeyCode != KeyCode.None;
        }

        /// <summary>
        /// Determines whether this instance is hooked.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is hooked; otherwise, <c>false</c>.
        /// </returns>
        public bool IsHooked()
        {
            return this._registeredKeycode != KeyCode.None;
        }

        /// <summary>
        /// Installs the specified hook.
        /// </summary>
        /// <param name="hook">The hook.</param>
        /// <param name="handler">The handler.</param>
        /// <param name="hold">if set to <c>true</c> [hold].</param>
        public void Install(KeyboardHook hook, Action<KeyboardMessageEventArgs> handler, bool hold = false)
        {
            if (!this.IsDefined() || handler == null)
            {
                return;
            }

            hook.AddHandler(this.KeyCode, this.Modifier, KeyDirection.Up, this.KeyboardHandler);

            if (hold)
            {
                hook.AddHandler(this.KeyCode, this.Modifier, KeyDirection.Down, this.KeyboardHandler);
            }

            this._hook = hook;
            this._callback = handler;
            this._registeredKeycode = this.KeyCode;
            this._registeredModifier = this.Modifier;
            this._isHoldHotkey = true;
        }

        /// <summary>
        /// Uninstalls the specified hook.
        /// </summary>
        public void Uninstall()
        {
            if (!this.IsHooked() || this._callback == null || this._hook == null)
            {
                return;
            }

            this._hook.RemoveHandler(this._registeredKeycode, this._registeredModifier, KeyDirection.Up, this.KeyboardHandler);

            if (this._isHoldHotkey)
            {
                this._hook.RemoveHandler(this._registeredKeycode, this._registeredModifier, KeyDirection.Down, this.KeyboardHandler);
            }

            this._hook = null;
            this._callback = null;
            this._isHoldHotkey = false;
            this._registeredKeycode = KeyCode.None;
            this._registeredModifier = Modifiers.None;
        }

        private void KeyboardHandler(object sender, Winook.KeyboardMessageEventArgs e)
        {
            this._callback?.Invoke(e);
        }

        #endregion
    }
}