using Lurker.AppData;

namespace PoeLurker.Core.Models;

internal class HotkeySettingsFile : AppDataFileBase<HotkeySettings>
{
    protected override string FileName => "HotKeys.json";

    protected override string FolderName => "PoeLurker";
}
