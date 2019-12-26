//-----------------------------------------------------------------------
// <copyright file="PlayerEvent.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Events
{
    public abstract class PlayerEvent : PoeEvent
    {
        #region Fields

        protected static readonly string EndOfGuildNameMarker = ">";

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerEvent"/> class.
        /// </summary>
        /// <param name="logLine">The log line.</param>
        protected PlayerEvent(string logLine)
            : base(logLine)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the player.
        public string PlayerName { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the guild.
        /// </summary>
        public string GuildName { get; protected set; }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the name of the player.
        /// </summary>
        /// <param name="index">The index.</param>
        protected void SetPlayerName(int index)
        {
            this.PlayerName = this.Message.Substring(0, index - 1);
        }

        #endregion
    }
}
