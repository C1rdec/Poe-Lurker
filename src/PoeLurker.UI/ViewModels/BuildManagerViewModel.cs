//-----------------------------------------------------------------------
// <copyright file="BuildManagerViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
using PoeLurker.Core.Helpers;
using PoeLurker.Core.Services;
using PoeLurker.UI.Models;

/// <summary>
/// Class BuildManagerViewModel.
/// Implements the <see cref="Caliburn.Micro.PropertyChangedBase" />.
/// </summary>
/// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
public class BuildManagerViewModel : Caliburn.Micro.PropertyChangedBase
{
    #region Fields

    private ObservableCollection<BuildConfigurationViewModel> _configurations;
    private readonly Func<string, string, MessageDialogStyle?, Task<MessageDialogResult>> _showMessage;
    private readonly BuildManagerContext _context;
    private bool _skipOpen;
    private bool _isFlyoutOpen;
    private BuildConfigurationViewModel _selectedConfiguration;
    private readonly BuildService _buildService;
    private readonly GithubService _githubService;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildManagerViewModel" /> class.
    /// </summary>
    /// <param name="showMessage">The show message.</param>
    /// <param name="service">The service.</param>
    public BuildManagerViewModel(Func<string, string, MessageDialogStyle?, Task<MessageDialogResult>> showMessage, GithubService service)
    {
        _buildService = IoC.Get<BuildService>();
        _showMessage = showMessage;
        _configurations = new ObservableCollection<BuildConfigurationViewModel>();
        _context = new BuildManagerContext(Remove, Open);
        _githubService = service;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets a value indicating whether this instance is flyout open.
    /// </summary>
    public bool IsFlyoutOpen
    {
        get
        {
            return _isFlyoutOpen;
        }

        set
        {
            _isFlyoutOpen = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets the selected configuration.
    /// </summary>
    public BuildConfigurationViewModel SelectedConfiguration
    {
        get
        {
            return _selectedConfiguration;
        }

        set
        {
            _selectedConfiguration = value;
            NotifyOfPropertyChange();
        }
    }

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
            return _configurations;
        }

        private set
        {
            _configurations = value;
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
        if (Uri.TryCreate(text, UriKind.Absolute, out var url))
        {
            var rawUri = new Uri($"https://pastebin.com/raw{url.AbsolutePath}");
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, rawUri);
                var response = await client.SendAsync(request);
                text = await response.Content.ReadAsStringAsync();
            }
        }

        if (string.IsNullOrEmpty(text) || !PathOfBuildingService.IsValid(text))
        {
            await ShowError();
            return;
        }

        using (var service = new PathOfBuildingService())
        {
            await service.InitializeAsync(_githubService);
            try
            {
                var build = service.Decode(text);
                var simpleBuild = _buildService.AddBuild(build);
                _buildService.Save();
                Configurations.Insert(0, new BuildConfigurationViewModel(simpleBuild));
            }
            catch
            {
                await ShowError();
            }
        }
    }

    /// <summary>
    /// Populates the builds.
    /// </summary>
    public void PopulateBuilds()
    {
        Execute.OnUIThread(() => _configurations.Clear());
        foreach (var build in _buildService.Builds.OrderBy(b => b.Name))
        {
            var viewModel = new BuildConfigurationViewModel(build);
            Execute.OnUIThread(() => _configurations.Add(viewModel));
        }
    }

    /// <summary>
    /// Shows the error.
    /// </summary>
    /// <returns>Task.</returns>
    private Task ShowError()
    {
        return _showMessage("Oops!", "You need to have a POB code in the clipboard.", MessageDialogStyle.Affirmative);
    }

    /// <summary>
    /// Opens the specified configuration.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    public void Open(BuildConfigurationViewModel configuration)
    {
        if (_skipOpen)
        {
            _skipOpen = false;
            return;
        }

        IsFlyoutOpen = true;
        SelectedConfiguration = configuration;
    }

    /// <summary>
    /// Raises the Close event.
    /// </summary>
    public void OnClose()
    {
        _buildService.Save();
    }

    /// <summary>
    /// Removes the specified configuration.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    public async void Remove(BuildConfigurationViewModel configuration)
    {
        _skipOpen = true;
        var result = await _showMessage("Are you sure?", $"You are about to delete {configuration.BuildName}", MessageDialogStyle.AffirmativeAndNegative);

        if (result == MessageDialogResult.Affirmative)
        {
            Configurations.Remove(configuration);
            _buildService.RemoveBuild(configuration.Id);
            _buildService.Save();
        }
    }

    #endregion
}