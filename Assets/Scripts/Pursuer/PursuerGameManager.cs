using Generation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pursuer
{
    public class PursuerGameManager : MonoBehaviour
    {
        public MapConfig[] mapConfigs;

        public static PursuerGameManager Instance;

        internal int? Seed;
        MapConfig Map;

        public bool InGame = false;

        // Start is called before the first frame update
        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            FieldOfViewSetting = new GameSetting(GameSetting.KEY_FIELD_OF_VIEW, 90);
            SensitivitySetting = new GameSetting(GameSetting.KEY_SENSITIVITY, 0.1f); 
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Map = mapConfigs[0];
        }

            

        private void Update()
        {
            if (InGame && Inputter.InputManager.CheckKeyDown(KeyName.Pause))
            {
                Paused = !Paused;
            }
        }

        #region Game Options
        public GameSetting FieldOfViewSetting;
        public GameSetting SensitivitySetting;

        #endregion

        #region Map Options
        public void SetSeed(string seed)
        {
            // if we have no seed or a blank seed, use a random one
            if (seed == null || seed == "")
            {
                Seed = null;
            }
            else if (int.TryParse(seed, out int parsedSeed))
            {
                // if the string represents an integer, the seed is that integer
                Seed = parsedSeed;
            }
            else
            {
                // otherwise, hash the string into an integer
                Seed = seed.GetHashCode();
            }
        }

        public static int GetSeed()
        {
            var randomSeed = DateTime.Now.GetHashCode();
            return Instance?.Seed.GetValueOrDefault(randomSeed) ?? randomSeed;
        }


        public List<string> GetMapTypes()
        {
            var list = new List<string>();
            foreach(MapConfig map in mapConfigs)
            {
                list.Add(map.OptionText);
            }

            return list;
        }

        public void SetMap(int index)
        {
            Map = mapConfigs[index];
        }
        #endregion

        #region Scenes
        public void LoadGameScene()
        {
            AsyncOperation sceneLoadOperation = SceneManager.LoadSceneAsync("Assets/Scenes/pursuer/Game", LoadSceneMode.Single);
            StartCoroutine(waitForSceneLoad(sceneLoadOperation, () =>
            {
                InGame = true;
                skulls = 0;
                createLevel();
            }));
        }

        public void LoadMenuScene()
        {
            cleanupEvents();
            AsyncOperation sceneLoadOperation = SceneManager.LoadSceneAsync("Assets/Scenes/pursuer/MainMenu", LoadSceneMode.Single);
            StartCoroutine(waitForSceneLoad(sceneLoadOperation, () =>
            {
                gameEnded = false;
                Paused = false;
                InGame = false;
                Seed = null;
                Map = mapConfigs[0];
            }));
        }

        void cleanupEvents()
        {
            OnPauseStateChanged = null;
            OnSkullPickedUp = null;
            OnGameOver = null;
            FieldOfViewSetting.OnChanged = null;
            SensitivitySetting.OnChanged = null;
        }
        #endregion

        #region Pausing
        static bool paused;
        public static bool Paused
        {
            get => paused;
            set
            {
                if (paused == value || gameEnded) return;
                paused = value;
                if (value)
                {
                    pause();
                }
                else
                {
                    resume();
                }
                if (Instance != null)
                {
                    Instance.OnPauseStateChanged?.Invoke(null, value);
                }
            }
        }
        public EventHandler<bool> OnPauseStateChanged;

        private static void pause()
        {
            Time.timeScale = 0;
        }

        private static void resume()
        {
            Time.timeScale = 1;
        }
        #endregion

        #region Game Logic

        public EventHandler<int> OnSkullPickedUp;

        int skulls = 0;
        public void PickUpKeyItem(KeyItem item)
        {
            if (item.name == "key_skull")
            {
                skulls++;
                OnSkullPickedUp?.Invoke(this, skulls);
            }
        }

        public EventHandler<bool> OnGameOver;

        static bool gameEnded = false;
        public void EndGame(bool didWin)
        {
            Paused = true;
            gameEnded = true;
            OnGameOver?.Invoke(this, didWin);
        }
        #endregion

        private void createLevel()
        {
            Instantiate(Map.LevelPrefab);
        }

        private IEnumerator waitForSceneLoad(AsyncOperation operation, Action onLoaded)
        {
            while(!operation.isDone)
            {
                yield return null;
            }
            onLoaded();
        }
    }
}
