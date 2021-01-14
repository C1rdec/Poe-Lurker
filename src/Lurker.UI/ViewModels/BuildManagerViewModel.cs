//-----------------------------------------------------------------------
// <copyright file="BuildManagerViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Lurker.Helpers;
    using Lurker.Services;

    /// <summary>
    /// Class BuildManagerViewModel.
    /// Implements the <see cref="Caliburn.Micro.PropertyChangedBase" />
    /// </summary>
    /// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
    public class BuildManagerViewModel: Caliburn.Micro.PropertyChangedBase
    {
        #region Fields

        private ObservableCollection<BuildConfigurationViewModel> _configurations;
        private Func<string, string, Task> _showMessage;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildManagerViewModel" /> class.
        /// </summary>
        /// <param name="showMessage">The show message.</param>
        public BuildManagerViewModel(Func<string, string, Task> showMessage)
        {
            this._showMessage = showMessage;
            this._configurations = new ObservableCollection<BuildConfigurationViewModel>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the path of building code.
        /// </summary>
        /// <value>The path of building code.</value>
        public string PathOfBuildingCode { get; set; }

        /// <summary>
        /// Gets the c onfigurations.
        /// </summary>
        /// <value>The configurations.</value>
        public ObservableCollection<BuildConfigurationViewModel> Configurations
        {
            get
            {
                return this._configurations;
            }

            private set
            {
                this._configurations = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds this instance.
        /// </summary>
        public async void Add()
        {
            var text = ClipboardHelper.GetClipboardText();
            if (Uri.TryCreate(text, UriKind.Absolute, out Uri url))
            {
                var rawUri = new Uri($"https://pastebin.com/raw{url.AbsolutePath}");
                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, rawUri);
                    var response = await client.SendAsync(request);
                    text = await response.Content.ReadAsStringAsync();
                }
            }

            if (string.IsNullOrEmpty(text))
            {
                await this.ShowError();
                return;
            }

            using (var service = new PathOfBuildingService())
            {
                await service.InitializeAsync();
                try
                {
                    this.Configurations.Add(new BuildConfigurationViewModel(service.Decode(text)));
                }
                catch
                {
                    await this.ShowError();
                }
            }
        }

        /// <summary>
        /// Shows the error.
        /// </summary>
        /// <returns>Task.</returns>
        private Task ShowError()
        {
            return this._showMessage("Oups!", "You need to have a POB code in the clipboard.");
        }

        #endregion
    }
}