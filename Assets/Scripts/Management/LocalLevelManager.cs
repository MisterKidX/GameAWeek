using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class LocalLevelManager : MonoBehaviour
{
    private static LocalLevelManager _instance;
    public static LocalLevelManager Instance
    {
        get { return _instance; }
        private set => _instance = value;
    }

    public static LocalLevelManager CurrentLevel { get; private set; }

    public PlayerInstace[] Players;

    private Grid _grid;
    private Tilemap[] _tilemaps;
    private GameplayUI _gameplayUI;
    private int _turnOrder = 0;

    public PlayerInstace CurrentPlayer => Players[_turnOrder];
    private Tilemap _metadataLayer => _tilemaps[2];

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    internal void LoadLevel(PlayerInstace[] playerInstances, string level)
    {
        CurrentLevel = Instance;
        StartCoroutine(LoadLevelRoutine(playerInstances, level));
    }

    private IEnumerator LoadLevelRoutine(PlayerInstace[] playerInstances, string level)
    {
        SceneManager.LoadScene(level);

        yield return null;

        Players = playerInstances;

        _grid = FindObjectsByType<Grid>(FindObjectsSortMode.None)[0];
        _tilemaps = _grid.GetComponentsInChildren<Tilemap>();
        var asset = Resources.Load<GameObject>("Structural/GameplayUI");
        _gameplayUI = Instantiate(asset).GetComponent<GameplayUI>();

        DecompileLevel();
        PlayerturnSequence();
    }

    private void PlayerturnSequence()
    {
        _gameplayUI.InitializePlayer(CurrentPlayer);
    }

    private void DecompileLevel()
    {
        for (int i = _metadataLayer.cellBounds.xMin; i < _metadataLayer.cellBounds.xMax; i++)
        {
            for (int j = _metadataLayer.cellBounds.yMin; j < _metadataLayer.cellBounds.yMax; j++)
            {
                var pos = new Vector3Int(i, j);
                var tile = _metadataLayer.GetTile(pos);
                if (tile != null)
                    HandleTileMetaData(tile, pos);
            }
        }
    }

    #region Validation

    private void ValidateLevelSetup()
    {
        ValidateAllPlayersHaveCastles();
    }

    private void ValidateAllPlayersHaveCastles()
    {
        if (_playerSetupIndex != Players.Length)
            Debug.LogError("All players must have castles at the beginning of the game.");

        foreach (var player in Players)
        {
            if (!player.HasCastle)
            {
                Debug.LogError("All players must have castles at the beginning of the game.");
            }
        }
    }

    #endregion

    #region Metadata Handlers

    private void HandleTileMetaData(TileBase tile, Vector3Int pos)
    {
        switch (tile)
        {
            case CastleEntranceTile ce:
                HandleCastleEntrance(ce, pos);
                break;
            default:
                throw new NotImplementedException(tile.GetType() + " - was not implemented.");
        }
    }

    int _playerSetupIndex = 0;
    private void HandleCastleEntrance(CastleEntranceTile castleEntranceTile, Vector3Int pos)
    {
        var player = Players[_playerSetupIndex];
        var position = _metadataLayer.CellToWorld(pos);
        CastleInstance castleInstance = GameLogic.CreateStartingCastle(player, position);
        castleInstance.Show();

        HeroInstance heroInstance = GameLogic.CreateRandomHeroInstanceFromCastle(
            player.StartingCastle,
            pos,
            player);
        heroInstance.Show();

        _playerSetupIndex++;
    }
    #endregion
}