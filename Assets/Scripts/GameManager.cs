using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameManagerNamespace
{
    public enum GameStatus
    {
        MENU,
        PLAY,
        FAIL,
        SUCCESS
    }

    public class GameManager : Singleton<GameManager>
    {
        public GameStatus gameStatus;

        public List<LevelManager> levels;
        public int currentLevelIndex { get; private set; } = 0;

        [SerializeField] private Transform finishCameraTransform;

        private void Awake()
        {
            Application.targetFrameRate = 60;

            gameStatus = GameStatus.MENU;

            LoadLevel();
        }

        private void LoadLevel()
        {
            if (PlayerPrefs.HasKey("CurrentLevel"))
            {
                currentLevelIndex = PlayerPrefs.GetInt("CurrentLevel");
            }
            else
            {
                currentLevelIndex = 0;
            }

            SetCurrentLevelIndex(currentLevelIndex);

            levels.ForEach(level => level.gameObject.SetActive(levels[currentLevelIndex] == level));
        }

        public void SetCurrentLevelIndex(int index)
        {
            if (currentLevelIndex >= levels.Count)
            {
                currentLevelIndex = 0;
                PlayerPrefs.SetInt("CurrentLevel", currentLevelIndex);
            }
            else
            {
                currentLevelIndex = index;
                PlayerPrefs.SetInt("CurrentLevel", currentLevelIndex);
            }
        }

        private void OnEnable()
        {
            MyEvents.OnGameEnd.AddListener(OnGameEnd);
        }

        private void OnDisable()
        {
            MyEvents.OnGameEnd.RemoveListener(OnGameEnd);
        }

        private void OnGameEnd()
        {
            if (gameStatus == GameStatus.SUCCESS)
            {
                Camera.main.GetComponent<CameraFollow>().MoveTo(finishCameraTransform, 0.5f, 4f, null);
            }
        }

        public void StartGame()
        {
            gameStatus = GameStatus.PLAY;
            MyEvents.OnGameStart?.Invoke();
        }

        public void RestartLevel()
        {
            SceneManager.LoadScene(0);
        }

        public void NextLevel()
        {
            SetCurrentLevelIndex(currentLevelIndex + 1);
            RestartLevel();
        }

        public void Fail()
        {
            gameStatus = GameStatus.FAIL;
            UIManager.Instance.OnFail();
            MyEvents.OnGameEnd?.Invoke();
        }

        public void Success()
        {
            gameStatus = GameStatus.SUCCESS;
            UIManager.Instance.OnSuccess();
            MyEvents.OnGameEnd?.Invoke();
        }
    }
}