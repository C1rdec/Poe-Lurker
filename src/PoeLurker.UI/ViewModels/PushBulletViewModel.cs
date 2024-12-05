//-----------------------------------------------------------------------
// <copyright file="PushBulletViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using System.Collections.ObjectModel;
using System.Linq;
using PoeLurker.Patreon.Services;

/// <summary>
/// The push bullet view model.
/// </summary>
/// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
public class PushBulletViewModel : Caliburn.Micro.PropertyChangedBase
{
    #region Fields

    private readonly PushBulletService _service;
    private DeviceViewModel _selectedDevice;
    private DeviceViewModel _popupSelectedDevice;
    private bool _showDevices;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="PushBulletViewModel" /> class.
    /// </summary>
    /// <param name="service">The service.</param>
    public PushBulletViewModel(PushBulletService service)
    {
        _service = service;
        if (service.Device != null && service.Device.Id != null)
        {
            SelectedDevice = new DeviceViewModel(service.Device);
        }

        Devices = new ObservableCollection<DeviceViewModel>();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether the service is connected.
    /// </summary>
    public bool Connected => _service.Connected;

    /// <summary>
    /// Gets a value indicating whether the service is not connected.
    /// </summary>
    public bool NotConnected => !_service.Connected;

    /// <summary>
    /// Gets or sets the popup selected device.
    /// </summary>
    public DeviceViewModel PopupSelectedDevice
    {
        get
        {
            return _popupSelectedDevice;
        }

        set
        {
            _popupSelectedDevice = value;
            ShowDevices = false;
            if (value != null)
            {
                SelectedDevice = value.Clone();
                _service.SelectDevice(SelectedDevice.Device);
            }

            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets the selected device.
    /// </summary>
    public DeviceViewModel SelectedDevice
    {
        get
        {
            return _selectedDevice;
        }

        set
        {
            _selectedDevice = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets the device list.
    /// </summary>
    public ObservableCollection<DeviceViewModel> Devices { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show devices].
    /// </summary>
    public bool ShowDevices
    {
        get
        {
            return _showDevices;
        }

        set
        {
            _showDevices = value;
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
    /// Login to Pushbullet.
    /// </summary>
    public async void Login()
    {
        await _service.CheckPledgeStatus();
        await _service.Login();
        if (_service.Connected)
        {
            var devices = await _service.GetDevices();
            devices = devices.Where(d => d.Active);
            if (devices.Any() && _service.Device?.Id == null)
            {
                var firstDevice = devices.First();
                _service.SelectDevice(firstDevice);
                SelectedDevice = new DeviceViewModel(firstDevice);
            }

            NotifyOfPropertyChange(() => Connected);
            NotifyOfPropertyChange(() => NotConnected);
        }
    }

    /// <summary>
    /// Send a Test.
    /// </summary>
    public async void Test()
    {
        await _service.SendTestAsync();
    }

    /// <summary>
    /// Logout from Pushbullet.
    /// </summary>
    public void Logout()
    {
        _service.Logout();
        Devices.Clear();
        PopupSelectedDevice = null;
        SelectedDevice = null;
        NotifyOfPropertyChange(() => Connected);
        NotifyOfPropertyChange(() => NotConnected);
        NotifyOfPropertyChange(() => Threshold);
        NotifyOfPropertyChange(() => Enable);
    }

    /// <summary>
    /// Sets the available devices.
    /// </summary>
    public async void GetDevices()
    {
        Devices.Clear();
        if (!_service.Connected)
        {
            await _service.CheckPledgeStatus();
            if (!_service.Connected)
            {
                return;
            }
        }

        var devices = await _service.GetDevices();
        devices = devices.Where(d => d.Active);

        if (devices.Count() == 1)
        {
            _service.SelectDevice(devices.First());
            return;
        }

        foreach (var device in devices.Where(d => d.Active))
        {
            Devices.Add(new DeviceViewModel(device));
        }

        ShowDevices = true;
    }

    #endregion
}