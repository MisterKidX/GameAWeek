using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Misc/Configuration")]
public class Configuration : ScriptableObject
{
    public int MovementBarMaxMovementPoints = 30;
    public float HeroMovementSpeed = 0.35f;

    [SerializeField]
    internal StatIcon[] StatIcons;

    [field: SerializeField]
    public ResourceCost[] StartingResources { get; internal set; }

    [SerializeField]
    [FormerlySerializedAs("StartingResourcesTuple")]
    internal CursorIconTextureTuple[] Cursors;

    [Serializable]
    internal struct CursorIconTextureTuple
    {
        public CursorIcon Type;
        public Vector2 hotSpot;
        public Texture2D View;
    }
}

[Serializable]
internal struct StatIcon
{
    public Stat Stat;
    public Sprite Icon;
}

public enum CursorIcon
{
    Regular,
    HeroMove,
    HeroAquirable,
    EnterCombat,
    EnterCastle,
    UnitMove,
    UnitMeleeAttack,
    UnitRangedAttack
}