using UnityEngine;

public class PlayerModel : ActorModel
{
    public static PlayerInstace Create(string name, int orderInGameplay, CastleModel startingCastle, Color color)
    {
        var model = ScriptableObject.CreateInstance<PlayerModel>();
        var playerInstance = ScriptableObject.CreateInstance<PlayerInstace>();
        playerInstance.Init(model, name, orderInGameplay, startingCastle, color);
        return playerInstance;
    }
}