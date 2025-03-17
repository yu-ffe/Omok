using System;

namespace MJ
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;

    public class TimerManager : MonoBehaviour
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
        private float radius = 100f;

        public Action OnTimeout; // 시간 초과 시 호출될 콜백

        // ========== 초기화 ==========
        private void Awake()
        {
            InitTimer(); // 시작 시 초기화
        }

        private void Start()
        {
            StartTimer();
        }

        private void Update()
        {
            if (_isPaused) return;

            // 시간 흐름
            CurrentTime += Time.deltaTime;

            // 시간 초과
            if (CurrentTime >= totalTime)
            {
                CurrentTime = totalTime;
                _isPaused = true;
                UpdateUI();
                OnTimeout?.Invoke(); // 콜백 실행
                HideHandles(); // 핸들 숨김
            }
            else
            {
                UpdateUI();
            }
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
                totalTime = customTime; // 사용자 지정 시간
            }

            CurrentTime = 0;
            _isPaused = false;
            ShowHandles(); // 핸들 표시
            UpdateUI(); // 즉시 반영
        }

        public void PauseTimer() => _isPaused = true;
        public void ResumeTimer() => _isPaused = false;

        public void StopTimer()
        {
            _isPaused = true;
            CurrentTime = totalTime;
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
            fillImage.fillAmount = 1;
            timeText.text = totalTime.ToString("F0");
            HideHandles();
            
            RectTransform trailCapImage = tailCapPivot.GetChild(0).GetComponent<RectTransform>();
            trailCapImage.anchoredPosition = new Vector2(0, radius);
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
}
