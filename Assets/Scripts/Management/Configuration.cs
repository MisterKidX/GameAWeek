using UnityEngine;

[CreateAssetMenu(menuName = "Misc/Configuration")]
public class Configuration : ScriptableObject
{
    public int MovementBarMaxMovementPoints = 30;
    public float HeroMovementSpeed = 0.35f;
}