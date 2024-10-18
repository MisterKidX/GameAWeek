using System;
using UnityEngine;

public class GameplayUI : MonoBehaviour
{
    public UIView_HeroSelectionPane HeroSelectionPane;
    public UIVIew_CastleSelectionPane CastleSelectionPane;
    public UIView_ResourceBar ResourceBar;

    private void ShowCastles(PlayerInstace currentPlayer)
    {
        CastleSelectionPane.Init(currentPlayer.Castles);
    }

    private void ShowHeroes(PlayerInstace currentPlayer)
    {
        HeroSelectionPane.Init(currentPlayer.Heroes);
    }

    private void ShowResources(PlayerInstace currentPlayer)
    {
        ResourceBar.Init(currentPlayer.Resources);
    }

    public void InitializePlayer(PlayerInstace currentPlayer)
    {
        ShowHeroes(currentPlayer);
        ShowCastles(currentPlayer);
        ShowResources(currentPlayer);
    }
}
