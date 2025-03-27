using System;
using Commons;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public Constants.GameType lastGameType { get; private set; }

    //private Canvas _canvas;
    private Constants.GameType _gameType;
    
    public GameLogic gameLogic;
    public OmokBoard omokBoard ;
    public Timer timer;
    
    public GameLogic GameLogicInstance => gameLogic;

    private bool _trackingAIState = false;
    
    public void SetTrackingAIState(bool state)
    {
        _trackingAIState = state;
    }
    
    public bool GetTrackingAIState()
    {
        return _trackingAIState;
    }
    
    public void StartGame(Constants.GameType gameType)
    {
        _gameType = gameType;
        lastGameType = gameType;
        SceneManager.LoadScene("Game"); 
        SetTrackingAIState(PlayerPrefs.GetInt("Experimental") == 1);
    }

    public Constants.GameType GetGameType() {
        return this._gameType;
    }

    public void RestartCurrentGame()
    {
        Debug.Log($"[GameManager] 이전 모드로 재시작: {lastGameType}");
        StartGame(lastGameType);
    }
    
    public void ChangeToGameScene(Constants.GameType gameType)
    {
        _gameType = gameType;
        lastGameType = gameType;
        SetTrackingAIState(PlayerPrefs.GetInt("Experimental") == 1);
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

        gameLogic?.Dispose();
        gameLogic = null;

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

    public RecordUIManager recordUIManager;


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            // TODO: 오목판 초기화
            //blockController.InitBlocks();

            // Game Logic 객체 생성
            if (gameLogic != null)
            {
                gameLogic.Dispose();
                Debug.Log($"_gameLogic을 삭제");
            }
            Debug.Log($"씬이 생성될 gameType은 : {_gameType}");
            gameLogic = new GameLogic(omokBoard, _gameType);
            Debug.Log($"_gameLogic이 존재함? : {gameLogic}");

            recordUIManager.RecordUISet(_gameType == Constants.GameType.Record); // 기보 UI 표기
        }

        //_canvas = GameObject.FindObjectOfType<Canvas>();
    }

    private void OnApplicationQuit()
    {
        gameLogic?.Dispose();
        gameLogic = null;
    }
}