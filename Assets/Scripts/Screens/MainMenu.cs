using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public int MaxPlayers = 4;

    public Button Play;
    public RectTransform PlayerStripContainer;
    public Button AddPlayerStrip;
    public UIView_PlayerParemetersStrip p_PlayerStrip;

    private List<UIView_PlayerParemetersStrip> _strips;
    private string[] _castleOptions;

    private void Start()
    {
        Play.onClick.AddListener(PlayButtonClckedHandler);

        var castles = Resources.LoadAll<CastleModel>("");
        _castleOptions = castles.Select(c => c.Name).ToArray();
        _strips = new(MaxPlayers);

        foreach (Transform child in PlayerStripContainer)
        {
            if (child.GetComponent<UIView_PlayerParemetersStrip>())
                Destroy(child.gameObject);
        }

        // at least two players in a game
        AddPlayerParemeterStrip(false);
        AddPlayerParemeterStrip(false);

        AddPlayerStrip.onClick.RemoveAllListeners();
        AddPlayerStrip.onClick.AddListener(AddPlayerStripButtonClickedHandler);
    }

    private void PlayButtonClckedHandler()
    {
        // GameManager.Instance.TransitionToGameplay(1);
    }

    private void AddPlayerStripButtonClickedHandler() => AddPlayerParemeterStrip(true);

    private void AddPlayerParemeterStrip(bool canRemoveStrip)
    {
        Debug.Assert(_strips.Count <= MaxPlayers);

        UIView_PlayerParemetersStrip strip = Instantiate(p_PlayerStrip, PlayerStripContainer);
        strip.Init(_castleOptions, canRemoveStrip);
        strip.Remove += Strip_Remove;
        strip.transform.SetSiblingIndex(0);
        _strips.Add(strip);

        if (_strips.Count == MaxPlayers)
            AddPlayerStrip.interactable = false;

        Debug.Assert(_strips.Count <= MaxPlayers);
    }

    private void Strip_Remove(UIView_PlayerParemetersStrip strip)
    {
        _strips.Remove(strip);
        AddPlayerStrip.interactable = true;
    }
}