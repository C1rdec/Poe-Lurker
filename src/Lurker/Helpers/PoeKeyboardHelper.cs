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
        public async Task Destroy()
        {
            await this.SendCommand("/destroy");
        }

        /// <summary>
        /// Remainings the monster.
        /// </summary>
        /// <returns>The task.</returns>
        public async Task RemainingMonster()
        {
            await this.SendCommand("/remaining");
        }

        /// <summary>
        /// Whoes the is.
        /// </summary>
        /// <param name="playerName">Name of the player.</param>
        /// <returns>The task.</returns>
        public async Task WhoIs(string playerName)
        {
            await this.SendCommand($@"/whois {playerName}");
        }

        /// <summary>
        /// Invites to party.
        /// </summary>
        /// <param name="playerName">Name of the player.</param>
        /// <returns>The task.</returns>
        public async Task Invite(string playerName)
        {
            await this.SendCommand($@"/invite {playerName}");
        }

        /// <summary>
        /// Kicks the specified player name.
        /// </summary>
        /// <param name="playerName">Name of the player.</param>
        /// <returns>The task.</returns>
        public async Task Kick(string playerName)
        {
            await this.SendCommand($@"/kick {playerName}");
        }

        /// <summary>
        /// Trades the specified character name.
        /// </summary>
        /// <param name="playerName">Name of the player.</param>
        /// <returns>The task.</returns>
        public async Task Trade(string playerName)
        {
            await this.SendCommand($@"/tradewith {playerName}");
        }

        /// <summary>
        /// Whispers the specified character name.
        /// </summary>
        /// <param name="playerName">Name of the character.</param>
        /// <param name="message">The message.</param>
        /// <returns>The task.</returns>
        public async Task Whisper(string playerName, string message)
        {
            await this.Whisper(playerName, message, true);
        }

        /// <summary>
        /// Whispers the specified character name.
        /// </summary>
        /// <param name="playerName">Name of the character.</param>
        /// <param name="message">The message.</param>
        /// <param name="setForeground">if set to <c>true</c> [set foreground].</param>
        /// <returns>
        /// The task.
        /// </returns>
        public async Task Whisper(string playerName, string message, bool setForeground)
        {
            await this.SendCommand($@"@{playerName} {message}", setForeground);
        }

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>The task.</returns>
        public async Task SendMessage(string message)
        {
            await this.SendCommand(message);
        }

        /// <summary>
        /// Joins the hideout.
        /// </summary>
        /// <returns>The task.</returns>
        public async Task JoinHideout()
        {
            await this.JoinHideout(null);
        }

        /// <summary>
        /// Joins the hideout.
        /// </summary>
        /// <param name="playerName">Name of the player.</param>
        /// <returns>The task.</returns>
        public async Task JoinHideout(string playerName)
        {
            if (string.IsNullOrEmpty(playerName))
            {
                await this.SendCommand($@"/hideout");
                return;
            }

            await this.SendCommand($@"/hideout {playerName}");
        }

        #endregion
    }
}