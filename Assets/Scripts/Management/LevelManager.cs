using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private static LevelManager _instance;
    public static LevelManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("LevelManager");
                go.AddComponent<LevelManager>();
            }
            return _instance;
        }
        private set => _instance = value;
    }

    public static LevelManager CurrentLevel { get; private set; }

    public PlayerInstace[] Players;

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
        SceneManager.LoadScene(level);

        Players = playerInstances;
    }
}