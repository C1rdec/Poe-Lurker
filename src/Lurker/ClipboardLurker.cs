//-----------------------------------------------------------------------
// <copyright file="ClipboardLurker.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------


namespace Lurker
{
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;

    public class ClipboardLurker
    {
        #region Fields

        private Dispatcher _dispatcher;

        #endregion

        #region Constructors

        public ClipboardLurker()
        {
            var text = GetClipboardData();
        }

        #endregion

        #region Methods

        private string GetClipboardData()
        {
            var clipboardText = string.Empty;
            Thread thread = new Thread(() => { clipboardText = Clipboard.GetText(); });
            thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            thread.Start();
            thread.Join();

            return clipboardText;
        }

        #endregion
    }
}
