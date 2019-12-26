//-----------------------------------------------------------------------
// <copyright file="PlayerEvent.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Events
{
    public abstract class PlayerEvent : PoeEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerEvent"/> class.
        /// </summary>
        /// <param name="logLine">The log line.</param>
        public PlayerEvent(string logLine)
            : base(logLine)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the player.
        public string PlayerName { get; protected set; }

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
