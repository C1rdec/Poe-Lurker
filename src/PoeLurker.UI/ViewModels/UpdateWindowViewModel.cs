//-----------------------------------------------------------------------
// <copyright file="UpdateWindowViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using Caliburn.Micro;

/// <summary>
/// Represents the dedicated update window view model.
/// </summary>
public class UpdateWindowViewModel : Screen
{
    #region Fields

    private int _progress;
    private bool _isIndeterminate = true;
    private string _title = "Hold on";
    private bool _isUpToDate;
    private string _status = "Checking for updates...";

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateWindowViewModel"/> class.
    /// </summary>
    public UpdateWindowViewModel()
    {
        DisplayName = "Poe Lurker";
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the progress (0-100).
    /// </summary>
    public int Progress
    {
        get
        {
            return _progress;
        }

        private set
        {
            _progress = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets a value indicating whether the progress is indeterminate.
    /// </summary>
    public bool IsIndeterminate
    {
        get
        {
            return _isIndeterminate;
        }

        private set
        {
            _isIndeterminate = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets the status text.
    /// </summary>
    public string Status
    {
        get
        {
            return _status;
        }

        private set
        {
            _status = value;
            NotifyOfPropertyChange();
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Reports the download progress.
    /// </summary>
    /// <param name="percent">The percent (0-100).</param>
    public void ReportProgress(int percent)
    {
        IsIndeterminate = false;
        Progress = percent;
        Status = percent >= 100
            ? "Installing update, Poe Lurker will restart..."
            : $"Downloading update... {percent}%";
    }

    /// <summary>
    /// Shows that the application is up to date and runs in the background.
    /// </summary>
    public void ReportUpToDate()
    {
        IsIndeterminate = false;
        Progress = 100;
        Title = "You're up to date!";
        Status = "Poe Lurker is up to date and running in the background.";
        IsUpToDate = true;
    }

    /// <summary>
    /// Gets the title text.
    /// </summary>
    public string Title
    {
        get
        {
            return _title;
        }

        private set
        {
            _title = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets a value indicating whether the application is up to date.
    /// </summary>
    public bool IsUpToDate
    {
        get
        {
            return _isUpToDate;
        }

        private set
        {
            _isUpToDate = value;
            NotifyOfPropertyChange();
        }
    }

    #endregion
}
