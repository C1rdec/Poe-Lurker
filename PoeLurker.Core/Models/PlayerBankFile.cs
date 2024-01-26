using Lurker.AppData;

namespace PoeLurker.Core.Models;

internal class PlayerBankFile : AppDataFileBase<PlayerBank>
{
    protected override string FileName => "Players.json";

    protected override string FolderName => "PoeLurker";

    #region Methods

    public Player GetKnownPlayer(string name)
        => Entity.Players.FirstOrDefault(p => p.Name == name);

    public Player GetExternalPlayer(string name)
        => Entity.ExternalPlayers.FirstOrDefault(p => p.Name == name);

    #endregion
}
