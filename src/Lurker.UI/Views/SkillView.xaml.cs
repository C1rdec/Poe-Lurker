//-----------------------------------------------------------------------
// <copyright file="SkillView.xaml.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.Views
{
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for SkillView.xaml.
    /// </summary>
    public partial class SkillView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SkillView"/> class.
        /// </summary>
        public SkillView()
        {
            this.InitializeComponent();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;
            scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - e.Delta);
            e.Handled = true;
        }
    }
}