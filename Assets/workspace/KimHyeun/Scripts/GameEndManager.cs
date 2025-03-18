using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

namespace KimHyeun {
    public class GameEndManager : Singleton<GameEndManager>
    {
        [Header("필수 할당")]
        [SerializeField] TMP_Text resultText;
        [SerializeField] TMP_Text gradeResultText;
        [SerializeField] TMP_Text gradeMinText;
        [SerializeField] TMP_Text gradeMaxText;
        [SerializeField] RectTransform gradeBar;
        [Header("+ 영역 셀")]
        [SerializeField] GameObject gradePlusCellPrefab;
        [Header("- 영역 셀")]
        [SerializeField] GameObject gradeMinusCellPrefab;
       
        RectTransform[] gradePlusCells = new RectTransform[3];       
        RectTransform[] gradeMinusCells = new RectTransform[3];

        [Header("필수 할당 - 버튼 역할 오브젝트,텍스트")]
        [SerializeField] GameObject okButton;
        [SerializeField] TMP_Text okButtonText;
        [SerializeField] GameObject restartButton;
        [SerializeField] TMP_Text restartButtonText;
        [SerializeField] GameObject recordButton;
        [SerializeField] TMP_Text recordButtonText;

        Sequence currentSequence;

        public void SetEndGameInfo(GameResult gameResult)  // 실행 코드
        {
            // 승점바 설정
            GradeBarSetting();
            GradeCellReset();
            GradeMinMaxTextSet();

            EndButtonInfoSet(); // 버튼 설정

            SetAfterGameEnd(gameResult);
        }

        // 셀 위치 조정
        void GradeBarSetting()
        {
            if (gradeBar == null) return;

            // 기존에 존재하는 오브젝트들을 삭제
            for (int i = 0; i < gradeMinusCells.Length; i++)
            {
                if (gradeMinusCells[i] != null)
                {
                    Destroy(gradeMinusCells[i].gameObject); // 기존 셀 제거
                }
            }

            for (int i = 0; i < gradePlusCells.Length; i++)
            {
                if (gradePlusCells[i] != null)
                {
                    Destroy(gradePlusCells[i].gameObject); // 기존 셀 제거
                }
            }

            // gradeMinusCells, gradePlusCells 배열 초기화
            gradeMinusCells = new RectTransform[3];
            gradePlusCells = new RectTransform[3];

            float barWidth = gradeBar.rect.width;  // 전체 바의 가로 크기
            float barHeight = gradeBar.rect.height; // 전체 바의 세로 크기
            float cellSpacing = 10f;  // 셀 간격

            // 6개 셀을 위한 실제 가용 너비 계산
            float availableWidth = barWidth - (cellSpacing * 7); // 양 끝과 셀 사이의 7개 간격
            float cellWidth = availableWidth / 6f; // 6개 셀에 대한 개별 너비
            float cellHeight = barHeight - (cellSpacing * 2);  // 위아래 간격 포함

            // 첫 번째 셀의 시작 위치 (왼쪽 여백부터)
            float startX = -barWidth / 2 + cellSpacing;

            for (int i = 0; i < 3; i++)
            {
                GameObject newCell = Instantiate(gradeMinusCellPrefab, gradeBar.transform); // 프리팹 생성
                gradeMinusCells[i] = newCell.GetComponent<RectTransform>(); // RectTransform 할당

                newCell = Instantiate(gradePlusCellPrefab, gradeBar.transform); // 프리팹 생성
                gradePlusCells[i] = newCell.GetComponent<RectTransform>(); // RectTransform 할당
            }

            // gradeMinusCells 배열만큼 프리팹 생성하여 저장
            for (int i = 0; i < 3; i++)
            {
                // 셀 크기와 위치 설정
                gradeMinusCells[i].pivot = new Vector2(1f, 0.5f); // 우측 중앙 피봇
                gradeMinusCells[i].sizeDelta = new Vector2(cellWidth, cellHeight);

                // 2,1,0 순서로 배치
                int index = 2 - i;
                float xPos = startX + (i + 1) * cellWidth + i * cellSpacing;
                gradeMinusCells[index].anchoredPosition = new Vector2(xPos, 0);
            }

            // gradePlusCells 배열만큼 프리팹 생성하여 저장
            for (int i = 0; i < 3; i++)
            {
                // 셀 크기와 위치 설정
                gradePlusCells[i].pivot = new Vector2(0f, 0.5f); // 좌측 중앙 피봇
                gradePlusCells[i].sizeDelta = new Vector2(cellWidth, cellHeight);

                // 0,1,2 순서로 배치
                float xPos = startX + (i + 3) * cellWidth + (i + 3) * cellSpacing;
                gradePlusCells[i].anchoredPosition = new Vector2(xPos, 0);
            }
        }

