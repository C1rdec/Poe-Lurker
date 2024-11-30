using Lurker.AppData;

namespace PoeLurker.Core.Models;

internal class SettingsFile : AppDataFileBase<Settings>
{
    protected override string FileName => "Settings.json";

    protected override string FolderName => "PoeLurker";
}
