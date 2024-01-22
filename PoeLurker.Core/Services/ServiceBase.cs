//-----------------------------------------------------------------------
// <copyright file="ServiceBase.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Core.Services;

using System;
using System.IO;

/// <summary>
/// Represents the base class.
/// </summary>
public abstract class ServiceBase
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceBase"/> class.
    /// </summary>
    public ServiceBase()
    {
        if (!Directory.Exists(FolderPath))
        {
            Directory.CreateDirectory(FolderPath);
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the name of the file.
    /// </summary>
    protected abstract string FileName { get; }

    /// <summary>
    /// Gets the name of the folder.
    /// </summary>
    protected string FolderName => "PoeLurker";

    /// <summary>
    /// Gets the application data folder path.
    /// </summary>
    protected string AppDataFolderPath => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

    /// <summary>
    /// Gets the settings folder path.
    /// </summary>
    protected string FolderPath => System.IO.Path.Combine(AppDataFolderPath, FolderName);

    /// <summary>
    /// Gets the settings file path.
    /// </summary>
    protected string FilePath => System.IO.Path.Combine(FolderPath, FileName);

    #endregion
}