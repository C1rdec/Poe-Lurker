﻿//-----------------------------------------------------------------------
// <copyright file="PoeKeyboardHelper.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Helpers
{
    using System.Diagnostics;

    /// <summary>
    /// Represents the PoeKeyboardHelper.
    /// </summary>
    /// <seealso cref="Lurker.Helpers.KeyboardHelper" />
    public class PoeKeyboardHelper : KeyboardHelper
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PoeKeyboardHelper" /> class.
        /// </summary>
        /// <param name="processId">The process identifier.</param>
        public PoeKeyboardHelper(int processId)
            : base(processId)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Destroys this instance.
        /// </summary>
        public void Destroy()
        {
            this.SendCommand("/destroy");
        }

        /// <summary>
        /// Remainings the monster.
        /// </summary>
        public void RemainingMonster()
        {
            this.SendCommand("/remaining");
        }

        /// <summary>
        /// Whoes the is.
        /// </summary>
        /// <param name="playerName">Name of the player.</param>
        public void WhoIs(string playerName)
        {
            this.SendCommand($@"/whois {playerName}");
        }

        /// <summary>
        /// Invites to party.
        /// </summary>
        /// <param name="playerName">Name of the player.</param>
        public void Invite(string playerName)
        {
            this.SendCommand($@"/invite {playerName}");
        }

        /// <summary>
        /// Kicks the specified player name.
        /// </summary>
        /// <param name="playerName">Name of the player.</param>
        public void Kick(string playerName)
        {
            this.SendCommand($@"/kick {playerName}");
        }

        /// <summary>
        /// Trades the specified character name.
        /// </summary>
        /// <param name="playerName">Name of the player.</param>
        public void Trade(string playerName)
        {
            this.SendCommand($@"/tradewith {playerName}");
        }

        /// <summary>
        /// Whispers the specified character name.
        /// </summary>
        /// <param name="playerName">Name of the character.</param>
        /// <param name="message">The message.</param>
        public void Whisper(string playerName, string message)
        {
            this.SendCommand($@"@{playerName} {message}");
        }

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void SendMessage(string message)
        {
            this.SendCommand(message);
        }

        /// <summary>
        /// Joins the hideout.
        /// </summary>
        public void JoinHideout()
        {
            this.JoinHideout(null);
        }

        /// <summary>
        /// Joins the hideout.
        /// </summary>
        /// <param name="playerName">Name of the player.</param>
        public void JoinHideout(string playerName)
        {
            if (string.IsNullOrEmpty(playerName))
            {
                this.SendCommand($@"/hideout");
                return;
            }

            this.SendCommand($@" /hideout {playerName}");
        }

        #endregion
    }
}