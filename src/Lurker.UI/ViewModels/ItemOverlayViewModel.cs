//-----------------------------------------------------------------------
// <copyright file="ItemOverlayViewModel.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using Lurker.Models;
    using System;

    public class ItemOverlayViewModel: Caliburn.Micro.PropertyChangedBase
    {
        #region Fields

        private Action _closeAction;
        private PoeItem _item;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemOverlayViewModel"/> class.
        /// </summary>
        public ItemOverlayViewModel(PoeItem item, Action closeAction)
        {
            this._item = item;
            this._closeAction = closeAction;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the item class.
        /// </summary>
        public ItemClass ItemClass => this._item.ItemClass;

        /// <summary>
        /// Gets the itemlevel.
        /// </summary>
        /// <value>
        /// The itemlevel.
        /// </value>
        public int ItemLevel => this._item.ItemLevel;

        #endregion

        #region Methods

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            this._closeAction?.Invoke();
        }

        #endregion
    }
}
