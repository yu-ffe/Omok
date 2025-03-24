using System;
using Commons;
using MyNamespace;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public Constants.GameType lastGameType { get; private set; }

    public Text timerText; // UI 타이머 텍스트
    public float timer = 30.0f; // 기본 타이머 값 
    public float currentTime = 30.0f; // 현재 남은 시간

    private Canvas _canvas;
    private Constants.GameType _gameType;
    private GameLogic _gameLogic;

    public GameLogic GameLogicInstance => _gameLogic;
    
    public void StartGame(Constants.GameType gameType)
    {
        _gameType = gameType;
        SceneManager.LoadScene("Game"); 
    }

    public void RestartCurrentGame()
    {
        Debug.Log($"[GameManager] 이전 모드로 재시작: {lastGameType}");
        StartGame(lastGameType);
    }
    
    // UI 타이머 업데이트 함수
    public void UpdateTimerUI()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(currentTime);
        timerText.text = string.Format("{0:00} : {1:000}", timeSpan.Seconds, timeSpan.Milliseconds);
    }

    public void ChangeToGameScene(Constants.GameType gameType)
    {
        _gameType = gameType;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void ChangeToMainScene()
    {
        _gameLogic?.Dispose();
        _gameLogic = null;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            // 씬에 배치된 오브젝트 찾기 (BlockContorller, GameUIController)
            var omokBoard = GameObject.FindObjectOfType<OmokBoard>();

            //_gameUIController = GameObject.FindObjectOfType<GameUIController>();

            // TODO: 오목판 초기화
            //blockController.InitBlocks();

            // Game UI 초기화
            // _gameUIController.SetGameUIMode(GameUIController.GameUIMode.Init);

            // Game Logic 객체 생성
            if (_gameLogic != null) _gameLogic.Dispose();
            Debug.Log($"씬이 생성될 gameType은 : {_gameType}");
            _gameLogic = new GameLogic(omokBoard, _gameType);
        }

        _canvas = GameObject.FindObjectOfType<Canvas>();
    }

    private void OnApplicationQuit()
    {
        _gameLogic?.Dispose();
        _gameLogic = null;
    }
}