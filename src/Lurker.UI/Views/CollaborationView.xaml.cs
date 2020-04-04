//-----------------------------------------------------------------------
// <copyright file="CollaborationView.xaml.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

using Lurker.Services;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Lurker.UI.Views
{
    /// <summary>
    /// Interaction logic for CollaborationView.xaml
    /// </summary>
    public partial class CollaborationView : UserControl
    {
        private static readonly string AnimationName = "SponsorAnimation.json";
        public CollaborationView()
        {
            this.InitializeComponent();
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

        /// <summary>
        /// Gets the content of the resource.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>The animation text.</returns>
        private static string GetResourceContent(string fileName)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"Lurker.UI.Assets.{fileName}"))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
