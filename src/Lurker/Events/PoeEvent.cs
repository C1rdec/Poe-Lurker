//-----------------------------------------------------------------------
// <copyright file="PoeEvent.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Events
{
    using System;
    using System.Globalization;
    using System.Linq;

    public abstract class PoeEvent : EventArgs
    {
        #region Fields

        protected static readonly string MessageMarker = ": ";

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PoeEvent"/> class.
        /// </summary>
        /// <param name="logLine">The log line.</param>
        protected PoeEvent(string logLine)
        {
            var result = logLine.Split(' ').Take(2);
            this.Date = DateTime.Parse(string.Join(" ", result), CultureInfo.InvariantCulture);

            this.Message = ParseMessage(logLine);
            this.Informations = ParseInformations(logLine);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        public DateTime Date { get; private set; }

        /// <summary>
        /// Gets the informations.
        /// </summary>
        protected string Informations { get; private set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        protected string Message { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Parses the message.
        /// </summary>
        /// <param name="logLine">The log line.</param>
        /// <returns>The message</returns>
        protected static string ParseMessage(string logLine)
        {
            var index = logLine.IndexOf(MessageMarker);
            if (index == -1)
            {
                return null;
            }

            return logLine.Substring(index + MessageMarker.Length);
        }

        /// <summary>
        /// Parses the information.
        /// </summary>
        /// <param name="logLine">The log line.</param>
        /// <returns>The information</returns>
        protected static string ParseInformations(string logLine)
        {
            var index = logLine.IndexOf("]");
            if (index == -1)
            {
                return null;
            }

            // +2 (Zero base and the white space)
            return logLine.Substring(index + 2);
        }

        #endregion
    }
}
