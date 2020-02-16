//-----------------------------------------------------------------------
// <copyright file="PoeKeyboardHelper.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Helpers
{
    using System.Diagnostics;

    public class PoeKeyboardHelper : KeyboardHelper
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PoeKeyboardHelper"/> class.
        /// </summary>
        /// <param name="windowHandle">The window handle.</param>
        public PoeKeyboardHelper(Process poeProcess) 
            : base(poeProcess.MainWindowHandle)
        {
        }

        #endregion

        #region Methods

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
        /// Joins the hideout.
        /// </summary>
        /// <param name="playerName">Name of the player.</param>
        public void JoinHideout(string playerName)
        {
            this.SendCommand($@"/hideout {playerName}");
        }

        #endregion
    }
}
