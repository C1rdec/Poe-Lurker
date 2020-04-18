//-----------------------------------------------------------------------
// <copyright file="ProcessExtension.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Extensions
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;

    public static class Extensions
    {
        [DllImport("Kernel32.dll")]
        private static extern bool QueryFullProcessImageName([In] IntPtr hProcess, [In] uint dwFlags, [Out] StringBuilder lpExeName, [In, Out] ref uint lpdwSize);

        /// <summary>
        /// Gets the name of the main module file.
        /// </summary>
        /// <param name="process">The process.</param>
        /// <param name="buffer">The buffer.</param>
        /// <returns>The module filepath.</returns>
        public static string GetMainModuleFileName(this Process process, int buffer = 1024)
        {
            var fileNameBuilder = new StringBuilder(buffer);
            uint bufferLength = (uint)fileNameBuilder.Capacity + 1;
            return QueryFullProcessImageName(process.Handle, 0, fileNameBuilder, ref bufferLength) ?
                fileNameBuilder.ToString() :
                null;
        }

        /// <summary>
        /// Gets the window handle.
        /// </summary>
        /// <param name="process">The process.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        public static IntPtr GetWindowHandle(this Process process)
        {
            process.Refresh();
            Process newProcess;

            try
            {
                do
                {
                    newProcess = Process.GetProcessesByName(process.ProcessName).FirstOrDefault();
                    Thread.Sleep(200);
                    if (process == null)
                    {
                        throw new System.InvalidOperationException();
                    }

                }
                while (newProcess.MainWindowHandle == IntPtr.Zero);
            }
            catch
            {
                return process.GetWindowHandle();
            }

            return newProcess.MainWindowHandle;
        }
    }
}
