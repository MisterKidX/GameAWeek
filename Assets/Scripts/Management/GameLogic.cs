using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GameLogic
{
    static HashSet<HeroModel> _heroesInGameplay = new(20);

    public static HeroInstance CreateRandomHeroInstanceFromCastle(CastleModel castleModel, Vector3 position, PlayerInstace holder)
    {
        var inst = castleModel.Heroes.
            Where(heroModel => !_heroesInGameplay.
            Contains(heroModel)).
            GetRandom().
            Create(position, holder);

        _heroesInGameplay.Add(inst.Model);
        holder.Heroes.Add(inst);

        return inst;
    }

    internal static CastleInstance CreateStartingCastle(PlayerInstace player, Vector3 pos)
    {
        var castleInst = player.StartingCastle.Create(pos, player);
        player.Castles.Add(castleInst);
        return castleInst;
    }
}