//-----------------------------------------------------------------------
// <copyright file="SettingsView.xaml.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.Views
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows.Interop;
    using MahApps.Metro.Controls;

    /// <summary>
    /// Interaction logic for SettingsView.xaml.
    /// </summary>
    public partial class SettingsView : MetroWindow
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsView"/> class.
        /// </summary>
        public SettingsView()
        {
            this.InitializeComponent();

            var hWnd = new WindowInteropHelper(GetWindow(this)).EnsureHandle();
            var attribute = DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE;
            var preference = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND;
            DwmSetWindowAttribute(hWnd, attribute, ref preference, sizeof(uint));
        }

        #endregion

        /// <summary>
        /// Represents window attributes.
        /// </summary>
        public enum DWMWINDOWATTRIBUTE
        {
            /// <summary>
            /// The windows preference.
            /// </summary>
            DWMWA_WINDOW_CORNER_PREFERENCE = 33,
        }

        /// <summary>
        /// The DWM_WINDOW_CORNER_PREFERENCE enum for DwmSetWindowAttribute's third parameter, which tells the function
        /// what value of the enum to set.
        /// </summary>
        public enum DWM_WINDOW_CORNER_PREFERENCE
        {
            /// <summary>
            /// Let the system decide whether or not to round window corners.
            /// </summary>
            DWMWCP_DEFAULT = 0,

            /// <summary>
            /// Never round window corners.
            /// </summary>
            DWMWCP_DONOTROUND = 1,

            /// <summary>
            /// Round the corners if appropriate.
            /// </summary>
            DWMWCP_ROUND = 2,

            /// <summary>
            /// Round the corners if appropriate, with a small radius.
            /// </summary>
            DWMWCP_ROUNDSMALL = 3,
        }

        /// <summary>
        /// The DWM_WINDOW_CORNER_PREFERENCE enum for DwmSetWindowAttribute's third parameter, which tells the function
        /// what value of the enum to set.
        /// </summary>
        /// <param name="hwnd">The identifier.</param>
        /// <param name="attribute">The attribute.</param>
        /// <param name="pvAttribute">The pvAttribute.</param>
        /// <param name="cbAttribute">The cbAttribute.</param>
        /// <returns>The long value.</returns>
        [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern long DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE attribute, ref DWM_WINDOW_CORNER_PREFERENCE pvAttribute, uint cbAttribute);
    }
}