//-----------------------------------------------------------------------
// <copyright file="CollaborationView.xaml.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.Views
{
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Imaging;
    using PoeLurker.Core.Services;

    /// <summary>
    /// Interaction logic for CollaborationView.xaml.
    /// </summary>
    public partial class CollaborationView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollaborationView"/> class.
        /// </summary>
        public CollaborationView()
        {
            this.IsVisibleChanged += this.CollaborationView_IsVisibleChanged;
        }

        private async void CollaborationView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                using (var service = new CollaborationService())
                {
                    var content = await service.GetImageAsync();

                    var bitMap = new BitmapImage();
                    bitMap.BeginInit();
                    bitMap.StreamSource = new MemoryStream(content);
                    bitMap.EndInit();

                    this.Image.Source = bitMap;
                }
            }
        }
    }
}