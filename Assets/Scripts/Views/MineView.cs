using UnityEngine;

public class MineView : MonoBehaviour
{
    public PlayerInstace Holder;
    public SpriteRenderer Flag;
    public void Capture(PlayerInstace player)
    {
        Holder = player;
        if (player == null)
            Flag.color = Color.grey;
        else 
            Flag.color = player.Color;
    }
}
