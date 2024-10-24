using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour
{
    [SerializeField]
    UIView_Castle p_castleView;
    [SerializeField]
    UIView_GameTime _gameTime;

    public UIView_HeroSelectionPane HeroSelectionPane;
    public UIView_CastleSelectionPane CastleSelectionPane;
    public UIView_ResourceBar ResourceBar;
    public Button _passTurnButton;

    Action _uiPanelOpen;
    Action _uiPanelClose;

    private void ShowCastles(PlayerInstace currentPlayer)
    {
        CastleSelectionPane.Init(currentPlayer.Castles, ShowCastleView);
    }

    private void ShowCastleView(CastleInstance instance)
    {
        var go = Instantiate(p_castleView);
        go.Init(instance, _uiPanelClose);
        _uiPanelOpen();
    }

    private void ShowHeroes(PlayerInstace currentPlayer)
    {
        HeroSelectionPane.Init(currentPlayer.Heroes);
    }

    private void ShowResources(PlayerInstace currentPlayer)
    {
        ResourceBar.Init(currentPlayer.Resources.ToList());
    }

    public void Init(Action UIPanelOpen, Action UIPanelClosed)
    {
        _uiPanelClose = UIPanelClosed;
        _uiPanelOpen = UIPanelOpen;
        _gameTime.Init(1);
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
