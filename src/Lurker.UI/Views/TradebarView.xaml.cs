//-----------------------------------------------------------------------
// <copyright file="TradeBarView.xaml.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.Views
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for TradeBarView.xaml
    /// </summary>
    public partial class TradebarView : Window
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TradebarView"/> class.
        /// </summary>
        public TradebarView()
        {
            InitializeComponent();
            this.LocationChanged += this.TradeBarView_LocationChanged;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the LocationChanged event of the TradeBarView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void TradeBarView_LocationChanged(object sender, System.EventArgs e)
        {
            var offset = this.ItemName.HorizontalOffset;
            this.ItemName.HorizontalOffset = offset + 1;
            this.ItemName.HorizontalOffset = offset;
        }

        /// <summary>
        /// Handles the PreviewMouseWheel event of the ScrollViewer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseWheelEventArgs"/> instance containing the event data.</param>
        private void ScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;
            scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - e.Delta);
            e.Handled = true;
        }

        #endregion
    }
}
