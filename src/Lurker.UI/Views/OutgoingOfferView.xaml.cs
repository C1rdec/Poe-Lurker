//-----------------------------------------------------------------------
// <copyright file="OutgoingOfferView.xaml.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.Views
{
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for OutgoingOfferView.xaml
    /// </summary>
    public partial class OutgoingOfferView : UserControl
    {
        public OutgoingOfferView()
        {
            this.InitializeComponent();
            this.MainAction.Height = this.MainAction.Width / 2;
        }
    }
}
