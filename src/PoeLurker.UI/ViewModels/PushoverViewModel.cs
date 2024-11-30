//-----------------------------------------------------------------------
// <copyright file="PushoverViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using System.Diagnostics;
using PoeLurker.Patreon.Services;

/// <summary>
/// The push bullet view model.
/// </summary>
/// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
public class PushoverViewModel : Caliburn.Micro.PropertyChangedBase
{
    #region Fields

    private readonly PushHoverService _service;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="PushoverViewModel" /> class.
    /// </summary>
    /// <param name="service">The service.</param>
    public PushoverViewModel(PushHoverService service)
    {
        _service = service;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the UserId.
    /// </summary>
    public string UserId
    {
        get
        {
            return _service.UserId;
        }

        set
        {
            _service.UserId = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets the Token.
    /// </summary>
    public string Token
    {
        get
        {
            return _service.Token;
        }

        set
        {
            _service.Token = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [enable].
    /// </summary>
    public bool Enable
    {
        get
        {
            return _service.Enable;
        }

        set
        {
            _service.Enable = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating the thresold.
    /// </summary>
    public int Threshold
    {
        get
        {
            return _service.Threshold;
        }

        set
        {
            _service.Threshold = value;
            NotifyOfPropertyChange();
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Send a Test.
    /// </summary>
    public async void Test()
    {
        await _service.SendTestAsync();
    }

    /// <summary>
    /// New Account.
    /// </summary>
    public void NewAccount()
    {
        Process.Start("https://pushover.net/signup");
    }

    /// <summary>
    /// Open the info.md.
    /// </summary>
    public void Info()
    {
        Process.Start("https://github.com/C1rdec/Poe-Lurker/blob/master/assets/Pushover.md");
    }

    #endregion
}