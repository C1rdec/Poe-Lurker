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

    #endregion
}