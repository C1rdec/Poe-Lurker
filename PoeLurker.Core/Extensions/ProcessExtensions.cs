//-----------------------------------------------------------------------
// <copyright file="ProcessExtensions.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.Core.Extensions;

using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

/// <summary>
/// Represents all the extension methods.
/// </summary>
public static class ProcessExtensions
{
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

        return Native.QueryFullProcessImageName(process.Handle, 0, fileNameBuilder, ref bufferLength) ?
            fileNameBuilder.ToString() :
            null;
    }

    /// <summary>
    /// Gets the window handle.
    /// </summary>
    /// <param name="process">The process.</param>
    /// <returns>The window hande pointer.</returns>
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
                    throw new InvalidOperationException();
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