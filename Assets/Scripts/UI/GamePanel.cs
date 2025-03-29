using System.Collections;
using System.Collections.Generic;
using Commons;
using Commons.Models;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GamePanel : UI_Panel {
    [Header("좌측 프로필")]
    public Image imgProfileLeft;
    public TextMeshProUGUI txtNickNameLeft;

    [Header("우측 프로필")]
    public Image imgProfileRight;
    public TextMeshProUGUI txtNickNameRight;


    [Header("시간/턴 정보")]
    public Transform timerHolder;
    public GameObject timerPrefab;
    private GameObject timerInstance;
    //public TextMeshProUGUI txtTimer;
    //public Image imgLeftTime;
    public Image[] imgGameTurn = new Image[2];
    
    public GamePopup gamePopupPrefab;
    private GamePopup popupInstance;

    bool isComponentsConnected = false;

    void Start() {
        if (!isComponentsConnected)
            FindComponents();
        
        UI_Manager.Instance.AddPanel(UI_Manager.PanelType.Game, this);
        UI_Manager.Instance.AddCallback("turn", TurnStateRefresh);
        UI_Manager.Instance.AddCallback("time", TimeRefresh);

        SpawnTimer();

        LoadProfile();

    }

    /// <summary>
    /// Timer 프리팹을 Inspector에서 연결된 빈 오브젝트 위치에 배치
    /// </summary>
    void SpawnTimer() {
        if (timerPrefab == null) {
            Debug.LogError("Timer Prefab이 설정되지 않았습니다.");
            return;
        }

        if (timerHolder == null) {
            Debug.LogError("Timer Holder가 Inspector에서 설정되지 않았습니다.");
            return;
        }

        // Timer 프리팹을 Timer Holder의 위치에 생성
        timerInstance = Instantiate(timerPrefab, timerHolder);

        // RectTransform 설정
        RectTransform timerRect = timerInstance.GetComponent<RectTransform>();
        if (timerRect != null) {
            timerRect.anchoredPosition = Vector2.zero; // 빈 오브젝트의 위치 기준으로 정렬
            timerRect.sizeDelta = new Vector2(200, 200); // 크기 조정
        }
    }

    public override void Show() {
        
        
        LoadGameState();

        gameObject.SetActive(true);
    }
    public override void Hide() {
        gameObject.SetActive(false);
    }

    public override void OnDisable() {
    }

    public override void OnEnable() {
    }

    [ContextMenu("Components Connect")]
    /// <summary> 컴포넌트와 버튼 이벤트 연결 스크립트 </summary>
    void FindComponents() {
        //0 Profile, 1 Info, 2 Button
        var parentsProfile = transform.GetChild(0);
        var parentsInfo = transform.GetChild(1);
        var parentsButton = transform.GetChild(2);
        imgProfileLeft = parentsProfile.GetChild(0).GetChild(0).GetComponent<Image>();
        txtNickNameLeft = imgProfileLeft.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        imgProfileRight = parentsProfile.GetChild(1).GetChild(0).GetComponent<Image>();
        txtNickNameRight = imgProfileRight.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        //imgLeftTime = parentsInfo.GetChild(0).GetComponent<Image>();
        //txtTimer = imgLeftTime.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        imgGameTurn[0] = parentsInfo.GetChild(0).GetComponent<Image>();
        imgGameTurn[1] = parentsInfo.GetChild(1).GetComponent<Image>();
        isComponentsConnected = true;

        var giveup = parentsButton.GetChild(0).GetComponent<Button>();
        var endturn = parentsButton.GetChild(1).GetComponent<Button>();

        giveup.onClick.RemoveAllListeners();
        endturn.onClick.RemoveAllListeners();
        giveup.onClick.AddListener(Button_GiveUp);
        endturn.onClick.AddListener(Button_EndOfTurn);

        isComponentsConnected = true;
    }


    /// <summary> 양쪽 게임 유저의 프로필 사진와 닉네임 가져옵니다 </summary>
    void LoadProfile() {
        

        imgProfileLeft.sprite = PlayerManager.Instance.GetProfileSprites(PlayerManager.Instance.playerData.profileNum);
        txtNickNameLeft.text = PlayerManager.Instance.playerData.grade + "급\n" + PlayerManager.Instance.playerData.nickname;


        if(GameManager.Instance.GetGameType() == GameEnums.GameType.SinglePlayer)
        {
            imgProfileRight.sprite = PlayerManager.Instance.GetProfileSprites(0);

            switch (GameManager.Instance.GetAILevel()) // AI 난이도
            {
                case GameEnums.AILevel.Easy:
                    txtNickNameRight.text = "AI-Lv1";
                    break;

                case GameEnums.AILevel.Middle:
                    txtNickNameRight.text = "AI-Lv2";
                    break;

                case GameEnums.AILevel.Hard:
                    txtNickNameRight.text = "AI-Lv3";
                    break;
            }
        }

        else if(GameManager.Instance.GetGameType() == GameEnums.GameType.MultiPlayer) // 멀티 시 상대 정보 불러오기
        {
            imgProfileRight.sprite = PlayerManager.Instance.GetProfileSprites(0);
            txtNickNameRight.text = "멀티 상대";
        }

        else if (GameManager.Instance.GetGameType() == GameEnums.GameType.Record) // 기보 시 저장된 상대 정보 불러오기
        {
            imgProfileRight.sprite = PlayerManager.Instance.GetProfileSprites(0);
            txtNickNameRight.text = "상대 플레이어";
        }

        else // 듀얼 플레이
        {
            imgProfileLeft.sprite = PlayerManager.Instance.GetProfileSprites(1);
            imgProfileRight.sprite = PlayerManager.Instance.GetProfileSprites(0);
            txtNickNameLeft.text = "플레이어 A";
            txtNickNameRight.text = "플레이어 B";
        }
        
    }


    void LoadGameState() {

    }


    void TurnStateRefresh() {

        //게임 로직에 따라 변경
        Debug.Log("Game Panel : TurnStateRefresh");
        Debug.Log($"{GameManager.Instance.gameLogic}");
        Debug.Log($"{GameManager.Instance.gameLogic.GetCurrentPlayerType()}");
        
        var currentPlayer = GameManager.Instance.gameLogic.GetCurrentPlayerType();

        if (imgGameTurn != null && imgGameTurn.Length >= 2)
        {
            if (imgGameTurn[0] != null)
                SetImageAlpha(imgGameTurn[0], currentPlayer == GameEnums.PlayerType.PlayerA ? 1f : 0.5f);
        
            if (imgGameTurn[1] != null)
                SetImageAlpha(imgGameTurn[1], currentPlayer == GameEnums.PlayerType.PlayerB ? 1f : 0.3f);
        }
        else
        {
            Debug.LogWarning("GamePanel의 imgGameTurn 이미지 참조가 누락되었습니다.");
        }
    }
    void SetImageAlpha(Image image, float alpha)
    {
        if (image == null)
        {
            Debug.LogWarning("SetImageAlpha 호출 시 image가 null입니다.");
            return;
        }

        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }

    void TimeRefresh() {
        //게임 로직에 따라 변경
        //Temp Test value
        float leftTime = Random.Range(0f, 30f);
        //imgLeftTime.fillAmount = leftTime / 30f;
        //txtTimer.text = Mathf.RoundToInt(leftTime).ToString();
    }


    void ExitToMain() {

    }


    /// <summary> 기권버튼 이벤트 </summary>
    public void Button_GiveUp()
    {
        SoundManager.Instance.ButtonClickSound();//버튼 클릭음
        
        if (popupInstance == null)
        {
            popupInstance = Instantiate(gamePopupPrefab, FindObjectOfType<Canvas>().transform);
        }

        popupInstance.Setup(
            message: "정말로 기권하시겠습니까?",
            confirmText: "예",
            confirmAction: OnGiveUpConfirmed,
            cancelText: "아니오",
            cancelAction: () => Debug.Log("기권 취소됨")
        );

        popupInstance.OpenPopup();
    }
    
    private void OnGiveUpConfirmed()
    {
        var currentPlayerType = GameManager.Instance.gameLogic.GetCurrentPlayerType();
        GameManager.Instance.gameLogic.HandleCurrentPlayerDefeat(currentPlayerType);
    }

    /// <summary> 착수버튼 이벤트 </summary>
    public void Button_EndOfTurn()
    {
        SoundManager.Instance.ButtonClickSound();//버튼 클릭음

    }

}
