//-----------------------------------------------------------------------
// <copyright file="MonstersRemainEvent.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Events
{
    using System.Linq;

    public class MonstersRemainEvent : PoeEvent
    {
        #region Fields

        private static readonly string MonstersRemainMarker = "monsters remain.";

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MonstersRemainEvent"/> class.
        /// </summary>
        /// <param name="logLine">The log line.</param>
        private MonstersRemainEvent(string logLine) 
            : base(logLine)
        {
            var index = this.Message.IndexOf(MonstersRemainMarker);
            var beforeMarker = this.Message.Substring(0, index -1);
            var monsterRemainValue = beforeMarker.Split(' ').Last();

            this.MonsterCount = int.Parse(monsterRemainValue);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the remaining monster count in the instance.
        /// </summary>
        public int MonsterCount { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Tries the parse.
        /// </summary>
        /// <param name="newLine">The new line.</param>
        /// <returns></returns>
        public static MonstersRemainEvent TryParse(string logLine)
        {
            var informations = ParseInformations(logLine);
            if (string.IsNullOrEmpty(informations) || !informations.StartsWith(MessageMarker) || !informations.EndsWith(MonstersRemainMarker))
            {
                return null;
            }

            return new MonstersRemainEvent(logLine);
        }

        #endregion
    }
}
