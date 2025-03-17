using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using workspace.Ham6._03_Sctipts.Game;
using Update = Unity.VisualScripting.Update;


public abstract class BasePlayerState
    {
        // 상태에 진입할 때 실행
        public abstract void OnEnter(GameLogic gameLogic);
    
        // 상태에서 벗어날 때 실행
        public abstract void OnExit(GameLogic gameLogic);
    
        // 특정 위치(row, col)에 돌을 놓는 함수
        public abstract void HandleMove(GameLogic gameLogic, int row, int col);
    
        // 다음 턴으로 전환하는 함수
        protected abstract void HandleNextTurn(GameLogic gameLogic);

        // 돌을 놓고 다음 상태로 이동하는 공통 처리 함수
        protected void ProcessMove(GameLogic gameLogic, Constants.PlayerType playerType, int row, int col)
        {
            // 돌을 정상적으로 놓았을 경우
            if (gameLogic.SetNewBoardValue(playerType, row, col))
            {
                var gameResult = gameLogic.CheckGameResult(); // 게임 결과 확인

                if (gameResult == GameLogic.GameResult.None)
                {
                    HandleNextTurn(gameLogic); // 게임이 계속 진행되면 다음 턴으로 전환
                }
                else
                {
                    gameLogic.EndGame(gameResult); // 게임이 끝났다면 종료 처리
                }
            }
        }
    }

    //직접 플레이하는 상태 (싱글 또는 멀티플레이)
    public class PlayerState : BasePlayerState
    {
        private Constants.PlayerType _playerType; // 플레이어 타입 (A 또는 B)
        private bool _isFirstPlayer; // 첫 번째 플레이어 여부
    
        /* TODO: 멀티시 구현
        private MultiplayManager _multiplayManager; // 네트워크 멀티플레이어 관리
        private string _roomId; // 멀티플레이 방 ID
        private bool _isMultiplay; // 멀티플레이 여부
        */

        //싱글 플레이어 생성자
        public PlayerState(bool isFirstPlayer)
        {
            _isFirstPlayer = isFirstPlayer;
            _playerType = _isFirstPlayer ? Constants.PlayerType.PlayerA : Constants.PlayerType.PlayerB;
            
            //TODO: 멀티시 구현
            //_isMultiplay = false;
        }

        /* TODO : 멀티 플레이시 구현
        멀티 플레이어 생성자
        public PlayerState(bool isFirstPlayer, MultiplayManager multiplayManager, string roomId)
            : this(isFirstPlayer)
        {
            _multiplayManager = multiplayManager;
            _roomId = roomId;
            _isMultiplay = true;
        }
        */

        // 상태 진입 시 실행 (플레이어 입력을 처리할 수 있도록 설정)
        public override void OnEnter(GameLogic gameLogic)
        {
            gameLogic.OmokBoard.OnOnGridClickedDelegate = (row, col) =>
            {
                HandleMove(gameLogic, row, col);
            };
        }

        // 상태 종료 시 실행 (이벤트 핸들러 해제)
        public override void OnExit(GameLogic gameLogic)
        {
            gameLogic.OmokBoard.OnOnGridClickedDelegate = null;
        }

        // 돌을 놓는 동작 처리
        public override void HandleMove(GameLogic gameLogic, int row, int col)
        {
            ProcessMove(gameLogic, _playerType, row, col);
            
            /* TODO : 멀티 플레이시 구현
            멀티플레이 상태라면 서버에 착수 정보를 전송
            if (_isMultiplay)
            {
                _multiplayManager.SendPlayerMove(_roomId, row * 3 + col);
            }
            */
        }

        // 다음 턴으로 전환
        protected override void HandleNextTurn(GameLogic gameLogic)
        {
            if (_isFirstPlayer)
            {
                gameLogic.SetState(gameLogic.secondPlayerState); // 두 번째 플레이어로 전환
            }
            else
            {
                gameLogic.SetState(gameLogic.firstPlayerState); // 첫 번째 플레이어로 전환
            }
        }
    }

    //AI 플레이 상태
    public class AIState : BasePlayerState
    {
        // AI가 자동으로 착수하는 로직
        public override void OnEnter(GameLogic gameLogic)
        {
            /*TODO: AI알고리즘 작성
            var result = MinimaxAIController.GetBestMove(gameLogic.GetBoard());

            if (result.HasValue)
            {
                HandleMove(gameLogic, result.Value.row, result.Value.col); // AI 착수
            }
            else
            {
                gameLogic.EndGame(GameLogic.GameResult.Draw); // 무승부 처리
            }
            */
        }

        public override void OnExit(GameLogic gameLogic) { }

        public override void HandleMove(GameLogic gameLogic, int row, int col)
        {
            ProcessMove(gameLogic, Constants.PlayerType.PlayerB, row, col);
        }

        protected override void HandleNextTurn(GameLogic gameLogic)
        {
            gameLogic.SetState(gameLogic.firstPlayerState); // AI 턴 후 플레이어로 변경
        }
    }

    /* TODO: 멀티시 구현
    //네트워크 플레이
    public class MultiplayState : BasePlayerState
    {
        private Constants.PlayerType _playerType;
        private bool _isFirstPlayer;
    
        private MultiplayManager _multiplayManager;

        public MultiplayState(bool isFirstPlayer, MultiplayManager multiplayManager)
        {
            _isFirstPlayer = isFirstPlayer;
            _playerType = _isFirstPlayer ? Constants.PlayerType.PlayerA : Constants.PlayerType.PlayerB;
            _multiplayManager = multiplayManager;
        }
    
        public override void OnEnter(GameLogic gameLogic)
        {
            _multiplayManager.OnOpponentMove = moveData =>
            {
                var row = moveData.position / 3;
                var col = moveData.position % 3;
                UnityThread.executeInUpdate(() =>
                {
                    HandleMove(gameLogic, row, col);                
                });
            };
        }
        */

    public class GameLogic : IDisposable
    {
    public OmokBoard OmokBoard; // 바둑판(게임판) 컨트롤러
    private Constants.PlayerType[,] _board; // 바둑판 데이터 (15x15 배열)

    //상태 패턴을 활용한 플레이어 상태 관리
    public BasePlayerState firstPlayerState; // 첫 번째 플레이어 상태
    public BasePlayerState secondPlayerState; // 두 번째 플레이어 상태
    private BasePlayerState _currentPlayerState; // 현재 플레이어 상태

    //TODO: 멀티시 구현
    //private MultiplayManager _multiplayManager; // 멀티플레이 관리 객체
    //private string _roomId; // 멀티플레이 방 ID

    //게임 결과 (승패 판정)
    public enum GameResult
    {
        None, // 게임 진행 중
        Win, // 플레이어 승
        Lose, // 플레이어 패
        Draw // 비김
    }

    //게임 로직 초기화 (싱글/멀티/AI 모드 설정)
    public GameLogic(OmokBoard OmokBoard, Constants.GameType gameType)
    {
        this.OmokBoard = OmokBoard;

        // 바둑판 배열 초기화 (15x15 크기)
        _board = new Constants.PlayerType[15, 15];

        switch (gameType)
        {
            case Constants.GameType.SinglePlayer:
            {
                firstPlayerState = new PlayerState(true); // 첫 번째 플레이어
                secondPlayerState = new AIState(); // AI 플레이어

                // 첫 번째 플레이어부터 시작
                SetState(firstPlayerState);
                break;
            }
            case Constants.GameType.DualPlayer:
            {
                firstPlayerState = new PlayerState(true);
                secondPlayerState = new PlayerState(false);
                // 게임 시작
                
                SetState(firstPlayerState);
                break;
            }
            
            /*TODO: 멀티플레이시 구현
            case Constants.GameType.MultiPlayer:
            {
                // 멀티플레이 매니저 초기화 및 상태 관리
                _multiplayManager = new MultiplayManager((state, roomId) =>
                {
                    _roomId = roomId;
                    switch (state)
                    {
                        case Constants.MultiplayManagerState.CreateRoom:
                            Debug.Log("## 방 생성 완료");
                            // TODO: 대기 화면 표시
                            break;
                        case Constants.MultiplayManagerState.JoinRoom:
                            Debug.Log("## 방 참가 완료");
                            firstPlayerState = new MultiplayState(true, _multiplayManager);
                            secondPlayerState = new GameManager.PlayerState(false, _multiplayManager, roomId);
                            SetState(firstPlayerState);
                            break;
                        case Constants.MultiplayManagerState.StartGame:
                            Debug.Log("## 게임 시작");
                            firstPlayerState = new GameManager.PlayerState(true, _multiplayManager, roomId);
                            secondPlayerState = new MultiplayState(false, _multiplayManager);
                            SetState(firstPlayerState);
                            break;
                        case Constants.MultiplayManagerState.ExitRoom:
                            Debug.Log("## 방 나가기");
                            // TODO: 방 나가기 처리
                            break;
                        case Constants.MultiplayManagerState.EndGame:
                            Debug.Log("## 게임 종료");
                            // TODO: 게임 종료 처리
                            break;
                    }
                });
                break;
            }
            */
        }
    }
    
    //현재 상태 변경 (턴 전환 시 사용)
    public void SetState(BasePlayerState state)
    {
        _currentPlayerState?.OnExit(this); // 기존 상태 종료
        _currentPlayerState = state;
        _currentPlayerState?.OnEnter(this); // 새로운 상태 진입
    }
    
    //보드에 새로운 값을 할당하는 함수
    public bool SetNewBoardValue(Constants.PlayerType playerType, int row, int col)
    {
        // 이미 돌이 있는 자리인지 확인
        if (_board[row, col] != Constants.PlayerType.None) return false;

        // 플레이어 A가 놓는 경우
        if (playerType == Constants.PlayerType.PlayerA)
        {
            _board[row, col] = playerType;
            OmokBoard.PlaceStone(playerType,Constants.StoneType.Normal, row, col); // UI에 마커 추가
            return true;
        }
        // 플레이어 B가 놓는 경우
        else if (playerType == Constants.PlayerType.PlayerB)
        {
            _board[row, col] = playerType;
            OmokBoard.PlaceStone(playerType,Constants.StoneType.Normal, row, col);
            return true;
        }
        return false;
    }

    //게임 결과 확인 함수
    public GameResult CheckGameResult()
    {
        if (CheckGameWin(Constants.PlayerType.PlayerA)) { return GameResult.Win; }
        if (CheckGameWin(Constants.PlayerType.PlayerB)) { return GameResult.Lose; }
        //TODO: 무승부 조건
        //if (MinimaxAIController.IsAllBlocksPlaced(_board)) { return GameResult.Draw; }
        
        return GameResult.None; // 게임 계속 진행
    }

    //게임의 승패를 판단하는 함수
    private bool CheckGameWin(Constants.PlayerType playerType)
    {
        //TODO : 오목 승리조건 작성

        return false; // 승리 조건 없음
    }
    
    //게임 종료 시 호출
    public void EndGame(GameResult gameResult)
    {
        SetState(null); // 상태 초기화
        firstPlayerState = null;
        secondPlayerState = null;
        //TODO: UI활성화
        //GameManager.Instance.OpenGameOverPanel(); // UI 업데이트
    }
    
    //현재 게임판 반환
    public Constants.PlayerType[,] GetBoard()
    {
        return _board;
    }

    public void Dispose()
    {
        /* TODO: 멀티시 구현
        _multiplayManager?.LeaveRoom(_roomId);
        _multiplayManager?.Dispose();
        */
    }
}