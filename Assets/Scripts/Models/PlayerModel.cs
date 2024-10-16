using UnityEngine;

public class PlayerModel : ActorModel
{
    public PlayerInstace Create(string name, int orderInGameplay, CastleModel startingCastle, Color color)
    {
        var playerInstance = ScriptableObject.CreateInstance<PlayerInstace>();
        playerInstance.Init(this, name, orderInGameplay, startingCastle, color);
        return playerInstance;
    }
}
