//-----------------------------------------------------------------------
// <copyright file="Hotkey.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Models
{
    using ConfOxide;
    using Winook;
    using static Winook.KeyboardHook;

    /// <summary>
    /// Represents a hotkey.
    /// </summary>
    public sealed class Hotkey : SettingsBase<Hotkey>
    {
        #region Fields

        private KeyboardEventHandler _handler;
        private KeyCode _registeredKeycode;
        private Modifiers _registeredModifier;

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
        public void Install(KeyboardHook hook, KeyboardEventHandler handler)
        {
            if (!this.IsDefined() || handler == null)
            {
                return;
            }

            this._handler = handler;
            if (this.Modifier != Modifiers.None)
            {
                hook.AddHandler(this.KeyCode, KeyDirection.Up, this.Modifier, handler);
            }
            else
            {
                hook.AddHandler(this.KeyCode, this._handler);
            }

            this._registeredKeycode = this.KeyCode;
            this._registeredModifier = this.Modifier;
        }

        /// <summary>
        /// Uninstalls the specified hook.
        /// </summary>
        /// <param name="hook">The hook.</param>
        public void Uninstall(KeyboardHook hook)
        {
            if (!this.IsHooked() || this._handler == null)
            {
                return;
            }

            if (this._registeredModifier != Modifiers.None)
            {
                hook.RemoveHandler(this._registeredKeycode, this._registeredModifier, KeyDirection.Up, this._handler);
            }
            else
            {
                hook.RemoveHandler(this._registeredKeycode, this._handler);
            }

            this._registeredKeycode = KeyCode.None;
            this._registeredModifier = Modifiers.None;
        }

        #endregion
    }
}