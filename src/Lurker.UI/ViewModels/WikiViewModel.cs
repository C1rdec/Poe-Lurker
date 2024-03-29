﻿//-----------------------------------------------------------------------
// <copyright file="WikiViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Caliburn.Micro;
    using Lurker;
    using Lurker.Helpers;
    using Lurker.Models;
    using Lurker.Services;
    using PoeLurker.Patreon.Models;
    using PoeLurker.Patreon.Services;
    using Winook;

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
        private KeyboardLurker _keyboardLurker;
        private ClientLurker _clientLurker;
        private PoeNinjaService _ninjaService;
        private bool _visible;
        private DivineRatioViewModel _divineRatioViewModel;
        private IEnumerable<UniqueItem> _uniques;
        private string _currentLeague;

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
        /// <param name="mouseLurker">The mouse lurker.</param>
        /// <param name="keyboardLurker">The keyboard lurker.</param>
        /// <param name="ninjaService">The ninja service.</param>
        /// <param name="clientLurker">The client lurker.</param>
        public WikiViewModel(
            IWindowManager windowManager,
            DockingHelper dockingHelper,
            ProcessLurker processLurker,
            SettingsService settingsService,
            GithubService githubService,
            MouseLurker mouseLurker,
            KeyboardLurker keyboardLurker,
            PoeNinjaService ninjaService,
            ClientLurker clientLurker)
            : base(windowManager, dockingHelper, processLurker, settingsService)
        {
            this._githubService = githubService;
            this._mouseLurker = mouseLurker;
            this._keyboardLurker = keyboardLurker;
            this._ninjaService = ninjaService;
            this._clientLurker = clientLurker;
            this._currentLeague = this.SettingsService.RecentLeagueName;
            this.Items = new ObservableCollection<WikiItemBaseViewModel>();

            this._clientLurker.LeagueChanged += this.ClientLurker_LeagueChanged;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current league.
        /// </summary>
        public string CurrentLeague => this._currentLeague;

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
        /// Gets or sets a value indicating whether is Visible.
        /// </summary>
        public bool Visible
        {
            get
            {
                return this._visible;
            }

            set
            {
                this._visible = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        public ObservableCollection<WikiItemBaseViewModel> Items { get; private set; }

        /// <summary>
        /// Gets or sets the ExaltedRatio.
        /// </summary>
        public PropertyChangedBase CurrentView { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void CloseWindow()
        {
            this.SearchValue = string.Empty;
            this.CurrentView = null;
            this.NotifyOfPropertyChange(() => this.CurrentView);
            this.Visible = false;
            this.DockingHelper.SetForeground();
        }

        /// <summary>
        /// Show the instance.
        /// </summary>
        /// <returns>The task.</returns>
        public async Task Show()
        {
            var clipboardTask = ClipboardHelper.GetItemInClipboard();
            this.Visible = true;
            this.SetInForeground();
            var clipboardItem = await clipboardTask;
            ClipboardHelper.ClearClipboard();
            if (clipboardItem != null && clipboardItem.Rarity == Rarity.Unique)
            {
                var item = this._uniques.FirstOrDefault(u => u.Name == clipboardItem.BaseType);
                if (item != null)
                {
                    this.SearchValue = item.Name;
                    this.OnItemClick(item);
                }
            }
            else
            {
                await this.SetDivineRatio();
            }
        }

        /// <summary>
        /// On keydown.
        /// </summary>
        /// <param name="e">The event.</param>
        public void OnKeyDown(KeyEventArgs e)
        {
            if (this._keyboardLurker.OpenWikiHotkey == null)
            {
                return;
            }

            var keyValue = e.Key.ToString();
            if (Enum.TryParse<KeyCode>(keyValue, out var keyCode))
            {
                if (keyCode == this._keyboardLurker.OpenWikiHotkey.KeyCode)
                {
                    this.CloseWindow();
                }
            }
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
        protected async override void OnActivate()
        {
            this._uniques = await this._githubService.Uniques();
            base.OnActivate();
            this.SetInForeground();
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

            var height = windowInformation.Height / 4;
            var overlayWidth = windowInformation.Width - (windowInformation.FlaskBarWidth * 2);
            Execute.OnUIThread(() =>
            {
                this.View.Height = this.ApplyAbsoluteScalingY(height);
                this.View.Width = this.ApplyAbsoluteScalingX(overlayWidth);
                this.View.Left = this.ApplyScalingX(windowInformation.Position.Left + windowInformation.FlaskBarWidth + Margin);
                this.View.Top = this.ApplyScalingY(windowInformation.Position.Bottom - height - windowInformation.ExpBarHeight + Margin);
            });
        }

        private void ClientLurker_LeagueChanged(object sender, string leagueName)
        {
            this._currentLeague = leagueName;
            this.NotifyOfPropertyChange(() => this.CurrentLeague);
        }

        private void MouseLurker_MouseLeftButtonUp(object sender, System.EventArgs e)
        {
            this.CloseWindow();
        }

        private void Search(string value)
        {
            this.CurrentView = this._divineRatioViewModel;
            this.NotifyOfPropertyChange(() => this.CurrentView);
            this.Items.Clear();
            if (string.IsNullOrEmpty(value))
            {
                this._divineRatioViewModel?.SetFraction(null);

                return;
            }

            if (double.TryParse(value, out var fraction))
            {
                this._divineRatioViewModel?.SetFraction(fraction);

                return;
            }

            var items = this._githubService.Search(value);

            foreach (var item in items)
            {
                switch (item)
                {
                    case Gem gem:
                        this.Items.Add(new GemViewModel(gem));
                        break;
                    case UniqueItem uniqueItem:
                        this.Items.Add(new UniqueItemViewModel(uniqueItem, this.OnItemClick));
                        break;
                    default:
                        break;
                }
            }

            if (this.Items.Any())
            {
                this.CurrentView = null;
                this.NotifyOfPropertyChange(() => this.CurrentView);
            }

            if (items.Count() == 1)
            {
                var firstItem = this.Items.FirstOrDefault();
                var unique = firstItem as UniqueItemViewModel;
                if (unique != null)
                {
                    this.OnItemClick(unique.Item);
                }
            }
        }

        private async void OnItemClick(UniqueItem item)
        {
            var itemLine = await this._ninjaService.GetItemAsync(item.Name, this.SettingsService.RecentLeagueName);
            if (itemLine == null)
            {
                return;
            }

            this.CurrentView = new ItemChartViewModel(itemLine, item);
            this.NotifyOfPropertyChange(() => this.CurrentView);
        }

        private async Task SetDivineRatio()
        {
            if (string.IsNullOrEmpty(this.SettingsService.RecentLeagueName))
            {
                return;
            }

            var line = await this._ninjaService.GetDivineRationAsync(this.SettingsService.RecentLeagueName);
            if (line != null && line.ChaosEquivalent != 0)
            {
                this._divineRatioViewModel = new DivineRatioViewModel(line);
                this.CurrentView = this._divineRatioViewModel;
                this.NotifyOfPropertyChange(() => this.CurrentView);
            }
        }

        #endregion
    }
}