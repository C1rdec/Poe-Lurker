//-----------------------------------------------------------------------
// <copyright file="CharacterManagerView.xaml.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.Views
{
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for CharacterManagerView.xaml.
    /// </summary>
    public partial class CharacterManagerView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterManagerView"/> class.
        /// </summary>
        public CharacterManagerView()
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