        // 셀 0 초기화
        void GradeCellReset()
        {
            // gradePlusCells의 x 축 스케일을 0으로 설정
            for (int i = 0; i < gradePlusCells.Length; i++)
            {
                if (gradePlusCells[i] != null)
                {
                    gradePlusCells[i].localScale = new Vector3(0f, 1f, 1f); // x 값만 0으로 설정하여 숨김
                }
            }

            // gradeMinusCells의 x 축 스케일을 0으로 설정
            for (int i = 0; i < gradeMinusCells.Length; i++)
            {
                if (gradeMinusCells[i] != null)
                {
                    gradeMinusCells[i].localScale = new Vector3(0f, 1f, 1f); // x 값만 0으로 설정하여 숨김
                }
            }
        }

        void GradeMinMaxTextSet()
        {
            int rankPointRange = GradeChangeManager.GetRankPointRange();
            gradeMinText.text = (-rankPointRange).ToString();
            gradeMaxText.text = rankPointRange.ToString();
        }

        void EndButtonInfoSet()
        {
            EndButtonClickListenerSet(); // 버튼 클릭 이벤트 할당
            EndButtonTextSet(); // 버튼 텍스트 할당
        }

        void EndButtonClickListenerSet()
        {
            // 기존에 있는 Button 컴포넌트를 가져오고, 없으면 추가
            Button okBtn = okButton.GetComponent<Button>() ?? okButton.AddComponent<Button>();
            Button restartBtn = restartButton.GetComponent<Button>() ?? restartButton.AddComponent<Button>();
            Button recordBtn = recordButton.GetComponent<Button>() ?? recordButton.AddComponent<Button>();

            // 기존 리스너 제거 (중복 방지)
            okBtn.onClick.RemoveAllListeners();
            restartBtn.onClick.RemoveAllListeners();
            recordBtn.onClick.RemoveAllListeners();

            // 새로운 클릭 리스너 추가
            okBtn.onClick.AddListener(() => { GameEndButtonClickManager.Instance.OnClick_OkButton(); });
            restartBtn.onClick.AddListener(() => { GameEndButtonClickManager.Instance.DORestart(); });
            recordBtn.onClick.AddListener(() => { GameEndButtonClickManager.Instance.OnClick_RecordButton(); });
        }

        void EndButtonTextSet()
        {
            okButtonText.text = "확인";
            restartButtonText.text = "재대국 신청";
            recordButtonText.text = "기보 저장";
        }

        public void SetAfterGameEnd(GameResult gameResult) // 게임 종료 처리
        {
            // 현재 로그인된 유저 세션ID, 해당 세션 정보 불러오기
            UserSession userSession = SessionManager.GetSession(SessionManager.currentUserId);

            
            UpdateCellScales(userSession.RankPoint); // 이전 랭크 포인트 기준 우선 표기

            // 승패 포인트 설정
            int getPointPlusValue = GradeChangeManager.GetWinPoint(userSession.Grade);
            int getPointMinusValue = GradeChangeManager.GetLosePoint();

            switch (gameResult) 
            {
                case GameResult.Win:
                    resultText.text = "승리!\n" + getPointPlusValue + "포인트 획득";

                    // 승점 변동 애니메이션
                    RankPointSet(userSession, gameResult);

                    break;

                case GameResult.Lose:
                    resultText.text = "패배!\n" + getPointMinusValue + "포인트 손실";

                    // 승점 변동 애니메이션
                    RankPointSet(userSession, gameResult);

                    break;

                case GameResult.Draw:
                    resultText.text = "무승부!\n포인트 변동 없음";

                    // 승점 변동 애니메이션
                    RankPointSet(userSession, gameResult);

                    break;

                default:
                    resultText.text = "오류!";
                    break;
            }
        }

