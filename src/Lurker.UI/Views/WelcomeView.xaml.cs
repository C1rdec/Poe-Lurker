//-----------------------------------------------------------------------
// <copyright file="WelcomeView.xaml.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.Views
{
    using System.Windows.Interop;
    using MahApps.Metro.Controls;
    using static Lurker.UI.Views.SettingsView;

    /// <summary>
    /// Interaction logic for WelcomeView.xaml.
    /// </summary>
    public partial class WelcomeView : MetroWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WelcomeView"/> class.
        /// </summary>
        public WelcomeView()
        {
            this.InitializeComponent();

            var hWnd = new WindowInteropHelper(GetWindow(this)).EnsureHandle();
            var attribute = DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE;
            var preference = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND;
            DwmSetWindowAttribute(hWnd, attribute, ref preference, sizeof(uint));
        }
    }
}