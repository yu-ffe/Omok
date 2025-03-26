using System;
using Commons;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public Constants.GameType lastGameType { get; private set; }

    private Canvas _canvas;
    private Constants.GameType _gameType;
    private GameLogic _gameLogic;
    
    public OmokBoard omokBoard ;
    public Timer timer;
    

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
        Debug.Log("[GameManager] ChangeToMainScene 호출됨");
        
        _gameLogic?.Dispose();
        _gameLogic = null;

        SceneTransitionManager.Instance.RegisterAfterLoadAction("Main", () =>
        {
            Debug.Log("[SceneTransition] Main 씬 로드 완료 후 초기화");

            var appStart = GameObject.FindObjectOfType<AppStart>();
            if (appStart != null)
            {
                appStart.Initialize();
                appStart.gameObject.SetActive(false);
            }
        });

        SceneTransitionManager.Instance.LoadSceneAsync("Main").Forget();
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            // 씬에 배치된 오브젝트 찾기

            if (omokBoard == null || timer == null)
            {
                Debug.Log($"null");
            }

            // TODO: 오목판 초기화
            //blockController.InitBlocks();

            // Game Logic 객체 생성
            if (_gameLogic != null) _gameLogic.Dispose();
            Debug.Log($"씬이 생성될 gameType은 : {_gameType}");
            _gameLogic = new GameLogic(_gameType);
        }

        _canvas = GameObject.FindObjectOfType<Canvas>();
    }

    private void OnApplicationQuit()
    {
        _gameLogic?.Dispose();
        _gameLogic = null;
    }
}