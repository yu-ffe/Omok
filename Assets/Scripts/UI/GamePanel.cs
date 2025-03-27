using System.Collections;
using System.Collections.Generic;
using Commons;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using KimHyeun;
using MJ;
using UnityEngine.SceneManagement;
using workspace.YU__FFE.Scripts;

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

    bool isComponentsConnected = false;

    void Start() {
        if (!isComponentsConnected)
            FindComponents();
        
        UI_Manager.Instance.AddPanel(UI_Manager.PanelType.Game, this);
        UI_Manager.Instance.AddCallback("turn", TurnStateRefresh);
        UI_Manager.Instance.AddCallback("time", TimeRefresh);

        SpawnTimer();
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
        LoadProfile();

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
        // Sprite sprite_Left = SessionManager.GetUserProfileSprite()
       //  Sprite sprite_Right = SessionManager.GetUserProfileSprite()

        // imgProfileLeft.sprite = sprite_Left;
        // imgProfileRight.sprite = sprite_Right;
         txtNickNameLeft.text = PlayerManager.Instance.playerData.grade + "급\n" + PlayerManager.Instance.playerData.nickname;
        // txtNickNameRight.text = nickName_Right; // 멀티 시 상대 정보 불러오기
    }


    void LoadGameState() {

    }


    void TurnStateRefresh() {

        //게임 로직에 따라 변경
        Debug.Log("Game Panel : TurnStateRefresh");
        Debug.Log($"{GameManager.Instance.gameLogic}");
        Debug.Log($"{GameManager.Instance.gameLogic.GetCurrentPlayerType()}");
        
        var currentPlayer = GameManager.Instance.gameLogic.GetCurrentPlayerType();

        SetImageAlpha(imgGameTurn[0], currentPlayer == Constants.PlayerType.PlayerA ? 1f : 0.5f);
        SetImageAlpha(imgGameTurn[1], currentPlayer == Constants.PlayerType.PlayerB ? 1f : 0.3f);
    }
    void SetImageAlpha(Image image, float alpha)
    {
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
    public void Button_GiveUp() {
        if (GameEndManager.Instance != null)
        {
            GameEndManager.Instance.ShowGameEndPanel("상대가 기권했습니다!"); // 원하는 메시지 전달
        }
        else
        {
            Debug.LogError("[GamePanel] GameEndManager.Instance가 null입니다!");
        }
    }

    /// <summary> 착수버튼 이벤트 </summary>
    public void Button_EndOfTurn() {

    }

}
