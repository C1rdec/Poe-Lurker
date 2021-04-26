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

        private KeyboardEventHandler _handler;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the modifier.
        /// </summary>
        public Winook.Modifiers Modifier { get; set; }

        /// <summary>
        /// Gets or sets the key code.
        /// </summary>
        public Winook.KeyCode KeyCode { get; set; }

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
            return this.KeyCode != Winook.KeyCode.None;
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
                hook.AddHandler(this.KeyCode, this.Modifier, handler);
            }
            else
            {
                hook.AddHandler(this.KeyCode, KeyDirection.Down, Modifiers.Alt, this._handler);
            }
        }

        /// <summary>
        /// Uninstalls the specified hook.
        /// </summary>
        /// <param name="hook">The hook.</param>
        public void Uninstall(KeyboardHook hook)
        {
            if (!this.IsDefined() || this._handler == null)
            {
                return;
            }

            if (this.Modifier != Modifiers.None)
            {
                hook.RemoveHandler(this.KeyCode, this.Modifier, KeyDirection.Down, this._handler);
            }
            else
            {
                hook.RemoveHandler(this.KeyCode, Modifiers.Alt, KeyDirection.Down, this._handler);
            }
        }

        #endregion
    }
}