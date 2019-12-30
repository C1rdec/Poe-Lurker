//-----------------------------------------------------------------------
// <copyright file="PoeKeyboardHelper.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.Helpers
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
        /// <param name="characterName">Name of the character.</param>
        public void Invite(string characterName)
        {
            this.SendCommand($@"/invite {characterName}");
        }

        /// <summary>
        /// Trades the specified character name.
        /// </summary>
        /// <param name="characterName">Name of the character.</param>
        public void Trade(string characterName)
        {
            this.SendCommand($@"/tradewith {characterName}");
        }

        #endregion
    }
}
