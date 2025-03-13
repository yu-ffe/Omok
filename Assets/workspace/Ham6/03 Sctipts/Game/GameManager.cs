using System;
using UnityEngine;
using UnityEngine.UI;
using workspace.Ham6._03_Sctipts.Game;

public class GameManager : Singleton<GameManager>
{
    public Text timerText; // UI 타이머 텍스트
    public float timer = 30.0f; // 기본 타이머 값 
    public float currentTime = 30.0f; // 현재 남은 시간

    // UI 타이머 업데이트 함수
    public void UpdateTimerUI()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(currentTime);
        timerText.text = string.Format("{0:00} : {1:000}", timeSpan.Seconds, timeSpan.Milliseconds);
    }
}