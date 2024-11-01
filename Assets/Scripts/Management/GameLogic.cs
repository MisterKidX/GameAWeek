using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class GameLogic
{
    static HashSet<HeroModel> _heroesInGameplay = new(20);

    public static HeroInstance CreateRandomHeroInstanceFromCastle(CastleModel castleModel, Vector3Int position, PlayerInstace holder)
    {
        var unitInstances = new UnitInstance[7];
        unitInstances[0] = castleModel.Units[0].Create(Random.Range(1, 25), true);
        unitInstances[1] = castleModel.Units[1].Create(Random.Range(1, 10), true);
        unitInstances[2] = castleModel.Units[1].Create(Random.Range(1, 5), true);
        unitInstances[3] = castleModel.Units[2].Create(Random.Range(1, 3), true);

        var inst = castleModel.Heroes.
            Where(heroModel => !_heroesInGameplay.
            Contains(heroModel)).
            GetRandom().
            Create(position, holder, unitInstances);

        _heroesInGameplay.Add(inst.Model);
        holder.Heroes.Add(inst);

        return inst;
    }

    public static HeroInstance CreateHeroInstanceFromCastle(HeroModel hero, CastleModel castleModel, Vector3Int position, PlayerInstace holder)
    {
        var unitInstances = new UnitInstance[7];
        unitInstances[0] = castleModel.Units[0].Create(Random.Range(1, 25), true);
        unitInstances[1] = castleModel.Units[1].Create(Random.Range(1, 10), true);
        unitInstances[2] = castleModel.Units[1].Create(Random.Range(1, 5), true);
        unitInstances[3] = castleModel.Units[2].Create(Random.Range(1, 3), true);

        var inst = hero.Create(position, holder, unitInstances);

        _heroesInGameplay.Add(inst.Model);
        holder.Heroes.Add(inst);

        return inst;
    }

    internal static UnitInstance CreateRandomUnit(int tier, CreatureCount count, Vector3 position, Vector3Int cell)
    {
        var model = GameConfig.Units.Where(u => u.Tier == tier).GetRandom();
        var creatureAmount = GetRandomFromCount(count);
        var inst = model.Create(creatureAmount, false);

        inst.WorldPosition = position;
        inst.CellPosition = cell;

        return inst;
    }

    public static int GetRandomFromCount(CreatureCount count)
    {
        switch (count)
        {
            case CreatureCount.Few:
                return Random.Range(1, (int)CreatureCount.Few);
            case CreatureCount.Several:
                return Random.Range((int)CreatureCount.Few, (int)CreatureCount.Several);
            case CreatureCount.Pack:
                return Random.Range((int)CreatureCount.Several, (int)CreatureCount.Pack);
            case CreatureCount.Lots:
                return Random.Range((int)CreatureCount.Pack, (int)CreatureCount.Lots);
            case CreatureCount.Horde:
                return Random.Range((int)CreatureCount.Lots, (int)CreatureCount.Horde);
            case CreatureCount.Throng:
                return Random.Range((int)CreatureCount.Horde, (int)CreatureCount.Throng);
            case CreatureCount.Swarm:
                return Random.Range((int)CreatureCount.Throng, (int)CreatureCount.Swarm);
            case CreatureCount.Zounds:
                return Random.Range((int)CreatureCount.Swarm, (int)CreatureCount.Zounds);
            case CreatureCount.Legion:
                return Random.Range((int)CreatureCount.Zounds, (int)CreatureCount.Legion);
            default:
                throw new NotImplementedException();
        }
    }

    internal static CastleInstance CreateStartingCastle(PlayerInstace player, Vector3Int pos)
    {
        var castleInst = player.StartingCastle.Create(pos, player);
        player.Castles.Add(castleInst);
        return castleInst;
    }

    internal static HeroModel[] GetRandomAvailableHeroes(int amount)
    {
        var ret = new HeroModel[amount];
        var available = GameConfig.Heroes.Where(h => !_heroesInGameplay.Contains(h)).ToList();

        if (available.Count < amount)
            throw new InvalidOperationException("No more available heroes for purchase, please create more!");

        return available.GetRandoms(amount);
    }

    public static void ChangeCursor(CursorIcon icon)
    {
        var config = GameConfig.Configuration.Cursors.First(c => c.Type == icon);
        Cursor.SetCursor(config.View, config.hotSpot, CursorMode.Auto);
    }
}