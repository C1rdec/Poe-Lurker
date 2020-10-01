//-----------------------------------------------------------------------
// <copyright file="GemView.xaml.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.Views
{
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for GemView.xaml.
    /// </summary>
    public partial class GemView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GemView"/> class.
        /// </summary>
        public GemView()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Handles the MouseEnter event of the Grid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void Grid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.Popup.IsOpen = true;
            this.mainGrid.Focus();
        }

        /// <summary>
        /// Handles the MouseLeave event of the Grid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void Grid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.Popup.IsOpen = false;
            Keyboard.ClearFocus();
            this.GemLocation.Visibility = System.Windows.Visibility.Collapsed;
        }

        /// <summary>
        /// Handles the PreviewKeyDown event of the Grid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private void Grid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)
            {
                this.GemLocation.Visibility = System.Windows.Visibility.Visible;
            }
        }

        /// <summary>
        /// Handles the PreviewKeyUp event of the Grid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private void Grid_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)
            {
                this.GemLocation.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}