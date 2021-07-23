//-----------------------------------------------------------------------
// <copyright file="WikiViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System.Collections.ObjectModel;
    using Caliburn.Micro;
    using Lurker;
    using Lurker.Helpers;
    using Lurker.Models;
    using Lurker.Services;

    /// <summary>
    /// Represents the wiki overlay.
    /// </summary>
    /// <seealso cref="PoeOverlayBase" />
    public class WikiViewModel : PoeOverlayBase
    {
        #region Fields

        private readonly GithubService _githubService;
        private string _searchValue = string.Empty;
        private MouseLurker _mouseLurker;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WikiViewModel" /> class.
        /// </summary>
        /// <param name="windowManager">The window manager.</param>
        /// <param name="dockingHelper">The docking helper.</param>
        /// <param name="processLurker">The process lurker.</param>
        /// <param name="settingsService">The settings service.</param>
        /// <param name="githubService">The github service.</param>
        /// <param name="mouseLurker">The mouse lurker..</param>
        public WikiViewModel(IWindowManager windowManager, DockingHelper dockingHelper, ProcessLurker processLurker, SettingsService settingsService, GithubService githubService, MouseLurker mouseLurker)
            : base(windowManager, dockingHelper, processLurker, settingsService)
        {
            this._githubService = githubService;
            this.Items = new ObservableCollection<WikiItemBaseViewModel>();
            this._mouseLurker = mouseLurker;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the search value.
        /// </summary>
        public string SearchValue
        {
            get
            {
                return this._searchValue;
            }

            set
            {
                this._searchValue = value;
                Execute.OnUIThread(() => this.Search(value));
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        public ObservableCollection<WikiItemBaseViewModel> Items { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            this.SearchValue = string.Empty;
            this.TryClose();
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        /// <param name="view">The window information.</param>
        protected override void OnViewReady(object view)
        {
            base.OnViewReady(view);
            var window = view as System.Windows.Window;
            Execute.OnUIThread(() => window.Focus());
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        protected override void OnActivate()
        {
            base.OnActivate();
            Execute.OnUIThread(() => this.View.Focus());
            this._mouseLurker.MouseLeftButtonUp += this.MouseLurker_MouseLeftButtonUp;
        }

        /// <summary>
        /// On Desactivate.
        /// </summary>
        /// <param name="close">if close.</param>
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            this._mouseLurker.MouseLeftButtonUp -= this.MouseLurker_MouseLeftButtonUp;
        }

        /// <summary>
        /// Sets the window position.
        /// </summary>
        /// <param name="windowInformation">The window information.</param>
        protected override void SetWindowPosition(PoeWindowInformation windowInformation)
        {
            if (this.View == null)
            {
                return;
            }

            Execute.OnUIThread(() =>
            {
                this.View.Height = this.ApplyScalingY(windowInformation.Height);
                this.View.Width = this.ApplyScalingX(windowInformation.Width);
                this.View.Left = windowInformation.Position.Left;
                this.View.Top = windowInformation.Position.Top;
            });
        }

        private void MouseLurker_MouseLeftButtonUp(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void Search(string value)
        {
            this.Items.Clear();
            var items = this._githubService.Search(value);

            foreach (var item in items)
            {
                switch (item)
                {
                    case Gem gem:
                        this.Items.Add(new GemViewModel(gem));
                        break;
                    case UniqueItem uniqueItem:
                        this.Items.Add(new UniqueItemViewModel(uniqueItem));
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion
    }
}