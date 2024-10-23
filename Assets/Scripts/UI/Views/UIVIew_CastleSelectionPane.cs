using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIVIew_CastleSelectionPane : MonoBehaviour
{
    public UIView_CastleBox[] CastleCards;

    internal void Init(List<CastleInstance> castles, Action<CastleInstance> castleClick)
    {
        for (int i = 0; i < castles.Count; i++)
        {
            var castle = castles[i];
            CastleCards[i].Init(castle.Model.Portrait, () => castleClick(castle));
        }
        for (int i = castles.Count; i < CastleCards.Length; i++)
        {
            CastleCards[i].Deactivate();
        }
    }
}