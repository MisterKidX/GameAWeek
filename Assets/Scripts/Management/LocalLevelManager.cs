using System;
using System.Collections;
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

    private Tilemap _castleTilemap => _tilemaps[2];

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
        StartCoroutine(LoadLevelRoutine(playerInstances, level));
    }

    private IEnumerator LoadLevelRoutine(PlayerInstace[] playerInstances, string level)
    {
        CurrentLevel = Instance;
        SceneManager.LoadScene(level);

        yield return null;

        Players = playerInstances;

        _grid = FindObjectsByType<Grid>(FindObjectsSortMode.None)[0];
        _tilemaps = _grid.GetComponentsInChildren<Tilemap>();

        var castleEntrances = _castleTilemap.GetTilesBlock(_castleTilemap.cellBounds)
            ?.OfType<CastleEntranceTile>().ToList();

        if (castleEntrances == null|| castleEntrances.Count < Players.Length)
            Debug.LogError("All players must have at least one castle entrance which is theirs.");

        for (int i = 0; i < Players.Length; i++)
        {
            PlayerInstace player = Players[i];
            var pos = _castleTilemap.WorldToCell(castleEntrances[i].transform.GetPosition());
            var ent = ScriptableObject.CreateInstance<CastleEntranceTile>();

            ent.sprite = castleEntrances[i].sprite;
            ent.color = player.Color;
            ent.Model = player.StartingCastle;

            _castleTilemap.SetTile(pos, ent);
        }
    }
}