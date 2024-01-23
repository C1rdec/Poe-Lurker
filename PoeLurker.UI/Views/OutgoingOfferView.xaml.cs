//-----------------------------------------------------------------------
// <copyright file="OutgoingOfferView.xaml.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.Views
{
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for OutgoingOfferView.xaml.
    /// </summary>
    public partial class OutgoingOfferView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutgoingOfferView"/> class.
        /// </summary>
        public OutgoingOfferView()
        {
            this.InitializeComponent();
            this.MainAction.Height = this.MainAction.Width / 2;
        }
    }
}