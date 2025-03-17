using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace KimHyeun {
    public class GameEndManager : Singleton<GameEndManager>
    {
        /*
        
- 승/패 UI- 승급/강등 표시
-> 우선 실행
- 승급/강등 애니메이션
-> 중간에 승급 계산 함수
- 기보 저장 여부 확인 팝업 버튼
-> 기보 저장/취소 버튼 이벤트 실행 함수
        */

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

        

        public void SetEndGameInfo(GameResult gameResult) 
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

        // TODO 각 버튼 기능 추가
        void EndButtonClickListenerSet()
        {
            okButton.AddComponent<Button>().onClick.AddListener(() => { Debug.Log("확인 버튼 클릭(메인 프로필로 돌아가기)"); });
            restartButton.AddComponent<Button>().onClick.AddListener(() => { Debug.Log("재대국 버튼 클릭(AI,듀얼-재시작)"); });
            recordButton.AddComponent<Button>().onClick.AddListener(() => { Debug.Log("기보 저장 버튼 클릭(기보 저장 확인 팝업 출력)"); });
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

            // gradeResultText -> 등급 결과에 따라 영향

            // 유저 아이디, 해당 유저의 세션과 대국 결과를 전달받아서 급수를 업데이트 하는 함수.
            // 승급 애니메이션 과정에 호출(이전 상태와 비교를 위해)
            //  GradeChangeManager.GradeUpdate(SessionManager.currentUserId, userSession, gameResult);

            Debug.Log($"이전 랭크 포인트: {userSession.RankPoint}");
            UpdateCellScales(userSession.RankPoint);


            switch (gameResult) 
            {
                case GameResult.Win:
                    resultText.text = "승리!";

                    // TODO 승점 표기 애니메이션, 승급 애니메이션 체크


                    break;

                case GameResult.Lose:
                    resultText.text = "패배!";

                    // TODO 승점 표기 애니메이션, 강등 애니메이션 체크

                    break;

                case GameResult.Draw:
                    resultText.text = "무승부!";

                    // TODO 승점 표기 애니메이션

                    break;

                default:
                    resultText.text = "오류!";
                    break;
            }
        }

        void UpdateCellScales(float rankPoint)
        {
            float scaleValue = Mathf.Abs(rankPoint) / 10f;  // RankPoint 값을 10으로 나눠 범위 변환
            int fullCells = Mathf.FloorToInt(scaleValue);  // 가득 찬 셀 개수
            float partialFill = scaleValue - fullCells;    // 마지막 셀의 소수점 값 (0~1)

            if (rankPoint > 0)
            {
                for (int i = 0; i < gradePlusCells.Length; i++)
                {
                    if (gradePlusCells[i] != null)
                    {
                        if (i < fullCells)
                            gradePlusCells[i].localScale = new Vector3(1f, 1f, 1f);  // 가득 찬 셀
                        else if (i == fullCells)
                            gradePlusCells[i].localScale = new Vector3(partialFill, 1f, 1f); // 소수점 적용
                        else
                            gradePlusCells[i].localScale = new Vector3(0f, 1f, 1f);  // 나머지는 0
                    }
                }
            }
            else if (rankPoint < 0)
            {
                for (int i = 0; i < gradeMinusCells.Length; i++)
                {
                    if (gradeMinusCells[i] != null)
                    {
                        if (i < fullCells)
                            gradeMinusCells[i].localScale = new Vector3(1f, 1f, 1f);
                        else if (i == fullCells)
                            gradeMinusCells[i].localScale = new Vector3(partialFill, 1f, 1f);
                        else
                            gradeMinusCells[i].localScale = new Vector3(0f, 1f, 1f);
                    }
                }
            }
        }





    }  
}
