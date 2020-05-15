//-----------------------------------------------------------------------
// <copyright file="OutgoingbarView.xaml.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.Views
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for OutgoingbarView.xaml.
    /// </summary>
    public partial class OutgoingbarView : Window
    {
        #region Fields

        private Window _parent;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OutgoingbarView"/> class.
        /// </summary>
        public OutgoingbarView()
        {
            this.InitializeComponent();
            this.HideFromAltTab();
        }

        #endregion

        #region Methods

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
                WindowStyle = WindowStyle.ToolWindow, // Set window style as ToolWindow to avoid its icon in AltTab
                ShowInTaskbar = false,
            };

            this._parent.Show();
            this.Owner = this._parent;
            this._parent.Hide();
        }

        /// <summary>
        /// Handles the PreviewMouseWheel event of the ScrollViewer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseWheelEventArgs"/> instance containing the event data.</param>
        private void ScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        #endregion
    }
}