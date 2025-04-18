﻿//-----------------------------------------------------------------------
// <copyright file="SoundService.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.Core.Services;

using System;
using System.IO;
using NAudio.Wave;

/// <summary>
/// Represents the sound services.
/// </summary>
public class SoundService
{
    #region Fields

    /// <summary>
    /// The trade alert file name.
    /// </summary>
    public static readonly string TradeAlertFileName = "TradeAlert.mp3";

    /// <summary>
    /// The Item alert file name.
    /// </summary>
    public static readonly string ItemAlertFileName = "ItemAlert.mp3";

    private WaveOutEvent _currentSound;

    #endregion

    #region Methods

    /// <summary>
    /// Determines whether [has custom trade alert].
    /// </summary>
    /// <returns>
    ///   <c>true</c> if [has custom trade alert]; otherwise, <c>false</c>.
    /// </returns>
    public bool HasCustomTradeAlert()
    {
        return AssetService.Exists(TradeAlertFileName);
    }

    /// <summary>
    /// Determines whether [has custom trade alert].
    /// </summary>
    /// <returns>
    ///   <c>true</c> if [has custom trade alert]; otherwise, <c>false</c>.
    /// </returns>
    public bool HasCustomItemAlert()
    {
        return AssetService.Exists(ItemAlertFileName);
    }

    /// <summary>
    /// Plays the join hideout.
    /// </summary>
    /// <param name="volume">The volume.</param>
    public void PlayJoinHideout(float volume)
    {
        var stream = GetStream("PoeLurker.Core.Assets.JoinHideout.mp3");
        Play(stream, volume);
    }

    /// <summary>
    /// Plays the trade alert.
    /// </summary>
    /// <param name="volume">The volume.</param>
    /// <returns>The trade alert.</returns>
    public WaveOutEvent PlayTradeAlert(float volume)
    {
        if (AssetService.Exists(TradeAlertFileName))
        {
            return Play(File.OpenRead(AssetService.GetFilePath(TradeAlertFileName)), volume);
        }

        var stream = GetStream("PoeLurker.Core.Assets.TradeAlert.mp3");
        return Play(stream, volume);
    }

    /// <summary>
    /// Plays the good item.
    /// </summary>
    /// <param name="volume">The volume.</param>
    /// <returns>The good item alert.</returns>
    public WaveOutEvent PlayItemAlert(float volume)
    {
        if (AssetService.Exists(ItemAlertFileName))
        {
            return Play(File.OpenRead(AssetService.GetFilePath(ItemAlertFileName)), volume);
        }

        var stream = GetStream("PoeLurker.Core.Assets.ItemAlert.mp3");
        return Play(stream, volume);
    }

    private static Stream GetStream(string resourceName)
    {
        return System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
    }

    /// <summary>
    /// Plays the specified stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="volume">The volume.</param>
    private WaveOutEvent Play(Stream stream, float volume)
    {
        try
        {
            var waveOut = new WaveOutEvent();
            var mp3Reader = new Mp3FileReader(stream);
            waveOut.Init(mp3Reader);
            waveOut.Volume = volume;

            if (_currentSound != null)
            {
                _currentSound.Stop();
                _currentSound = null;
            }

            _currentSound = waveOut;
            waveOut.Play();

            EventHandler<StoppedEventArgs> handler = default;
            handler = (object s, StoppedEventArgs e) =>
            {
                stream.Dispose();
                mp3Reader.Dispose();
                waveOut.Dispose();
                waveOut.PlaybackStopped -= handler;
            };

            waveOut.PlaybackStopped += handler;
            return waveOut;
        }
        catch
        {
            return null;
        }
    }

    #endregion
}