//-----------------------------------------------------------------------
// <copyright file="PoeKeyboardHelper.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Helpers
{
    using System.Threading.Tasks;

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
        /// <returns>The task.</returns>
        public Task Destroy()
            => this.SendCommand("/destroy");

        /// <summary>
        /// Remainings the monster.
        /// </summary>
        /// <returns>The task.</returns>
        public Task RemainingMonster()
            => this.SendCommand("/remaining");

        /// <summary>
        /// Whoes the is.
        /// </summary>
        /// <param name="playerName">Name of the player.</param>
        /// <returns>The task.</returns>
        public Task WhoIs(string playerName)
            => this.SendCommand($@"/whois {playerName}");

        /// <summary>
        /// Invites to party.
        /// </summary>
        /// <param name="playerName">Name of the player.</param>
        /// <returns>The task.</returns>
        public Task Invite(string playerName)
            => this.SendCommand($@"/invite {playerName}");

        /// <summary>
        /// Whisper to buyer.
        /// </summary>
        /// <param name="playerName">Name of the player.</param>
        /// <returns>The task.</returns>
        public Task Whisper(string playerName)
            => this.SendCommand($@"@{playerName} ", true);

        /// <summary>
        /// Kicks the specified player name.
        /// </summary>
        /// <param name="playerName">Name of the player.</param>
        /// <returns>The task.</returns>
        public Task Kick(string playerName)
            => this.SendCommand($@"/kick {playerName}");

        /// <summary>
        /// Leave the current party.
        /// </summary>
        /// <returns>The task.</returns>
        public Task Leave()
            => this.SendCommand("/leave");

        /// <summary>
        /// Trades the specified character name.
        /// </summary>
        /// <param name="playerName">Name of the player.</param>
        /// <returns>The task.</returns>
        public Task Trade(string playerName)
            => this.SendCommand($@"/tradewith {playerName}");

        /// <summary>
        /// Whispers the specified character name.
        /// </summary>
        /// <param name="playerName">Name of the character.</param>
        /// <param name="message">The message.</param>
        /// <returns>The task.</returns>
        public Task Whisper(string playerName, string message)
            => this.SendCommand($@"@{playerName} {message}");

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>The task.</returns>
        public Task SendMessage(string message)
            => this.SendCommand(message);

        /// <summary>
        /// Sends joni gild Hiedout.
        /// </summary>
        /// <returns>The task.</returns>
        public Task JoinGuildHideout()
            => this.SendCommand($@"/guild");

        /// <summary>
        /// Joins the hideout.
        /// </summary>
        /// <returns>The task.</returns>
        public Task JoinHideout()
            => this.JoinHideout(null);

        /// <summary>
        /// Joins the hideout.
        /// </summary>
        /// <param name="playerName">Name of the player.</param>
        /// <returns>The task.</returns>
        public Task JoinHideout(string playerName)
        {
            if (string.IsNullOrEmpty(playerName))
            {
                return this.SendCommand($@"/hideout");
            }

            return this.SendCommand($@"/hideout {playerName}");
        }

        #endregion
    }
}