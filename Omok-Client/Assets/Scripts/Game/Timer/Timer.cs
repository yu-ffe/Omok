using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class Timer : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private TMP_Text timeText; 
    [SerializeField] private Image headCapImage; // 시작 
    [SerializeField] private RectTransform tailCapPivot; 
    [SerializeField] private Image fillImage; // 원형 채우기

    [Header("타이머 설정")]
    [SerializeField] private float totalTime = 30f; // 전체 시간 (기본 30초)

    public float CurrentTime { get; private set; } // 현재 진행 시간
    private bool _isPaused = true; // 일시정지 여부
    private bool _isActive = false;
    private float radius = 100f;
    
    private bool isWarningPlaying = false;
    private Coroutine warningCoroutine;

    public Action OnTimeout; // 시간 초과 시 호출될 콜백

    // ========== 초기화 ==========
    private void Awake()
    {
        InitTimer(); // 시작 시 초기화
        GameManager.Instance.timer = this;
        Debug.Log($"{GameManager.Instance.timer} 타이머");
    }

    private void Update()
    {
        if (!_isActive || _isPaused) return;

        // 시간 흐름
        CurrentTime += Time.deltaTime;
        float remaining = totalTime - CurrentTime;

        // 경고 사운드 (5초 이하로 처음 진입했을 때 반복)
        if (!isWarningPlaying && remaining <= 5f)
        {
            isWarningPlaying = true;
            warningCoroutine = StartCoroutine(PlayWarningSoundLoop());
        }

        // 시간 초과
        if (CurrentTime >= totalTime)
        {
            CurrentTime = totalTime;
            _isPaused = true;
            UpdateUI();
            
            // 경고 사운드 반복 종료
            if (warningCoroutine != null)
            {
                StopCoroutine(warningCoroutine);
                warningCoroutine = null;
            }
            isWarningPlaying = false;
            
            HideHandles(); // 핸들 숨김
            
            // 중복 종료 방지: 게임이 아직 진행 중인 경우만 타임오버 처리
            if (!GameManager.Instance.gameLogic.IsGameEnded)
            {
                OnTimeout?.Invoke();
            }
            
            //GameManager.Instance.gameLogic.HandleCurrentPlayerDefeat(GameManager.Instance.gameLogic.GetCurrentPlayerType());
        }
        else
        {
            UpdateUI();
        }
    }
    
    private IEnumerator PlayWarningSoundLoop()
    {
        while (!_isPaused && _isActive && !GameManager.Instance.gameLogic.IsGameEnded)
        {
            SoundManager.Instance.PlayTimerSound();
            Debug.Log(" 경고음 울림");
            yield return new WaitForSeconds(1f);
        }
    }
    
    private void CleanupAfterGameEnd()
    {
        GameManager.Instance.timer.StopTimer(); // StopTimer 내부에서 코루틴도 정리됨

        GameManager.Instance.omokBoard.OnOnGridClickedDelegate = null;
        GameManager.Instance.omokBoard.RemoveXmarker();
    }
    
    private void StopWarningCoroutine()
    {
        if (warningCoroutine != null)
        {
            StopCoroutine(warningCoroutine);
            warningCoroutine = null;
        }
        isWarningPlaying = false;
        
        SoundManager.Instance.StopTimerSound();
    }

    // ========== UI 갱신 ==========
    private void UpdateUI()
    {
        float remainRatio = (totalTime - CurrentTime) / totalTime;
        
        // 핸들 회전 (시계 방향)
        float angle = 360f * remainRatio;
        tailCapPivot.localRotation = Quaternion.Euler(0, 0, angle);
        
        // 원형 차감
        fillImage.fillAmount = remainRatio;

        // 텍스트
        timeText.text = Mathf.CeilToInt(totalTime - CurrentTime).ToString();
    }

    // ========== 타이머 제어 ==========
    public void StartTimer(float customTime = -1f)
    {
        if (customTime > 0)
        {
            totalTime = customTime;
        }

        CurrentTime = 0;
        _isPaused = false;
        _isActive = true;
        
        isWarningPlaying = false;
        if (warningCoroutine != null)
        {
            StopCoroutine(warningCoroutine);
            warningCoroutine = null;
        }

        ShowHandles();
        UpdateUI();
    }

    public void PauseTimer() => _isPaused = true;
    public void ResumeTimer() => _isPaused = false;

    public void StopTimer()
    {
        _isPaused = true;
        _isActive = false; //타이머 완전 중단
        
        CurrentTime = totalTime;
        
        StopWarningCoroutine(); // 사운드 중단 추가
        
        UpdateUI();
        HideHandles();
    }

    public void InitTimer(float customTime = -1f)
    {
        if (customTime > 0)
        {
            totalTime = customTime;
        }

        CurrentTime = 0;
        _isPaused = true;
        _isActive = false; // 타이머 완전 중단
        
        fillImage.fillAmount = 1;
        timeText.text = totalTime.ToString("F0");
        HideHandles();
        
        RectTransform trailCapImage = tailCapPivot.GetChild(0).GetComponent<RectTransform>();
        trailCapImage.anchoredPosition = new Vector2(0, radius);
        
        StopWarningCoroutine(); // 사운드 중단 추가
    }

    // ========== 핸들 표시/숨김 ==========
    private void ShowHandles()
    {
        headCapImage.gameObject.SetActive(true);
        tailCapPivot.gameObject.SetActive(true);
    }

    private void HideHandles()
    {
        headCapImage.gameObject.SetActive(false);
        tailCapPivot.gameObject.SetActive(false);
    }

    // ========== 남은 시간 반환 ==========
    public float GetRemainingTime()
    {
        return Mathf.Max(totalTime - CurrentTime, 0);
    }
}
