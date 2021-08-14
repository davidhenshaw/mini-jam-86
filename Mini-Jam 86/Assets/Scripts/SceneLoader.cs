using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System;

namespace metakazz{
    public class SceneLoader : MonoBehaviour
    {
        static SceneLoader instance;
        public static SceneLoader Instance => instance;

        public event Action GameLoaded;

        public Scene MainGame
        {
            get {return SceneManager.GetSceneByName("Game"); }
        }
        public Scene GameUI
        {
            get { return SceneManager.GetSceneByName("GameplayUI"); }
        }

        private void Awake()
        {
            if(!instance)
            {
                instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this);
            }
        }

        private void Start()
        {
            GameState.Instance.NextRoundReady += LoadGame;
        }

        private void Update()
        {
            if(Keyboard.current.rKey.wasPressedThisFrame)
            {
                LoadGame();
            }
        }

        public void LoadGame()
        {
            if(MainGame.isLoaded)
            {
                SceneManager.UnloadSceneAsync(MainGame.buildIndex);
            }
            SceneManager.LoadScene(MainGame.buildIndex, LoadSceneMode.Additive);

            if(!GameUI.isLoaded)
            {
                SceneManager.LoadScene(GameUI.buildIndex, LoadSceneMode.Additive);
            }
        }
    }
}