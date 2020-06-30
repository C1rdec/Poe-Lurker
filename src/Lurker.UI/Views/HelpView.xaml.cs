//-----------------------------------------------------------------------
// <copyright file="HelpView.xaml.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.Views
{
    using System;
    using System.Windows;

    /// <summary>
    /// Interaction logic for HelpView.xaml.
    /// </summary>
    public partial class HelpView : Window
    {
        private Window _parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpView" /> class.
        /// </summary>
        public HelpView()
        {
            this.InitializeComponent();
            this.HideFromAltTab();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Window.Closed" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnClosed(EventArgs e)
        {
            this._parent.Close();
            base.OnClosed(e);
        }

        /// <summary>
        /// Hides the window from alt tab.
        /// </summary>
        private void HideFromAltTab()
        {
            this._parent = new Window
            {
                Top = -100,
                Left = -100,
                Width = 1,
                Height = 1,

                // Set window style as ToolWindow to avoid its icon in AltTab
                WindowStyle = WindowStyle.ToolWindow,
                ShowInTaskbar = false,
            };

            this._parent.Show();
            this.Owner = this._parent;
            this._parent.Hide();
        }
    }
}