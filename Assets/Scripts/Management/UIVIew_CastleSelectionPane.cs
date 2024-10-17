using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIVIew_CastleSelectionPane : MonoBehaviour
{
    public Image[] CastleCards;

    internal void Init(List<CastleInstance> castles)
    {
        for (int i = 0; i < castles.Count; i++)
        {
            CastleCards[i].gameObject.SetActive(true);
            CastleCards[i].sprite = castles[i].Model.Portrait;
        }
    }
}