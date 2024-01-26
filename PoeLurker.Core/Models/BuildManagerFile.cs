using Lurker.AppData;

namespace PoeLurker.Core.Models;
internal class BuildManagerFile : AppDataFileBase<BuildManager>
{
    protected override string FileName => "Builds.json";

    protected override string FolderName => "PoeLurker";
}
