//-----------------------------------------------------------------------
// <copyright file="MapAffixView.xaml.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.Views
{
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for MapAffixView.xaml.
    /// </summary>
    public partial class MapAffixView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapAffixView"/> class.
        /// </summary>
        public MapAffixView()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Handles the MouseEnter event of the Help control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void Help_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Popup.IsOpen = true;
        }

        /// <summary>
        /// Handles the MouseLeave event of the Help control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void Help_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Popup.IsOpen = false;
        }
    }
}