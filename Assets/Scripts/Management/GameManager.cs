﻿using System;
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
        LevelManager levelManager = null;
        if (LevelManager.Instance == null)
        {
            var go = new GameObject("Local Level Manager");
            levelManager = go.AddComponent<LevelManager>();
        }
        else
            levelManager = LevelManager.Instance;

        levelManager.LoadLevel(playerInstances, level);
    }
}