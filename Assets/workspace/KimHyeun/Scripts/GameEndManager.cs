using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace KimHyeun {
    public class GameEndManager : MonoBehaviour
    {
        /*
        EndGame(GameResult gameResult) 내부에서 실행
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
        [SerializeField] RectTransform[] gradePlusCells = new RectTransform[3];
        [Header("- 영역 셀")]
        [SerializeField] RectTransform[] gradeMinusCells = new RectTransform[3];

        [Header("필수 할당 - 버튼 역할 오브젝트,텍스트")]
        [SerializeField] GameObject okButton;
        [SerializeField] TMP_Text okButtonText;
        [SerializeField] GameObject restartButton;
        [SerializeField] TMP_Text restartButtonText;
        [SerializeField] GameObject recordButton;
        [SerializeField] TMP_Text recordButtonText;

        private void Start()
        {
            GradeBarSetting();
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

            // 왼쪽 영역에 마이너스(-) 셀 배치 (2,1,0 순서로)
            for (int i = 0; i < 3; i++)
            {
                int index = 2 - i; // 2,1,0 순서로 접근
                if (gradeMinusCells[index] != null)
                {
                    gradeMinusCells[index].pivot = new Vector2(1f, 0.5f); // 우측 중앙 피봇
                    gradeMinusCells[index].sizeDelta = new Vector2(cellWidth, cellHeight);

                    // i=0일 때 첫번째 셀, i=1일 때 두번째 셀, i=2일 때 세번째 셀
                    float xPos = startX + (i + 1) * cellWidth + i * cellSpacing;
                    gradeMinusCells[index].anchoredPosition = new Vector2(xPos, 0);
                }
            }

            // 우측 영역에 플러스(+) 셀 배치 (0,1,2 순서로)
            for (int i = 0; i < 3; i++)
            {
                if (gradePlusCells[i] != null)
                {
                    gradePlusCells[i].pivot = new Vector2(0f, 0.5f); // 좌측 중앙 피봇
                    gradePlusCells[i].sizeDelta = new Vector2(cellWidth, cellHeight);

                    // i=0일 때 네번째 셀, i=1일 때 다섯번째 셀, i=2일 때 여섯번째 셀
                    float xPos = startX + (i + 3) * cellWidth + (i + 3) * cellSpacing;
                    gradePlusCells[i].anchoredPosition = new Vector2(xPos, 0);
                }
            }
        }

        void GradeCellReset()
        {

        }


        public void SetAfterGameEnd(GameResult gameResult) // 게임 종료 시 호출
        {
            // string sessionId, UserSession userSession, 
            // 현재 로그인된 유저 세션ID, 해당 세션 정보 불러오기


        }
    }
}
