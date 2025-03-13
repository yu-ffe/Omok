using System;
using UnityEngine;
using UnityEngine.UI;
using workspace.Ham6._03_Sctipts.Game;

public class GameManager : Singleton<GameManager>
{
    public Text timerText; // UI 타이머 텍스트
    public float timer = 30.0f; // 기본 타이머 값 
    public float currentTime = 30.0f; // 현재 남은 시간
    public bool isMyTurn = false; // 내 턴 인지 확인

    private IGameState currentState; // 현재 게임 상태

    private void Awake()
    {
        ChangeState(new ReadyState());
    }

    private void Update()
    {
        currentState?.UpdateState(this);
    }

    // 상태 변경 메서드
    public void ChangeState(IGameState newState)
    {
        currentState = newState;
        currentState.EnterState(this);
    }

    // UI 타이머 업데이트 함수
    public void UpdateTimerUI()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(currentTime);
        timerText.text = string.Format("{0:00} : {1:000}", timeSpan.Seconds, timeSpan.Milliseconds);
    }

    // 턴 변경
    public void NextTurn()
    {
        if (isMyTurn)
        {
            ChangeState(new OpponentTurnState()); // 상대 턴으로 변경
        }
        else
        {
            ChangeState(new MyTurnState()); // 내 턴으로 변경
        }
    }
}