using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public enum GameResult // TODO 게임 로직과 겹치면 제거
{
    None, // 게임 진행 중
    Win, // 플레이어 승
    Lose, // 플레이어 패
    Draw // 비김
}

public class GameEndManager : UI_Panel
{
    public static GameEndManager Instance{get; private set;}
    
    [SerializeField] GameObject gg;

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
    [SerializeField] Button okButton;
    [SerializeField] TMP_Text okButtonText;
    [SerializeField] Button restartButton;
    [SerializeField] TMP_Text restartButtonText;
    [SerializeField] Button recordButton;
    [SerializeField] TMP_Text recordButtonText;

    private GameResult pendingResult;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        TryAutoRegister();
        gameObject.SetActive(false);
        
    }
    
    void Start()
    {
        if (!UI_Manager.Instance.HasPanel(panelType))
        {
            UI_Manager.Instance.AddPanel(panelType, this);
        }
        gameObject.SetActive(false); // 패널 비활성화
    }

    public void PrepareGameEndInfo(GameResult result)
    {
        pendingResult = result;

        // 패널이 아직 비활성화되어 있으면 Enable 될 때 처리
        if (!gameObject.activeInHierarchy)
        {
            Debug.Log("[GameEndManager] 패널 비활성 상태, Enable 후 처리 예정");
            return;
        }

        // 이미 켜져 있다면 바로 처리
        ApplyEndGameInfo();
    }
    
    private void ApplyEndGameInfo()
    {
        if (pendingResult == GameResult.None) return;

        Debug.Log("[GameEndManager] 게임 종료 정보 적용 시작");

        SetEndGameInfo(pendingResult);
        pendingResult = GameResult.None;
    }
    
    public void SetEndGameInfo(GameResult gameResult)  // 실행 코드
    {
        // 승점바 설정
        GradeBarSetting();
        GradeCellReset();
        GradeMinMaxTextSet();

        EndButtonInfoSet(); // 버튼 설정

        SetAfterGameEnd(gameResult);
    }
    
    public void OnClick_OkButton()
    {
        GameManager.Instance.ChangeToMainScene();
    }

    public void OnClick_RestartButton()
    {
        Debug.Log("재대국 버튼 클릭");
        GameManager.Instance.RestartCurrentGame();
    }

    public void OnClick_RecordButton()
    {
        UI_Manager.Instance.popup.Show(
            "기보를 저장하시겠습니까?",
            "저장", "취소",
            okAction: () =>
            {
                Debug.Log("기보 저장 완료 (예정)");
            },
            cancelAction: () =>
            {
                Debug.Log("기보 저장 취소");
            }
        );
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
        //EndButtonClickListenerSet(); // 버튼 클릭 이벤트 할당
        EndButtonTextSet(); // 버튼 텍스트 할당
    }

    /*void EndButtonClickListenerSet()
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
    }*/

    void EndButtonTextSet()
    {
        okButtonText.text = "확인";
        restartButtonText.text = "재대국 신청";
        recordButtonText.text = "기보 저장";
    }

    public void SetAfterGameEnd(GameResult gameResult) // 게임 종료 처리
    {
        // 현재 로그인된 유저 세션ID, 해당 세션 정보 불러오기
        PlayerData playerData = PlayerManager.Instance.playerData;


        UpdateCellScales(playerData.rankPoint); // 이전 랭크 포인트 기준 우선 표기

        // 승패 포인트 설정
        int getPointPlusValue = GradeChangeManager.GetWinPoint(playerData.grade);
        int getPointMinusValue = GradeChangeManager.GetLosePoint();

        switch (gameResult)
        {
            case GameResult.Win:
                resultText.text = "승리!\n" + getPointPlusValue + "포인트 획득";

                // 승점 변동 애니메이션
                RankPointSet(playerData, gameResult);

                break;

            case GameResult.Lose:
                resultText.text = "패배!\n" + getPointMinusValue + "포인트 손실";

                // 승점 변동 애니메이션
                RankPointSet(playerData, gameResult);

                break;

            case GameResult.Draw:
                resultText.text = "무승부!\n포인트 변동 없음";

                // 승점 변동 애니메이션
                RankPointSet(playerData, gameResult);

                break;

            default:
                resultText.text = "오류!";
                break;
        }
    }

    void UpdateCellScales(float rankPoint, bool animate = false)
    {
        float scaleValue = Mathf.Abs(rankPoint) / 10f;  // RankPoint 값을 10으로 나눠 범위 변환
        int fullCells = Mathf.FloorToInt(scaleValue);  // 가득 찬 셀 개수
        float partialFill = scaleValue - fullCells;    // 마지막 셀의 소수점 값 (0~1)

        Transform[] targetCells = rankPoint > 0 ? gradePlusCells : gradeMinusCells;
        Transform[] oppositeCells = rankPoint > 0 ? gradeMinusCells : gradePlusCells;

        Sequence seq = DOTween.Sequence(); // DOTween 애니메이션 순차 실행

        // 반대쪽 셀들의 스케일을 0으로 초기화
        foreach (var cell in oppositeCells)
        {
            if (cell != null)
            {
                seq.Join(cell.DOScaleX(0f, 0.2f)); // 반대쪽 셀의 스케일을 0으로
            }
        }

        // 플러스/마이너스 셀들에 대해 애니메이션을 처리
        for (int i = 0; i < targetCells.Length; i++)
        {
            if (targetCells[i] != null)
            {
                float targetScaleX = (i < fullCells) ? 1f : (i == fullCells ? partialFill : 0f);

                if (animate)
                {
                    // 십의 자리가 변동될 때는 애니메이션 순차적으로 실행
                    if (i == fullCells && fullCells > 0) // 십의 자리 변화가 있을 경우
                    {
                        if (rankPoint > 0)  // 플러스 셀 감소
                        {
                            seq.Append(targetCells[i].DOScaleX(targetScaleX, 0.2f));
                        }
                        else  // 마이너스 셀 증가
                        {
                            seq.Append(targetCells[i].DOScaleX(targetScaleX, 0.2f));
                        }
                    }
                    else
                    {
                        seq.Join(targetCells[i].DOScaleX(targetScaleX, 0.2f)); // 다른 셀들은 동시 실행
                    }
                }
                else
                {
                    targetCells[i].localScale = new Vector3(targetScaleX, 1f, 1f);
                }
            }
        }

        seq.Play(); // 애니메이션 실행
    }

    void RankPointSet(PlayerData playerData, GameResult gameResult)
    {
        // 실질적 승급 계산
        int afterRankPoint = GradeChangeManager.GetRankPointAndGradeUpdate(playerData.nickname, playerData, gameResult);

        int needPlayNum = 0;

        if (afterRankPoint >= 0)
        {
            needPlayNum = Mathf.CeilToInt((GradeChangeManager.GetRankPointRange() - afterRankPoint) / GradeChangeManager.GetWinPoint(playerData.grade));
        }
        else
        {
            needPlayNum = Mathf.CeilToInt(GradeChangeManager.GetRankPointRange() / GradeChangeManager.GetWinPoint(playerData.grade) +
                (afterRankPoint / GradeChangeManager.GetWinPoint(playerData.grade)));
        }

        gradeResultText.text = needPlayNum + "게임 승리 시 승급";

        StartCoroutine(RankAnimation(afterRankPoint));
    }

    IEnumerator RankAnimation(int afterRankPoint)
    {
        yield return new WaitForSeconds(0.5f);


        // 변동된 값 기준 셀너비 재설정
        UpdateCellScales(afterRankPoint, true);
    }


    public override void Show()
    {
        if (!gg.activeSelf)
        {
            gg.SetActive(true);
            Debug.Log("보이기");
        }
        
        okButton.onClick.RemoveAllListeners();
        restartButton.onClick.RemoveAllListeners();
        recordButton.onClick.RemoveAllListeners();

        okButton.onClick.AddListener(OnClick_OkButton);
        restartButton.onClick.AddListener(OnClick_RestartButton);
        recordButton.onClick.AddListener(OnClick_RecordButton);
    }
    
    public override void Hide()
    {
        //gameObject.SetActive(false);
    }

    public override void OnEnable()
    {
        TryAutoRegister();
        
        StartCoroutine(DelayedApply());
    }

    public override void OnDisable()
    {
    }
    
    private IEnumerator DelayedApply()
    {
        yield return null; // 1프레임 대기 후
        ApplyEndGameInfo();
    }

    private void TryAutoRegister()
    {
        if (UI_Manager.Instance == null) return;
        if (panelType == UI_Manager.PanelType.None) return;

        if (!UI_Manager.Instance.HasPanel(panelType))
        {
            UI_Manager.Instance.AddPanel(panelType, this);
            Debug.Log($"[UI_Panel] {panelType} 패널이 자동 등록됨");
        }
    }

}
