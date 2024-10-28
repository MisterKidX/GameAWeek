using System;
using System.Collections.Generic;
using UnityEngine;

public class UIView_HeroSelectionPane : MonoBehaviour
{
    public UIView_HeroCard[] HeroCards;

    internal void Init(List<HeroInstance> heroes)
    {
        if (heroes.Count > HeroCards.Length)
            Debug.LogError("Game does not support scrolling heroes UI");

        for (int i = 0; i < heroes.Count; i++)
        {
            HeroCards[i].Init(heroes[i]);
        }
        for (int i = heroes.Count;i < HeroCards.Length; i++)
        {
            HeroCards[i].Disable();
        }
    }
}
