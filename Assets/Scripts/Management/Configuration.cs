using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Misc/Configuration")]
public class Configuration : ScriptableObject
{
    public int MovementBarMaxMovementPoints = 30;
    public float HeroMovementSpeed = 0.35f;

    [SerializeField]
    internal StatIcon[] StatIcons;

    [field: SerializeField]
    public ResourceCost[] StartingResources { get; internal set; }
}

[Serializable]
internal struct StatIcon
{
    public Stat Stat;
    public Sprite Icon;
}