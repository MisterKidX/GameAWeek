using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GameLogic
{
    static HashSet<HeroModel> _heroesInGameplay = new(20);

    public static HeroInstance CreateRandomHeroInstanceFromCastle(CastleModel castleModel, Vector3Int position, PlayerInstace holder)
    {
        var unitInstances = new UnitInstance[7];
        unitInstances[0] = castleModel.Units[0].Create(Random.Range(1, 25));
        unitInstances[1] = castleModel.Units[1].Create(Random.Range(1, 10));
        unitInstances[2] = castleModel.Units[1].Create(Random.Range(1, 5));
        unitInstances[3] = castleModel.Units[2].Create(Random.Range(1, 3));

        var inst = castleModel.Heroes.
            Where(heroModel => !_heroesInGameplay.
            Contains(heroModel)).
            GetRandom().
            Create(position, holder, unitInstances);

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