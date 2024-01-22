//-----------------------------------------------------------------------
// <copyright file="CollaborationService.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.Core.Services;

using System;
using System.Text.Json;
using System.Threading.Tasks;
using PoeLurker.Core.Models;

/// <summary>
/// Represents the collaboation Service.
/// </summary>
/// <seealso cref="PoeLurker.Core.Services.HttpServiceBase" />
public class CollaborationService : HttpServiceBase
{
    #region Fields

    private static readonly string BaseGitUrl = "https://raw.githubusercontent.com/C1rdec/Poe-Lurker/master/assets/Collaboration/";
    private static readonly string InformationFileUrl = $"{BaseGitUrl}Info";
    private static readonly string ImageFileUrl = $"{BaseGitUrl}Image";

    #endregion

    #region Methods

    /// <summary>
    /// Gets the collaboration asynchronous.
    /// </summary>
    /// <returns>The collaboration.</returns>
    public async Task<Collaboration> GetCollaborationAsync()
    {
        try
        {
            var fileContent = await GetText(InformationFileUrl);

            return JsonSerializer.Deserialize<Collaboration>(fileContent);
        }
        catch
        {
            return new Collaboration()
            {
                ExpireDate = DateTime.Now.AddDays(-1),
            };
        }
    }

    /// <summary>
    /// Gets the image asynchronous.
    /// </summary>
    /// <returns>The image content.</returns>
    public Task<byte[]> GetImageAsync()
        => GetContent(ImageFileUrl);

    #endregion
}