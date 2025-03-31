using Commons.Models;
using Commons.Models.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Game;

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
    protected void ProcessMove(GameLogic gameLogic, PlayerType playerType, int row, int col)
    {
        // 돌을 정상적으로 놓았을 경우
        if (gameLogic.SetNewBoardValue(playerType, row, col))
        {
            var gameResult = gameLogic.CheckGameResult(); // 게임 결과 확인

            if (gameResult == GameResult.None)
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
    public PlayerType _playerType; // 플레이어 타입 (A 또는 B)
    private bool _isFirstPlayer; // 첫 번째 플레이어 여부

    public PlayerType CurrentPlayerType => _playerType;

    /* TODO: 멀티시 구현
private MultiplayManager _multiplayManager; // 네트워크 멀티플레이어 관리
private string _roomId; // 멀티플레이 방 ID
private bool _isMultiplay; // 멀티플레이 여부
*/

    //싱글 플레이어 생성자
    public PlayerState(bool isFirstPlayer)
    {
        _isFirstPlayer = isFirstPlayer;
        _playerType = _isFirstPlayer ? PlayerType.PlayerA : PlayerType.PlayerB;

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
        GameManager.Instance.omokBoard.OnOnGridClickedDelegate = (row, col) => { HandleMove(gameLogic, row, col); };
    }

    // 상태 종료 시 실행 (이벤트 핸들러 해제)
    public override void OnExit(GameLogic gameLogic)
    {
        GameManager.Instance.omokBoard.OnOnGridClickedDelegate = null;
        if (PlayerType.PlayerB == gameLogic.GetCurrentPlayerType())
        {
            GameManager.Instance.omokBoard.ShowXMarker();
        }

        if (PlayerType.PlayerA == gameLogic.GetCurrentPlayerType())
        {
            GameManager.Instance.omokBoard.RemoveXmarker();
        }
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
    public override async void OnEnter(GameLogic gameLogic)
    {
        OmokAI OmokAI = new OmokAI(gameLogic.GetBoard());

        int aiTimeLevel = 3000;

        if (PlayerManager.Instance.playerData.grade > 9) // 18~10급
        {
            aiTimeLevel = 2000;
            GameManager.Instance.SetAILevel(AILevel.Easy);
        }

        else if (PlayerManager.Instance.playerData.grade > 5) // 9~6급
        {
            aiTimeLevel = 3000;
            GameManager.Instance.SetAILevel(AILevel.Middle);
        }

        else // 5~1급
        {
            aiTimeLevel = 4000;
            GameManager.Instance.SetAILevel(AILevel.Hard);
        }

        try
        {
            var move = await OmokAI.GetBestMoveAsync(gameLogic.OmokBoard, aiTimeLevel);


            if (move.Item1 >= 0 && move.Item2 >= 0)
            {
                HandleMove(gameLogic, move.Item1, move.Item2);
            }
            else
            {
                Debug.Log($"{move.Item1},{move.Item2}");
                Debug.Log("둘 수 있는 수가 없음");
                gameLogic.EndGame(GameResult.Draw); // 무승부 처리
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public override void OnExit(GameLogic gameLogic)
    {
        GameManager.Instance.omokBoard.ShowXMarker();
    }

    public override void HandleMove(GameLogic gameLogic, int row, int col)
    {
        SoundManager.Instance.PlayPlacingSound(); // 착수 효과음
        ProcessMove(gameLogic, PlayerType.PlayerB, row, col);
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
    private PlayerType _playerType;
    private bool _isFirstPlayer;

    private MultiplayManager _multiplayManager;

    public MultiplayState(bool isFirstPlayer, MultiplayManager multiplayManager)
    {
        _isFirstPlayer = isFirstPlayer;
        _playerType = _isFirstPlayer ? PlayerType.PlayerA : PlayerType.PlayerB;
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
    public string Player1Nickname { get; private set; }
    public string Player2Nickname { get; private set; }

    public OmokBoard OmokBoard;

    private PlayerType[,] _board; // 바둑판 데이터 (15x15 배열)

    //기보확인을 위한 리스트
    public List<(PlayerType player, int x, int y)> moveList = new List<(PlayerType, int, int)>();

    //상태 패턴을 활용한 플레이어 상태 관리
    public BasePlayerState firstPlayerState; // 첫 번째 플레이어 상태
    public BasePlayerState secondPlayerState; // 두 번째 플레이어 상태
    private BasePlayerState _currentPlayerState; // 현재 플레이어 상태

    //TODO: 멀티시 구현
    //private MultiplayManager _multiplayManager; // 멀티플레이 관리 객체
    //private string _roomId; // 멀티플레이 방 ID


    //게임 로직 초기화 (싱글/멀티/AI 모드 설정)
    public GameLogic(OmokBoard omokBoard, GameType gameType,
        string player1Nickname = "Player1", string player2Nickname = "Player2")
    {
        this.OmokBoard = omokBoard;
        Player1Nickname = player1Nickname;
        Player2Nickname = player2Nickname;

        // 바둑판 배열 초기화 (15x15 크기)
        _board = new PlayerType[15, 15];

        switch (gameType)
        {
            case GameType.SinglePlayer:
            {
                firstPlayerState = new PlayerState(true);
                secondPlayerState = new AIState();
                SetState(firstPlayerState, true);
                break;
            }
            case GameType.DualPlayer:
            {
                firstPlayerState = new PlayerState(true);
                secondPlayerState = new PlayerState(false);
                SetState(firstPlayerState, true);
                break;
            }


            /*TODO: 멀티플레이시 구현
            case GameType.MultiPlayer:
            {
                // 멀티플레이 매니저 초기화 및 상태 관리
                _multiplayManager = new MultiplayManager((state, roomId) =>
                {
                    _roomId = roomId;
                    switch (state)
                    {
                        case MultiplayManagerState.CreateRoom:
                            Debug.Log("## 방 생성 완료");
                            // TODO: 대기 화면 표시
                            break;
                        case MultiplayManagerState.JoinRoom:
                            Debug.Log("## 방 참가 완료");
                            firstPlayerState = new MultiplayState(true, _multiplayManager);
                            secondPlayerState = new GameManager.PlayerState(false, _multiplayManager, roomId);
                            SetState(firstPlayerState);
                            break;
                        case MultiplayManagerState.StartGame:
                            Debug.Log("## 게임 시작");
                            firstPlayerState = new GameManager.PlayerState(true, _multiplayManager, roomId);
                            secondPlayerState = new MultiplayState(false, _multiplayManager);
                            SetState(firstPlayerState);
                            break;
                        case MultiplayManagerState.ExitRoom:
                            Debug.Log("## 방 나가기");
                            // TODO: 방 나가기 처리
                            break;
                        case MultiplayManagerState.EndGame:
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
    public void SetState(BasePlayerState state, bool isStart = false)
    {
        GameManager.Instance.timer.StopTimer();
        // Debug.Log($"{GetCurrentPlayerType()}의 턴 끝1");
        _currentPlayerState?.OnExit(this); // 기존 상태 종료
        // Debug.Log($"{GetCurrentPlayerType()}의 턴 끝2");

        _currentPlayerState = state;

        //TODO: 여기에 턴이 시잘할 때 쓸 함수입력
        GameManager.Instance.timer.StartTimer();

        if (!isStart)
            UI_Manager.Instance.RequestExecute("turn");

        // Debug.Log($"{GetCurrentPlayerType()}의 턴 시작3");
        _currentPlayerState?.OnEnter(this); // 새로운 상태 진입
        // Debug.Log($"{GetCurrentPlayerType()}의 턴 시작4");
        //TODO: 여기에 턴이 끝날 때 쓸 함수입력
    }

    public PlayerType GetCurrentPlayerType()
    {
        if (_currentPlayerState is PlayerState playerState)
        {
            return playerState.CurrentPlayerType;
        }

        // AIState나 다른 상태인 경우 별도의 처리 필요하다면 여기서 처리
        return PlayerType.None;
    }

    //보드가 비워져잇는지 체크하는 함수
    public bool IsCellEmpty(int row, int col)
    {
        return _board[row, col] == PlayerType.None;
    }

    //보드에 새로운 값을 할당하는 함수
    public bool SetNewBoardValue(PlayerType playerType, int row, int col)
    {
        // 이미 돌이 있는 자리인지 확인
        if (_board[row, col] != PlayerType.None) return false;

        // 플레이어 A가 놓는 경우
        if (playerType == PlayerType.PlayerA)
        {
            _board[row, col] = playerType;
            Debug.Log($"{GetCurrentPlayerType()}의 턴{row},{col}에 보드상에 흑돌{playerType} 기입");
            //기보저장
            moveList.Add((playerType, row, col));
            GameManager.Instance.omokBoard.PlaceStone(playerType, row, col); // UI에 마커 추가

            GameManager.Instance.omokBoard.ShowLastStone(); // 마지막 돌 표시

            return true;
        }
        // 플레이어 B가 놓는 경우
        else if (playerType == PlayerType.PlayerB)
        {
            _board[row, col] = playerType;

            Debug.Log($"{GetCurrentPlayerType()}의 턴 {row},{col}에 보드상에 백돌{playerType} 기입");
            //기보저장
            moveList.Add((playerType, row, col));
            GameManager.Instance.omokBoard.PlaceStone(playerType, row, col); // UI에 마커 추가

            GameManager.Instance.omokBoard.ShowLastStone(); // 마지막 돌 표시

            return true;
        }

        return false;
    }

    //게임 결과 확인 함수
    public GameResult CheckGameResult()
    {
        if (CheckGameWin(PlayerType.PlayerA))
        {
            Debug.Log($"{PlayerType.PlayerA} 승리");

            return GameManager.Instance.CurrentGameType == GameType.DualPlayer
                ? GameResult.Player1Win
                : GameResult.Win;
        }

        if (CheckGameWin(PlayerType.PlayerB))
        {
            Debug.Log($"{PlayerType.PlayerB} 승리");

            return GameManager.Instance.CurrentGameType == GameType.DualPlayer
                ? GameResult.Player2Win
                : GameResult.Lose;
        }

        // TODO: 무승부 처리 추가
        return GameResult.None;
    }

    //게임의 승패를 판단하는 함수
    private bool CheckGameWin(PlayerType playerType)
    {
        int[][] directions = new int[][]
        {
            new int[] { 0, 1 }, new int[] { 1, 0 }, new int[] { 1, 1 }, new int[] { 1, -1 }
        };

        // 보드의 모든 셀 순회 (BOARD_SIZE는 15)
        for (int r = 0; r < GameManager.Instance.omokBoard.gridSize; r++)
        {
            for (int c = 0; c < GameManager.Instance.omokBoard.gridSize; c++)
            {
                PlayerType cell = _board[r, c];
                // 비어 있는 셀은 건너뜁니다.
                if (cell == PlayerType.None) continue;

                // 4가지 방향에 대해 연속된 돌의 개수 평가
                foreach (var dir in directions)
                {
                    // 시작점인지 확인 (이전에 같은 돌이 있다면 이미 계산된 것으로 간주)
                    int prevR = r - dir[0];
                    int prevC = c - dir[1];
                    if (prevR >= 0 && prevR < GameManager.Instance.omokBoard.gridSize && prevC >= 0 &&
                        prevC < GameManager.Instance.omokBoard.gridSize &&
                        _board[prevR, prevC] == cell)
                        continue;

                    int count = 1; // 현재 셀 포함
                    int nr = r + dir[0];
                    int nc = c + dir[1];
                    while (nr >= 0 && nr < GameManager.Instance.omokBoard.gridSize && nc >= 0 &&
                           nc < GameManager.Instance.omokBoard.gridSize && _board[nr, nc] == cell && cell == playerType)
                    {
                        count++;
                        nr += dir[0];
                        nc += dir[1];
                    }

                    // 5개 이상의 연속 돌이 있으면 즉시 승리 판단
                    if (count >= 5)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }


    //현재 플레이어를 패배하게 하는 함수
    public void HandleCurrentPlayerDefeat(PlayerType playerType)
    {
        if (_isGameEnded) return;

        // DualPlayer → 팝업으로 처리하고 Draw 결과만 넘김
        if (GameManager.Instance.CurrentGameType == GameType.DualPlayer)
        {
            GameResult result = playerType == PlayerType.PlayerA
                ? GameResult.Player2Win
                : GameResult.Player1Win;

            GameEndManager.Instance.ShowGameEndPanel("");
            GameEndManager.Instance.PrepareGameEndInfo(result);
            return;
        }

        // 나머지 모드는 실제 승패 처리
        if (playerType == PlayerType.PlayerA)
        {
            Debug.Log($"{playerType} 패배 → EndGame(Lose)");
            EndGame(GameResult.Lose);
        }
        else
        {
            Debug.Log($"{playerType} 패배 → EndGame(Win)");
            EndGame(GameResult.Win);
        }
    }

    private bool _isGameEnded = false;
    public bool IsGameEnded => _isGameEnded;

    //게임 종료 시 호출
    public async Task EndGame(GameResult gameResult)
    {
        GameManager.Instance.timer.StopTimer();
        
        
        if (_isGameEnded) return;
        _isGameEnded = true;

        Debug.Log($"게임끝 게임결과 : {gameResult}");

        CleanupAfterGameEnd(); // 종료 시 클린업 추가

        GameRecorder.GameResultSave(gameResult); // 결과 임시 저장
        NetworkManager.Instance.GameEndSendForm(gameResult);

        SetState(null); // 상태 초기화
        firstPlayerState = null;
        secondPlayerState = null;
        
        await Task.Delay(1500);

        UI_Manager.Instance.Show(UI_Manager.PanelType.GameEnd);
        GameEndManager.Instance.Show();

        // 바로 실행 안 하고 GameEndManager에 맡긴다
        GameEndManager.Instance?.PrepareGameEndInfo(gameResult);

        //TODO: 서버에 승리 정보 전송
        //TODO: 이 부분 봇과의 대전도 서버로 전송?
        //TODO: UI활성화
        //GameManager.Instance.OpenGameOverPanel(); // UI 업데이트
    }

    /// <summary>
    /// 겡임 종료 시 상태 초기화
    /// </summary>
    private void CleanupAfterGameEnd()
    {
        GameManager.Instance.timer.StopTimer();

        // 입력 이벤트 제거
        GameManager.Instance.omokBoard.OnOnGridClickedDelegate = null;

        // 마커 제거 등 추가 클린업
        GameManager.Instance.omokBoard.RemoveXmarker();

        // 필요한 경우 다른 핸들러, 콜백, Coroutine도 여기서 정리
    }

    //현재 게임판 반환
    public PlayerType[,] GetBoard()
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