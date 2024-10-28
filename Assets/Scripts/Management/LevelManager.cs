using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    CameraController _camController;
    [SerializeField]
    TraversalInteractions _traversalInteraction;
    [SerializeField]
    GameplayUI _gameplayUI;

    private const string COMBAT_SCENE_NAME = "Combat";

    private static LevelManager _instance;
    public static LevelManager Instance
    {
        get { return _instance; }
        private set => _instance = value;
    }

    public static LevelManager CurrentLevel { get; private set; }

    public PlayerInstace[] Players;

    private Grid _grid;
    private Tilemap[] _tilemaps;
    private int _turnOrder = 0;

    public PlayerInstace CurrentPlayer => Players[_turnOrder];
    private Tilemap _metadataLayer => _tilemaps[2];
    private Tilemap _interactionLayer => _tilemaps[1];
    private Tilemap _groundLayer => _tilemaps[0];

    private TimeManagement _timeManagement;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        if (CurrentLevel == null)
        {
            GUIStyle g = new GUIStyle(GUI.skin.button);
            g.fontSize = 72;

            Vector2 size = new Vector2(500, 150);
            Rect r1 = new Rect(Screen.width / 2f - size.x / 2f, Screen.height / 2f + size.y / 2f, size.x, size.y);
            Rect r2 = new Rect(Screen.width / 2f - size.x / 2f, Screen.height / 2f - size.y / 2f, size.x, size.y);

            if (GUI.Button(r1, "Quick Load", g))
            {
                LoadTestLevel();
            }
            if (GUI.Button(r2, "Quick Combat", g))
            {
                EDITORONLY_LoadCombat();
            }
        }
    }

    [ContextMenu("Quick Load")]
    private void LoadTestLevel()
    {
        if (!Application.isPlaying)
            return;

        var pInstances = new PlayerInstace[]
        {
            PlayerModel.Create("p1", 0, Resources.LoadAll<CastleModel>("")[0], Color.blue),
            PlayerModel.Create("p2", 1, Resources.LoadAll<CastleModel>("")[0], Color.green),
        };
        LoadLevel(pInstances, null);
    }

    [ContextMenu("Quick Load + Combat")]
    public void EDITORONLY_LoadCombat()
    {
        if (!Application.isPlaying)
            return;

        var pInstances = new PlayerInstace[]
        {
            PlayerModel.Create("p1", 0, Resources.LoadAll<CastleModel>("")[0], Color.blue),
            PlayerModel.Create("p2", 1, Resources.LoadAll<CastleModel>("")[0], Color.green),
        };
        LoadLevel(pInstances, null);
        EnterCombat(pInstances[0].Heroes[0], pInstances[1].Heroes[0]);
    }
