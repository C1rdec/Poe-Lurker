//-----------------------------------------------------------------------
// <copyright file="OutgoingbarViewModel.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using Caliburn.Micro;
    using Lurker.Helpers;
    using Lurker.UI.Helpers;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Represents the outgoing bar view model.
    /// </summary>
    /// <seealso cref="Lurker.UI.ViewModels.ScreenBase" />
    /// <seealso cref="Caliburn.Micro.IViewAware" />
    public class OutgoingbarViewModel : ScreenBase, IViewAware
    {
        #region Fields

        private ClientLurker _lurker;
        private DockingHelper _dockingHelper;
        private KeyboardHelper _keyboardHelper;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OutgoingbarViewModel"/> class.
        /// </summary>
        /// <param name="lurker">The lurker.</param>
        /// <param name="windowManager">The window manager.</param>
        public OutgoingbarViewModel(ClientLurker lurker, DockingHelper dockingHelper, PoeKeyboardHelper keyboardHelper, IWindowManager windowManager) 
            : base(windowManager)
        {
            this.Offers = new ObservableCollection<OutgoingOfferViewModel>();
            this._dockingHelper = dockingHelper;
            this._keyboardHelper = keyboardHelper;
            this._lurker = lurker;

            this._lurker.OutgoingOffer += this.Lurker_OutgoingOffer;
            this._dockingHelper.OnWindowMove += this.DockingHelper_OnWindowMove;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the offers.
        /// </summary>
        public ObservableCollection<OutgoingOfferViewModel> Offers { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Lurkers the outgoing offer.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void Lurker_OutgoingOffer(object sender, Events.OutgoingTradeEvent e)
        {
            this.Offers.Add(new OutgoingOfferViewModel(e));
        }

        /// <summary>
        /// Handles the OnWindowMove event of the DockingHelper control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void DockingHelper_OnWindowMove(object sender, System.EventArgs e)
        {
        }

        #endregion
    }
}
