//-----------------------------------------------------------------------
// <copyright file="ChangedLocationEvent.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Events
{
    public class LocationChangedEvent : PoeEvent
    {
        #region Fields

        private static readonly string LocationMarker = "You have entered";

        #endregion

        #region Constructors

        private LocationChangedEvent(string logLine) 
            : base(logLine)
        {
            // -2 (Zero base and to remove the "." at the end)
            this.Location = this.Message.Substring(LocationMarker.Length + 1, this.Message.Length - LocationMarker.Length - 2);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the location name the character have entered.
        /// </summary>
        public string Location { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Tries the parse.
        /// </summary>
        /// <param name="logLine">The log line.</param>
        /// <returns>The ChangedLocation event.</returns>
        public static LocationChangedEvent TryParse(string logLine)
        {
            var informations = ParseInformations(logLine);
            if (string.IsNullOrEmpty(informations) || !informations.StartsWith($"{MessageMarker}{LocationMarker}"))
            {
                return null;
            }

            return new LocationChangedEvent(logLine);
        }

        #endregion
    }
}
