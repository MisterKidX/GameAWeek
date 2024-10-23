using System;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour
{
    public UIView_HeroSelectionPane HeroSelectionPane;
    public UIVIew_CastleSelectionPane CastleSelectionPane;
    public UIView_ResourceBar ResourceBar;
    public Button _passTurnButton;

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

    public void InitializePlayer(PlayerInstace currentPlayer, Action passTurn)
    {
        ShowHeroes(currentPlayer);
        ShowCastles(currentPlayer);
        ShowResources(currentPlayer);

        _passTurnButton.onClick.RemoveAllListeners();
        _passTurnButton.onClick.AddListener(() => passTurn());
    }
}
