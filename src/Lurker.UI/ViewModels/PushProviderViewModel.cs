//-----------------------------------------------------------------------
// <copyright file="PushProviderViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System;
    using Caliburn.Micro;
    using Lurker.Patreon.Services;

    /// <summary>
    /// Represents the provider.
    /// </summary>
    public class PushProviderViewModel : PropertyChangedBase
    {
        #region Fields

        private Func<PropertyChangedBase> _buildViewModel;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PushProviderViewModel"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="service">The service.</param>
        public PushProviderViewModel(string name, PushBulletService service)
            : this(name)
        {
            this._buildViewModel = () => new PushBulletViewModel(service);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PushProviderViewModel"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="service">The service.</param>
        public PushProviderViewModel(string name, PushHoverService service)
            : this(name)
        {
            this._buildViewModel = () => new PushoverViewModel(service);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PushProviderViewModel"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public PushProviderViewModel(string name)
        {
            this.ProviderName = name;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the provider name.
        /// </summary>
        public string ProviderName { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Build the view model.
        /// </summary>
        /// <returns>View model.</returns>
        public PropertyChangedBase GetViewModel()
        {
            return this._buildViewModel();
        }

        #endregion
    }
}