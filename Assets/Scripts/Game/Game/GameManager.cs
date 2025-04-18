using Commons.Models;
using Commons.Models.Enums;
using Commons.Patterns;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Game {
    public class GameManager : MonoSingleton<GameManager>
    {
        public GameType lastGameType { get; private set; }
        public GameType CurrentGameType {get; private set;}
        public string DualPlayWinnerNickname { get; private set; } //닉네임 저장

        //private Canvas _canvas;
        private GameType _gameType;
    
        public GameLogic gameLogic;
        public OmokBoard omokBoard ;
        public Timer timer;
    
        public GameLogic GameLogicInstance => gameLogic;

        private bool _trackingAIState = false;
    
        public GamePopup gamePopupPrefab;
        private GamePopup popupInstance;
    
        public void SetDualPlayWinner(string nickname)
        {
            DualPlayWinnerNickname = nickname;
        }
    
        public void SetGameType(GameType gameType)
        {
            _gameType = gameType;
            CurrentGameType = gameType;
            Debug.Log($"[GameManager] 게임 타입 설정됨: {gameType}");
        }
    
        public void ShowPopup(string message, string confirmText, UnityAction onConfirm, string cancelText = null, UnityAction onCancel = null)
        {
            if (popupInstance == null)
            {
                popupInstance = Instantiate(gamePopupPrefab, FindObjectOfType<Canvas>().transform);
            }

            popupInstance.Setup(message, confirmText, onConfirm, cancelText, onCancel);
            popupInstance.OpenPopup();
        }
    
        public void SetTrackingAIState(bool state)
        {
            _trackingAIState = state;
        }
    
        public bool GetTrackingAIState()
        {
            return _trackingAIState;
        }
    
        public void StartGame(GameType gameType)
        {
            _gameType = gameType;
            lastGameType = gameType;
            CurrentGameType = gameType;
            SceneManager.LoadScene("Game"); 
            SetTrackingAIState(PlayerPrefs.GetInt("Experimental") == 1);
        }

        public GameType GetGameType() {
            return this._gameType;
        }

        public void RestartCurrentGame()
        {
            Debug.Log($"[GameManager] 이전 모드로 재시작: {lastGameType}");

            if(lastGameType != GameType.DualPlayer &&
               lastGameType != GameType.Record)
            {
                StartCoroutine(NetworkManager.Instance.GameStartRequest(callback => {
                    if (callback.Success)
                    {
                        StartGame(lastGameType);
                    }
                    else
                    {
                        Debug.Log("싱글 플레이 실패: 돈 부족@@@@");
                        if (popupInstance == null)
                        {
                            popupInstance = Instantiate(gamePopupPrefab, FindObjectOfType<Canvas>().transform);
                        }

                        popupInstance.Setup(
                            message: "돈이 부족합니다",
                            confirmText: "예",
                            confirmAction: popupInstance.ClosePopup
                        );

                        popupInstance.OpenPopup();

                    
                    }
                }));
            }

            else
            {
                StartGame(lastGameType);
            }

        }
    
        public void ChangeToGameScene(GameType gameType)
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

        // 게임씬 UI 관련 매니저
        public RecordUIManager recordUIManager;


        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "Game")
            {
                // TODO: 오목판 초기화
                //blockController.InitBlocks();

                // Game Logic 객체 생성
                if (scene.name == "Game")
                {
                    if (gameLogic != null)
                    {
                        gameLogic.Dispose();
                        Debug.Log($"_gameLogic을 삭제");
                    }

                    Debug.Log($"씬이 생성될 gameType은 : {_gameType}");

                    string player1Nick = PlayerManager.Instance.playerData.nickname;
                    string player2Nick = "상대"; // 듀얼플레이에서 쓸 고정 닉네임

                    gameLogic = new GameLogic(omokBoard, _gameType, player1Nick, player2Nick); // ✅ 한 번만 생성

                    Debug.Log($"_gameLogic이 존재함? : {gameLogic}");

                    recordUIManager.RecordUISet(_gameType == GameType.Record);
                }

                //_canvas = GameObject.FindObjectOfType<Canvas>();
            }
        }

        private void OnApplicationQuit()
        {
            gameLogic?.Dispose();
            gameLogic = null;
        }



        // 게임 시작 시 AI 레벨 설정 관련
        [SerializeField] AILevel aILevel;

        public void SetAILevel(AILevel aILevel)
        {
            this.aILevel = aILevel;
        }

        public AILevel GetAILevel()
        {
            return aILevel;
        }
    }
}