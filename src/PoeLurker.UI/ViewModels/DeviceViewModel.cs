//-----------------------------------------------------------------------
// <copyright file="DeviceViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using PoeLurker.Patreon.Models.PushBullet;

/// <summary>
/// Represents the device view model.
/// </summary>
/// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
public class DeviceViewModel : Caliburn.Micro.PropertyChangedBase
{
    #region Fields

    private readonly Device _device;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceViewModel" /> class.
    /// </summary>
    /// <param name="device">The device.</param>
    public DeviceViewModel(Device device)
    {
        _device = device;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether gets the IsApple.
    /// </summary>
    public bool IsApple => _device.Manufacturer == "Apple";

    /// <summary>
    /// Gets a value indicating whether gets the IsGoogle.
    /// </summary>
    public bool IsGoogle => _device.Manufacturer == "Google";

    /// <summary>
    /// Gets a value indicating the device name.
    /// </summary>
    public string DeviceName => _device.Name;

    /// <summary>
    /// Gets a value indicating the device id.
    /// </summary>
    public string Id => _device.Id;

    /// <summary>
    /// Gets a value indicating whether gets the device.
    /// </summary>
    public Device Device => _device;

    #endregion

    #region Methods

    /// <summary>
    /// Opens the discord.
    /// </summary>
    /// <returns>The device view model.</returns>
    public DeviceViewModel Clone()
    {
        return new DeviceViewModel(_device);
    }

    #endregion
}