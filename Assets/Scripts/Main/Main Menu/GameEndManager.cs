using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Commons;

public class GameEndManager : UI_Panel
{
    public static GameEndManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] GameObject gg;
    [SerializeField] TMP_Text resultText, gradeResultText, gradeMinText, gradeMaxText;
    [SerializeField] RectTransform gradeBar;
    [SerializeField] GameObject gradePlusCellPrefab, gradeMinusCellPrefab;
    [SerializeField] GameObject okButton, restartButton, recordButton;
    [SerializeField] TMP_Text okButtonText, restartButtonText, recordButtonText;

    RectTransform[] gradePlusCells = new RectTransform[3];
    RectTransform[] gradeMinusCells = new RectTransform[3];
    Constants.GameResult pendingResult;

    [SerializeField] Transform originPosition;
    [SerializeField] Transform showPosition;

    protected override void Awake()
    {
        base.Awake();
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
       
        TryAutoRegister();

        transform.position = originPosition.position;
    }

    void Start()
    {
        if (!UI_Manager.Instance.HasPanel(panelType))
            UI_Manager.Instance.AddPanel(panelType, this);
    }

    public override void Show()
    {
        transform.position = showPosition.position;
        /*
        Debug.Log("[GameEndManager] Show() 호출됨");

        if (!gameObject.activeSelf)
        {
            Debug.Log("[GameEndManager] gg 활성화");
            gameObject.SetActive(true);
        }*/

        EndButtonInfoSet();
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< Updated upstream
=======
<<<<<<< HEAD
        
<<<<<<< HEAD
=======
>>>>>>> parent of 119989b (Merge pull request #261 from yu-ffe/feature/minji/Add-shadows)
>>>>>>> Stashed changes
=======
        VictoryEffectManager.Instance.ShowVictoryEffect();
        
>>>>>>> parent of dbfe2b9 (승리시 파티클 적용)
=======
>>>>>>> parent of 119989b (Merge pull request #261 from yu-ffe/feature/minji/Add-shadows)
=======
>>>>>>> parent of ed06ff2 (일단 간단한 승리파티클 생성)
    }

    public override void Hide()
    {
        transform.position = originPosition.position;
        // gameObject.SetActive(false); // 혹시 어디선가 이게 먼저 실행되면 안 됨
    }

    public override void OnEnable()
    {
        TryAutoRegister();
        StartCoroutine(DelayedApply());
    }

    public override void OnDisable() { }

    private IEnumerator DelayedApply()
    {
        yield return null;
        ApplyEndGameInfo();
    }

    public void PrepareGameEndInfo(Constants.GameResult result)
    {
        pendingResult = result;
        if (!gameObject.activeInHierarchy) return;
        ApplyEndGameInfo();
    }

    private void ApplyEndGameInfo()
    {
        if (pendingResult == Constants.GameResult.None) return;
        SetEndGameInfo(pendingResult);
        pendingResult = Constants.GameResult.None;
    }

    public void SetEndGameInfo(Constants.GameResult gameResult)
    {
        // DualPlayer일 경우 버튼 변경 처리
        if (GameManager.Instance.CurrentGameType == Constants.GameType.DualPlayer)
        {
            Debug.LogWarning("듀얼 처리");
            var winnerPlayer = Constants.GetWinnerPlayerFromGameResult(gameResult);

            string winnerLabel = winnerPlayer switch
            {
                Constants.PlayerType.PlayerA => "PlayerA",
                Constants.PlayerType.PlayerB => "PlayerB",
                _ => "플레이어"
            };

            resultText.text = $"{winnerLabel}의 승리입니다!";
            gradeResultText.text = null;

            // 버튼 보이기/숨기기
            okButton.SetActive(true);
            restartButton.SetActive(true);
            recordButton.SetActive(false);

            // 텍스트 설정
            okButtonText.text = "확인";
            restartButtonText.text = "재대국";

            // okButton 클릭 시 → 씬 전환 해서 메인 패널 열기
            var okBtn = okButton.GetComponent<Button>();
            okBtn.onClick.RemoveAllListeners();
            okBtn.onClick.AddListener(() =>
            {
                this.Hide();
                GameManager.Instance.ChangeToMainScene();
            });

            var restartBtn = restartButton.GetComponent<Button>();
            restartBtn.onClick.RemoveAllListeners();
            restartBtn.onClick.AddListener(() =>
            {
                GameManager.Instance.SetGameType(Constants.GameType.DualPlayer);
                GameManager.Instance.ChangeToGameScene(Constants.GameType.DualPlayer);
            });

            return; // DualPlayer 처리 종료
        }
        else if (GameManager.Instance.CurrentGameType == Constants.GameType.SinglePlayer 
                 || GameManager.Instance.CurrentGameType == Constants.GameType.MultiPlayer)
        {
            Debug.LogWarning("일반 처리");
            // 싱글/멀티 플레이일 경우 기존 로직 실행
            GradeBarSetting();
            GradeCellReset();
            GradeMinMaxTextSet();
            SetAfterGameEnd(gameResult);
        }
        
    }

    private void EndButtonInfoSet()
    {
        EndButtonClickListenerSet();
        SetButtonTexts();
    }

    private void EndButtonClickListenerSet()
    {
        Debug.Log("[GameEndManager] 버튼 리스너 등록 시작");
        
        Button okBtn = okButton.GetComponent<Button>();
        Debug.Log("okButton 컴포넌트 있음? " + (okBtn != null));

        Button restartBtn = restartButton.GetComponent<Button>() ?? restartButton.AddComponent<Button>();
        Button recordBtn = recordButton.GetComponent<Button>() ?? recordButton.AddComponent<Button>();

        okBtn.onClick.RemoveAllListeners();
        restartBtn.onClick.RemoveAllListeners();
        recordBtn.onClick.RemoveAllListeners();

        okBtn.onClick.AddListener(() => { GameEndButtonClickManager.Instance.OnClick_OkButton();});
        restartBtn.onClick.AddListener(() => { GameEndButtonClickManager.Instance.OnClick_RestartButton(); });
        recordBtn.onClick.AddListener(() => { GameRecorder.SaveGameRecord(); });
    }
    
    public void ShowGameEndPanel(string message)
    {
        UI_Manager.Instance.Show(panelType);
        Show();
    }


    private void SetButtonTexts()
    {
        okButtonText.text = "확인";
        restartButtonText.text = "재대국 신청";
        recordButtonText.text = "기보 저장";
    }

    void GradeBarSetting()
    {
        ClearExistingCells();
        float barWidth = gradeBar.rect.width;
        float barHeight = gradeBar.rect.height;
        float spacing = 10f;
        float availableWidth = barWidth - (spacing * 7);
        float cellWidth = availableWidth / 6f;
        float cellHeight = barHeight - (spacing * 2);
        float startX = -barWidth / 2 + spacing;

        for (int i = 0; i < 3; i++)
        {
            gradeMinusCells[i] = Instantiate(gradeMinusCellPrefab, gradeBar).GetComponent<RectTransform>();
            gradePlusCells[i] = Instantiate(gradePlusCellPrefab, gradeBar).GetComponent<RectTransform>();
        }

        for (int i = 0; i < 3; i++)
        {
            int idx = 2 - i;
            float xPosMinus = startX + (i + 1) * cellWidth + i * spacing;
            gradeMinusCells[idx].pivot = new Vector2(1f, 0.5f);
            gradeMinusCells[idx].sizeDelta = new Vector2(cellWidth, cellHeight);
            gradeMinusCells[idx].anchoredPosition = new Vector2(xPosMinus, 0);

            float xPosPlus = startX + (i + 3) * cellWidth + (i + 3) * spacing;
            gradePlusCells[i].pivot = new Vector2(0f, 0.5f);
            gradePlusCells[i].sizeDelta = new Vector2(cellWidth, cellHeight);
            gradePlusCells[i].anchoredPosition = new Vector2(xPosPlus, 0);
        }
    }

    void ClearExistingCells()
    {
        foreach (var cell in gradeMinusCells) if (cell != null) Destroy(cell.gameObject);
        foreach (var cell in gradePlusCells) if (cell != null) Destroy(cell.gameObject);
    }

    void GradeCellReset()
    {
        foreach (var cell in gradePlusCells) if (cell != null) cell.localScale = new Vector3(0f, 1f, 1f);
        foreach (var cell in gradeMinusCells) if (cell != null) cell.localScale = new Vector3(0f, 1f, 1f);
    }

    void GradeMinMaxTextSet()
    {
        int range = GradeChangeManager.GetRankPointRange();
        gradeMinText.text = (-range).ToString();
        gradeMaxText.text = range.ToString();
    }

    public void SetAfterGameEnd(Constants.GameResult result)
    {
        var player = PlayerManager.Instance.playerData;
        UpdateCellScales(player.rankPoint);

        int winPoint = GradeChangeManager.GetWinPoint(player.grade);
        int losePoint = GradeChangeManager.GetLosePoint();

        switch (result)
        {
            case Constants.GameResult.Win:
                resultText.text = $"승리!\n{winPoint}포인트 획득";
                break;
            case Constants.GameResult.Lose:
                resultText.text = $"패배!\n{losePoint}포인트 손실";
                break;
            case Constants.GameResult.Draw:
                resultText.text = "무승부!\n포인트 변동 없음";
                break;
            case Constants.GameResult.Player1Win:
                resultText.text = "Player1의 승리!";
                break;
            case Constants.GameResult.Player2Win:
                resultText.text = "Player2의 승리!";
                break;
            default:
                resultText.text = "오류!";
                return;
        }
        
        // Single/Multi만 포인트 처리
        if (result == Constants.GameResult.Win || result == Constants.GameResult.Lose)
        {
            RankPointSet(player, result);
        }

        //RankPointSet(player, result);
        GameRecorder.GameResultSave(result); // 게임 결과 저장
    }

    void UpdateCellScales(float rankPoint, bool animate = false)
    {
        float scaleVal = Mathf.Abs(rankPoint) / 10f;
        int full = Mathf.FloorToInt(scaleVal);
        float partial = scaleVal - full;

        var target = rankPoint > 0 ? gradePlusCells : gradeMinusCells;
        var opposite = rankPoint > 0 ? gradeMinusCells : gradePlusCells;
        var seq = DOTween.Sequence();

        foreach (var cell in opposite) if (cell != null) seq.Join(cell.DOScaleX(0f, 0.2f));

        for (int i = 0; i < target.Length; i++)
        {
            if (target[i] == null) continue;
            float targetScaleX = (i < full) ? 1f : (i == full ? partial : 0f);
            if (animate) seq.Append(target[i].DOScaleX(targetScaleX, 0.2f));
            else target[i].localScale = new Vector3(targetScaleX, 1f, 1f);
        }

        seq.Play();
    }

    void RankPointSet(PlayerData playerData, Constants.GameResult result)
    {
        int afterPoint = GradeChangeManager.GetRankPointAndGradeUpdate(playerData.nickname, playerData, result);
        int winPoint = GradeChangeManager.GetWinPoint(playerData.grade);

        int needPlay = (afterPoint >= 0)
            ? Mathf.CeilToInt((GradeChangeManager.GetRankPointRange() - afterPoint) / (float)winPoint)
            : Mathf.CeilToInt(GradeChangeManager.GetRankPointRange() / (float)winPoint + afterPoint / (float)winPoint);

        gradeResultText.text = $"{needPlay}게임 승리 시 승급";
        StartCoroutine(RankAnimation(afterPoint));
    }

    IEnumerator RankAnimation(int afterRankPoint)
    {
        yield return new WaitForSeconds(0.5f);
        UpdateCellScales(afterRankPoint, true);
    }

    void TryAutoRegister()
    {
        if (UI_Manager.Instance == null || panelType == UI_Manager.PanelType.None) return;
        if (!UI_Manager.Instance.HasPanel(panelType))
        {
            UI_Manager.Instance.AddPanel(panelType, this);
            Debug.Log($"[UI_Panel] {panelType} 패널이 자동 등록됨");
        }
    }
}