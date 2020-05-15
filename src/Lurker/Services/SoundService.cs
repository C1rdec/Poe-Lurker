//-----------------------------------------------------------------------
// <copyright file="SoundService.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Services
{
    using System;
    using System.IO;
    using NAudio.Wave;

    /// <summary>
    /// Represents the sound services.
    /// </summary>
    public class SoundService
    {
        #region Methods

        /// <summary>
        /// Plays the trade alert.
        /// </summary>
        /// <param name="volume">The volume.</param>
        public void PlayTradeAlert(float volume)
        {
            var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Lurker.Assets.TradeAlert.mp3");
            this.Play(stream, volume);
        }

        /// <summary>
        /// Plays the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="volume">The volume.</param>
        private void Play(Stream stream, float volume)
        {
            try
            {
                var waveOut = new WaveOutEvent();
                var mp3Reader = new Mp3FileReader(stream);
                waveOut.Init(mp3Reader);
                waveOut.Volume = volume;
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
            }
            catch
            {
            }
        }

        #endregion
    }
}