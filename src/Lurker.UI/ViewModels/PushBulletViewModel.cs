//-----------------------------------------------------------------------
// <copyright file="PushBulletViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using Lurker.Patreon.Services;

    /// <summary>
    /// The push bullet view model.
    /// </summary>
    /// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
    public class PushBulletViewModel : Caliburn.Micro.PropertyChangedBase
    {
        #region Fields

        private PushBulletService _service;
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
            this._service = service;
            if (service.Device != null && service.Device.Id != null)
            {
                this.SelectedDevice = new DeviceViewModel(service.Device);
            }

            this.Devices = new ObservableCollection<DeviceViewModel>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the service is connected.
        /// </summary>
        public bool Connected => this._service.Connected;

        /// <summary>
        /// Gets a value indicating whether the service is not connected.
        /// </summary>
        public bool NotConnected => !this._service.Connected;

        /// <summary>
        /// Gets or sets the popup selected device.
        /// </summary>
        public DeviceViewModel PopupSelectedDevice
        {
            get
            {
                return this._popupSelectedDevice;
            }

            set
            {
                this._popupSelectedDevice = value;
                this.ShowDevices = false;
                if (value != null)
                {
                    this.SelectedDevice = value.Clone();
                    this._service.SelectDevice(this.SelectedDevice.Device);
                }

                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets the selected device.
        /// </summary>
        public DeviceViewModel SelectedDevice
        {
            get
            {
                return this._selectedDevice;
            }

            set
            {
                this._selectedDevice = value;
                this.NotifyOfPropertyChange();
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
                return this._showDevices;
            }

            set
            {
                this._showDevices = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable].
        /// </summary>
        public bool Enable
        {
            get
            {
                return this._service.Enable;
            }

            set
            {
                this._service.Enable = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the thresold.
        /// </summary>
        public int Thresold
        {
            get
            {
                return this._service.Thresold;
            }

            set
            {
                this._service.Thresold = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Login to Pushbullet.
        /// </summary>
        public async void Login()
        {
            await this._service.Login();
            if (this._service.Connected)
            {
                var devices = await this._service.GetDevices();
                devices = devices.Where(d => d.Active);
                if (devices.Any() && this._service.Device.Id == null)
                {
                    var firstDevice = devices.First();
                    this._service.SelectDevice(firstDevice);
                    this.SelectedDevice = new DeviceViewModel(firstDevice);
                }

                this.NotifyOfPropertyChange(() => this.Connected);
                this.NotifyOfPropertyChange(() => this.NotConnected);
            }
        }

        /// <summary>
        /// Logout from Pushbullet.
        /// </summary>
        public void Logout()
        {
            this._service.Logout();
            this.Devices.Clear();
            this.PopupSelectedDevice = null;
            this.SelectedDevice = null;
            this.NotifyOfPropertyChange(() => this.Connected);
            this.NotifyOfPropertyChange(() => this.NotConnected);
            this.NotifyOfPropertyChange(() => this.Thresold);
            this.NotifyOfPropertyChange(() => this.Enable);
        }

        /// <summary>
        /// Sets the available devices.
        /// </summary>
        public async void GetDevices()
        {
            this.Devices.Clear();
            if (!this._service.Connected)
            {
                await this._service.Login();
                if (!this._service.Connected)
                {
                    return;
                }
            }

            var devices = await this._service.GetDevices();
            devices = devices.Where(d => d.Active);

            if (devices.Count() == 1)
            {
                this._service.SelectDevice(devices.First());
                return;
            }

            foreach (var device in devices.Where(d => d.Active))
            {
                this.Devices.Add(new DeviceViewModel(device));
            }

            this.ShowDevices = true;
        }

        #endregion
    }
}