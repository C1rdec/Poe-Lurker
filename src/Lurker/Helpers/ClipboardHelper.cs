//-----------------------------------------------------------------------
// <copyright file="ClipboardHelper.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Helpers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Lurker.Patreon.Models;
    using Lurker.Patreon.Parsers;
    using WindowsInput;

    /// <summary>
    /// Represents the Clipboard helper.
    /// </summary>
    public static class ClipboardHelper
    {
        #region Fields

        private static readonly ItemParser ItemParser = new ItemParser();

        #endregion

        #region Methods

        /// <summary>
        /// Gets the clipboard data.
        /// </summary>
        /// <returns>The clipboard text.</returns>
        public static string GetClipboardText()
        {
            return GetLegacy();
        }

        /// <summary>
        /// Clears the clipboard.
        /// </summary>
        public static void ClearClipboard()
        {
            RetryOnMainThread(() =>
            {
                 System.Windows.Clipboard.Clear();
            });
        }

        /// <summary>
        /// Gets the item in clipboard.
        /// </summary>
        /// <returns>The item in the clipboard.</returns>
        public static async Task<PoeItem> GetItemInClipboard()
        {
            try
            {
                await Simulate.Events().ClickChord(WindowsInput.Events.KeyCode.LControl, WindowsInput.Events.KeyCode.C).Invoke();
                await Task.Delay(100);
                var text = GetClipboardText();
                var item = ItemParser.Parse(text);
                return ItemParser.Parse(text);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Checks the pledge asynchronous.
        /// </summary>
        /// <returns>The task awaiter.</returns>
        public static Task CheckPledgeStatusAsync()
        {
            return ItemParser.CheckPledgeStatus();
        }

        /// <summary>
        /// Retries the on main thread.
        /// </summary>
        /// <param name="action">The action.</param>
        private static void RetryOnMainThread(Action action)
        {
            Thread thread = new Thread(() =>
            {
                var retryCount = 3;
                while (retryCount != 0)
                {
                    try
                    {
                        action();
                        break;
                    }
                    catch
                    {
                        retryCount--;
                        Thread.Sleep(200);
                    }
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
        }

        private static string GetLegacy()
        {
            var clipboardText = string.Empty;
            RetryOnMainThread(() =>
            {
                clipboardText = System.Windows.Clipboard.GetText();
            });

            if (string.IsNullOrEmpty(clipboardText))
            {
                return string.Empty;
            }

            return clipboardText;
        }

        #endregion
    }
}