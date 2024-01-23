//-----------------------------------------------------------------------
// <copyright file="BuildSelectorView.xaml.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.Views
{
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for BuildSelectorView.xaml.
    /// </summary>
    public partial class BuildSelectorView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildSelectorView"/> class.
        /// </summary>
        public BuildSelectorView()
        {
            this.InitializeComponent();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}