        /*
        void UpdateCellScales(float rankPoint, bool animate = false)
        {
            // 이전 시퀀스가 실행 중이면 즉시 종료
            if (currentSequence != null && currentSequence.IsActive())
            {
                currentSequence.Kill(); // 현재 애니메이션 즉시 종료
            }

            float scaleValue = Mathf.Abs(rankPoint) / 10f;  // RankPoint 값을 10으로 나눠 범위 변환
            int fullCells = Mathf.FloorToInt(scaleValue);  // 가득 찬 셀 개수
            float partialFill = scaleValue - fullCells;    // 마지막 셀의 소수점 값 (0~1)

            Transform[] targetCells = rankPoint > 0 ? gradePlusCells : gradeMinusCells;
            Transform[] oppositeCells = rankPoint > 0 ? gradeMinusCells : gradePlusCells;

            Sequence seq = DOTween.Sequence(); // DOTween 애니메이션 순차 실행
            currentSequence = seq; // 현재 시퀀스를 추적

            // 반대쪽 셀들의 스케일을 0으로 초기화
            foreach (var cell in oppositeCells)
            {
                if (cell != null)
                {
                    seq.Join(cell.DOScaleX(0f, 0.2f)); // 반대쪽 셀의 스케일을 0으로
                }
            }

            bool isIncreasing = false;
            bool isDecreasing = false;

            foreach (var cell in targetCells)
            {
                if (cell != null)
                {
                    float currentScale = cell.localScale.x;
                    float expectedScale = 0f;

                    // Index 초과를 방지하기 위해 fullCells가 targetCells 배열의 길이를 초과하지 않도록 처리
                    if (Array.IndexOf(targetCells, cell) < fullCells)
                    {
                        expectedScale = 1f;  // 채워진 셀
                    }
                    else if (Array.IndexOf(targetCells, cell) == fullCells)
                    {
                        expectedScale = partialFill;  // 마지막 셀의 소수점 값
                    }
                    else
                    {
                        expectedScale = 0f;  // 빈 셀
                    }

                    if (expectedScale > currentScale)
                        isIncreasing = true;
                    else if (expectedScale < currentScale)
                        isDecreasing = true;
                }
            }

            bool reverseOrder = false;

            if (rankPoint > 0)
            {
                reverseOrder = isDecreasing;
            }
            else
            {
                reverseOrder = isDecreasing;
            }

            int startIndex = reverseOrder ? targetCells.Length - 1 : 0;
            int endIndex = reverseOrder ? -1 : targetCells.Length;
            int step = reverseOrder ? -1 : 1;

            bool lastCellHandled = false; // 마지막 셀 처리 여부를 추적하는 변수

            for (int i = startIndex; i != endIndex; i += step)
            {
                if (i < 0 || i >= targetCells.Length) continue; // 인덱스 초과 방지

                if (targetCells[i] != null)
                {
                    float targetScaleX = 0f;

                    // 마지막 셀의 경우 partialFill을 사용
                    if (i < fullCells)
                    {
                        targetScaleX = 1f; // 채워진 셀
                    }
                    else if (i == fullCells)
                    {
                        targetScaleX = partialFill; // 마지막 셀의 소수점 값
                    }
                    else
                    {
                        targetScaleX = 0f; // 빈 셀
                    }

                    // 마지막 셀 처리
                    if (!lastCellHandled && i == fullCells)
                    {
                        lastCellHandled = true; // 마지막 셀 처리되었음을 표시
                        if (animate)
                        {
                            seq.Append(targetCells[i].DOScaleX(targetScaleX, 0.2f)); // 마지막 셀 애니메이션 실행
                        }
                        else
                        {
                            targetCells[i].localScale = new Vector3(targetScaleX, 1f, 1f); // 마지막 셀 직접 설정
                        }
                        continue; // 마지막 셀은 애니메이션이 실행된 후 다음 셀을 처리하지 않도록
                    }

                    // 마지막 셀 이후에는 애니메이션이 더 이상 실행되지 않도록 함
                    if (animate)
                    {
                        seq.Append(targetCells[i].DOScaleX(targetScaleX, 0.2f));
                    }
                    else
                    {
                        targetCells[i].localScale = new Vector3(targetScaleX, 1f, 1f);
                    }
                }
            }

            seq.Play(); // 애니메이션 실행
        }
        */
        void RankPointSet(UserSession userSession, GameResult gameResult)
        {
            // 실질적 승급 계산
            (int afterRankPoint, bool isRankChange) = GradeChangeManager.GetRankPointAndGradeUpdate(SessionManager.currentUserId, userSession, gameResult);

            int needPlayNum = Mathf.CeilToInt((float)(GradeChangeManager.GetRankPointRange() - afterRankPoint) / GradeChangeManager.GetWinPoint(userSession.Grade));

           

            gradeResultText.text = needPlayNum + "게임 승리 시 승급";

            if (isRankChange) // 급수 변경됨
            {
                if (gameResult == GameResult.Win)
                {
                    gradeResultText.text = "승급!";
                }

                else
                {
                    gradeResultText.text = "강등!";
                }
            }


            StartCoroutine(RankAnimation(afterRankPoint));
        }

        IEnumerator RankAnimation(int afterRankPoint)
        {
            yield return new WaitForSeconds(0.5f);


            // 변동된 값 기준 셀너비 재설정
            UpdateCellScales(afterRankPoint, true);
        }










