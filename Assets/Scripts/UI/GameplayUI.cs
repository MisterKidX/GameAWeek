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
    [SerializeField]
    UIView_PlayerOutcomeModal p_playerOutcomeModal;

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

    public void ShowCastleView(CastleInstance instance)
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

    public void Refresh()
    {
        ShowHeroes(_currentPlayer);
        ShowCastles(_currentPlayer);
        ShowResources(_currentPlayer);
    }

    PlayerInstace _currentPlayer;
    public void InitializePlayer(PlayerInstace currentPlayer, Action passTurn)
    {
        _currentPlayer = currentPlayer;

        Refresh();

        _passTurnButton.onClick.RemoveAllListeners();
        _passTurnButton.onClick.AddListener(() => passTurn());
    }

    internal void ShowPlayerLostModal(PlayerInstace player)
    {
        var modal = Instantiate(p_playerOutcomeModal);
        modal.Init(player, false, null);
    }

    internal void ShowPlayerWonModal(PlayerInstace player, Action confirm)
    {
        var modal = Instantiate(p_playerOutcomeModal);
        modal.Init(player, true, confirm);
    }
}
