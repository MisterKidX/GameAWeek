using System;
using UnityEngine;

public class GameplayUI : MonoBehaviour
{
    public UIView_HeroSelectionPane HeroSelectionPane;
    public UIVIew_CastleSelectionPane CastleSelectionPane;

    internal void ShowCastles(PlayerInstace currentPlayer)
    {
        CastleSelectionPane.Init(currentPlayer.Castles);
    }

    internal void ShowHeroes(PlayerInstace currentPlayer)
    {
        HeroSelectionPane.Init(currentPlayer.Heroes);
    }
}