        void UpdateCellScales(float rankPoint, bool animate = false)
        {
            // 이전 시퀀스가 실행 중이면 즉시 종료
            KillCurrentSequenceIfActive();

            float scaleValue = Mathf.Abs(rankPoint) / 10f;  // RankPoint 값을 10으로 나눠 범위 변환
            int fullCells = Mathf.FloorToInt(scaleValue);  // 가득 찬 셀 개수
            float partialFill = scaleValue - fullCells;    // 마지막 셀의 소수점 값 (0~1)

            Transform[] targetCells = GetTargetCells(rankPoint);
            Transform[] oppositeCells = GetOppositeCells(rankPoint);

            Sequence seq = DOTween.Sequence(); // DOTween 애니메이션 순차 실행
            currentSequence = seq; // 현재 시퀀스를 추적

            // 반대쪽 셀들의 스케일을 0으로 초기화
            ResetOppositeCellsScale(seq, oppositeCells);

            // 셀들의 상태 업데이트 및 애니메이션 실행
            AnimateCells(seq, targetCells, fullCells, partialFill, animate);

            seq.Play(); // 애니메이션 실행
        }

        void KillCurrentSequenceIfActive()
        {
            if (currentSequence != null && currentSequence.IsActive())
            {
                currentSequence.Kill(); // 현재 애니메이션 즉시 종료
            }
        }

        Transform[] GetTargetCells(float rankPoint)
        {
            return rankPoint > 0 ? gradePlusCells : gradeMinusCells;
        }

        Transform[] GetOppositeCells(float rankPoint)
        {
            return rankPoint > 0 ? gradeMinusCells : gradePlusCells;
        }

        void ResetOppositeCellsScale(Sequence seq, Transform[] oppositeCells)
        {
            foreach (var cell in oppositeCells)
            {
                if (cell != null)
                {
                    seq.Join(cell.DOScaleX(0f, 0.2f)); // 반대쪽 셀의 스케일을 0으로
                }
            }
        }

        void AnimateCells(Sequence seq, Transform[] targetCells, int fullCells, float partialFill, bool animate)
        {
            bool isDecreasing = false;

            // 각 셀의 상태에 따라 애니메이션을 준비
            foreach (var cell in targetCells)
            {
                if (cell != null)
                {
                    float currentScale = cell.localScale.x;
                    float expectedScale = GetExpectedScale(targetCells, fullCells, partialFill, cell);

                    if (expectedScale < currentScale) isDecreasing = true;
                }
            }

            bool reverseOrder = DetermineReverseOrder(isDecreasing, targetCells, fullCells);

            int startIndex = reverseOrder ? targetCells.Length - 1 : 0;
            int endIndex = reverseOrder ? -1 : targetCells.Length;
            int step = reverseOrder ? -1 : 1;

            bool lastCellHandled = false; // 마지막 셀 처리 여부를 추적하는 변수

            // 셀에 대한 애니메이션 실행
            for (int i = startIndex; i != endIndex; i += step)
            {
                if (i < 0 || i >= targetCells.Length) continue; // 인덱스 초과 방지

                if (targetCells[i] != null)
                {
                    float targetScaleX = GetExpectedScale(targetCells, fullCells, partialFill, targetCells[i]);

                    // 마지막 셀 처리
                    if (!lastCellHandled && i == fullCells)
                    {
                        lastCellHandled = true; // 마지막 셀 처리되었음을 표시
                        ApplyScale(targetCells[i], targetScaleX, animate, seq);
                        continue; // 마지막 셀은 애니메이션이 실행된 후 다음 셀을 처리하지 않도록
                    }

                    ApplyScale(targetCells[i], targetScaleX, animate, seq);
                }
            }
        }

        float GetExpectedScale(Transform[] targetCells, int fullCells, float partialFill, Transform cell)
        {
            int index = Array.IndexOf(targetCells, cell);

            if (index < fullCells)
                return 1f; // 채워진 셀
            else if (index == fullCells)
                return partialFill; // 마지막 셀의 소수점 값
            else
                return 0f; // 빈 셀
        }

        bool DetermineReverseOrder(bool isDecreasing, Transform[] targetCells, int fullCells)
        {
            return isDecreasing;
        }

        void ApplyScale(Transform cell, float targetScaleX, bool animate, Sequence seq)
        {
            if (animate)
            {
                seq.Append(cell.DOScaleX(targetScaleX, 0.2f)); // 애니메이션 실행
            }
            else
            {
                cell.localScale = new Vector3(targetScaleX, 1f, 1f); // 직접 설정
            }
        }
    }
}
