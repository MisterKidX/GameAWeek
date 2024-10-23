using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("GameManager");
                go.AddComponent<GameManager>();
            }
            return _instance;
        }
        private set => _instance = value;
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    internal void TransitionToGameplay(PlayerInstace[] playerInstances, string level)
    {
        StartCoroutine(TransitionToGameplayRoutine(playerInstances, level));
    }

    private IEnumerator TransitionToGameplayRoutine(PlayerInstace[] playerInstances, string level)
    {
        SceneManager.LoadScene(level);

        yield return null;

        FindFirstObjectByType<LevelManager>().LoadLevel(playerInstances, level);
    }
}