#endif

    internal void LoadLevel(PlayerInstace[] playerInstances, string level)
    {
        CurrentLevel = Instance;
        _timeManagement = new();

        Players = playerInstances;

        _grid = FindObjectsByType<Grid>(FindObjectsSortMode.None)[0];
        _tilemaps = _grid.GetComponentsInChildren<Tilemap>();
        _gameplayUI.Init(
            () => _traversalInteraction.gameObject.SetActive(false),
            () => _traversalInteraction.gameObject.SetActive(true));
        _traversalInteraction.gameObject.SetActive(true);

        DecompileLevel();
        PlayerturnSequence();
    }

    public void FinishedTurn()
    {
        _turnOrder = ++_turnOrder % Players.Length;
        if (_turnOrder == 0)
            _timeManagement.RoundOver();

        _traversalInteraction.Reset();
        PlayerturnSequence();
    }

    private void PlayerturnSequence()
    {
        if (CurrentPlayer.Heroes.Count == 0 && CurrentPlayer.Castles.Count == 0)
        {
            Debug.Log($"Player {CurrentPlayer.Name} Lost!");
            return;
        }

        _gameplayUI.InitializePlayer(CurrentPlayer, FinishedTurn);

        if (CurrentPlayer.SelectedHero != null)
            _camController.PointAt(CurrentPlayer.SelectedHero.View.gameObject);
        else
            _camController.PointAt(CurrentPlayer.Castles[0].View.gameObject);

        CurrentPlayer.NewTurn();
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

        var hiddenBlocker = Resources.Load<BaseTile>("Structural/hiddenBlocker");
        for (int i = _interactionLayer.cellBounds.xMin; i < _interactionLayer.cellBounds.xMax; i++)
        {
            for (int j = _interactionLayer.cellBounds.yMin; j < _interactionLayer.cellBounds.yMax; j++)
            {
                var pos = new Vector3Int(i, j);
                var tile = _interactionLayer.GetTile(pos);
                var mine = _interactionLayer.GetTile(pos) as MineTile;
                if (mine != null)
                {
                    _metadataLayer.SetTile(pos + new Vector3Int(1, 0), hiddenBlocker);
                    _metadataLayer.SetTile(pos + new Vector3Int(-1, 0), hiddenBlocker);
                    _metadataLayer.SetTile(pos + new Vector3Int(1, 1), hiddenBlocker);
                    _metadataLayer.SetTile(pos + new Vector3Int(0, 1), hiddenBlocker);
                    _metadataLayer.SetTile(pos + new Vector3Int(-1, 1), hiddenBlocker);
                }
            }
        }
    }

    public T GetInstance<T>(Vector3Int pos)
    {
        return default(T);
    }

    public void EnlistTimeObject(ITimeableReactor timeableReactor)
    {
        _timeManagement.TimeableReactors.Add(timeableReactor, 0);
    }

    internal void EnterCombat(HeroInstance selectedHero, HeroInstance hero)
    {
        StartCoroutine(EnterCombatroutine(selectedHero, hero));
    }

    GameObject[] _root;
    private IEnumerator EnterCombatroutine(HeroInstance attacker, HeroInstance defender)
    {
        _attacker = attacker;
        _defender = defender;
        _root = SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (var go in _root)
        {
            if (go != gameObject)
                go.SetActive(false);
        }

        SceneManager.LoadScene(COMBAT_SCENE_NAME, LoadSceneMode.Additive);

        yield return null;

        var combatScene = SceneManager.GetSceneByName(COMBAT_SCENE_NAME);
        SceneManager.SetActiveScene(combatScene);

        var root = combatScene.GetRootGameObjects();
        foreach (var go in root)
        {
            if (go.TryGetComponent(out CombatManager cm))
            {
                cm.Init(attacker, defender);
                break;
            }
        }
    }

    HeroInstance _attacker;
    HeroInstance _defender;
    internal void BackFromCombat(bool attackerWon)
    {
        if (attackerWon)
            _defender.Die();
        else
            _attacker.Die();

        SceneManager.SetActiveScene(SceneManager.GetSceneByName("DemoLevel"));
        _root = SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (var go in _root)
            go.SetActive(true);

        _attacker = null;
        _defender = null;
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

internal class TimeManagement
{
    // int is last time in days it was updated
    public Dictionary<ITimeableReactor, int> TimeableReactors = new();
    private int _totalDays;

    public int TotalDays
    {
        get => _totalDays;
        private set
        {
            _totalDays = value;
            UpdateTimeableReactors();
        }
    }

    // called each round (day++)
    private void UpdateTimeableReactors()
    {
        Dictionary<ITimeableReactor, int> changes = new();
        foreach (var timeable in TimeableReactors)
        {
            var requestedUpdateTime = timeable.Value + timeable.Key.ReactionTime;
            if (requestedUpdateTime <= TotalDays)
                changes.Add(timeable.Key, TotalDays);

            timeable.Key.React(TotalDays);
        }

        foreach (var change in changes)
        {
            TimeableReactors[change.Key] = change.Value;
        }
    }

    public TimeManagement()
    {
        TotalDays = 1;
    }

    public void RoundOver()
    {
        TotalDays++;
    }
}