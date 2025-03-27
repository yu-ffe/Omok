using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainPanel : UI_Panel
{
    public TextMeshProUGUI txtCoin;
    public Image imgUserPortrait;
    public TextMeshProUGUI txtUserName;

    // PlayerData playerData => SessionManager.GetSession(SessionManager.currentUserId);
    // 데이터 요청 후 가져오기

    private PlayerData playerData;

    bool isConnctedCompoenets = false;



    void Start()
    {
        Debug.Log("[MainPanel] Start");
        StartCoroutine(EnsureUIManagerInitialized());


        if (!isConnctedCompoenets)
            FindComponents();

        // 유저 정보를 서버에서 가져온 후 UI 생성, 비동기로 실행
        //StartCoroutine(LoadPlayerDataAndInitializeUI());
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator EnsureUIManagerInitialized()
    {
        while (!UI_Manager.Instance)
        {
            yield return null; // UI_Manager가 초기화될 때까지 대기
        }

        UI_Manager.Instance.AddPanel(UI_Manager.PanelType.Main, this);
        //gameObject.SetActive(false);
    }

    private IEnumerator LoadPlayerDataAndInitializeUI()
    {
        yield return StartCoroutine(PlayerManager.Instance.UpdateUserData());
        playerData = PlayerManager.Instance.playerData;

        UI_Manager.Instance.AddPanel(panelType, this);
        gameObject.SetActive(false);
    }

    public override void Show()
    {
        Debug.Log("[MainPanel] Show() 호출됨");
        gameObject.SetActive(true);

        if (!isConnctedCompoenets)
        {
            Debug.Log("[MainPanel] FindComponents() 실행");
            FindComponents();

        }

        if (UI_Manager.Instance.Panels.TryGetValue(UI_Manager.PanelType.Login, out var loginPanel))
            loginPanel.gameObject.SetActive(false);
        if (UI_Manager.Instance.Panels.TryGetValue(UI_Manager.PanelType.Loading, out var loadingPanel))
            loadingPanel.gameObject.SetActive(false);
    }
    public override void Hide()
    {
        gameObject.SetActive(false);
        UI_Manager.Instance.Panels[UI_Manager.PanelType.Login].gameObject.SetActive(false);

    }


    void FindComponents()
    {
        Debug.Log("[MainPanel] FindComponents 시작");

        var root = transform;
        txtCoin = root.GetChild(0).GetComponent<TextMeshProUGUI>();
        imgUserPortrait = root.GetChild(1).GetComponent<Image>();
        txtUserName = root.GetChild(2).GetComponent<TextMeshProUGUI>();

        Debug.Log($"txtCoin: {txtCoin}, imgUserPortrait: {imgUserPortrait}, txtUserName: {txtUserName}");

        var buttons = root.GetChild(3).GetComponentsInChildren<Button>();
        // Debug.Log($"버튼 수: {buttons.Length}");

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].onClick.RemoveAllListeners(); // 중복 방지
            int index = i;
            buttons[i].onClick.AddListener(() => { OnClick_Menu(index); });
        }

        isConnctedCompoenets = true;
    }

    public override void OnEnable()
    {
        ResfreshUserInfo();
        UI_Manager.Instance.AddCallback("UserInfo", ResfreshUserInfo);
    }
    public override void OnDisable()
    {
        if (UI_Manager.Instance == null) return;
        UI_Manager.Instance.RemoveCallback("UserInfo");
    }


    public void ResfreshUserInfo()
    {
        //Coin
        playerData = PlayerManager.Instance.playerData;
        txtCoin.text = playerData.coins.ToString();
        //유저정보 새로고침
        txtUserName.text = $"{playerData.grade}급 {playerData.nickname}";

        //TODO: 다른 클래스 
        // imgUserPortrait.sprite = SessionManager.ProfileSprites[playerData.profileNum];
    }


    private void StartSinglePlay()
    {
        UI_Manager.Instance.popup.Show(
            "싱글플레이를 시작하시겠습니까?",
            "시작", "취소",
            okAction: () => SceneManager.LoadScene("SingleGame"),
            cancelAction: () => UI_Manager.Instance.popup.Show("싱글플레이를 취소하였습니다.", "확인")
        );
    }

    private void StartMultiPlay()
    {
        UI_Manager.Instance.popup.Show(
            "멀티플레이를 시작하시겠습니까?",
            "시작", "취소",
            okAction: () => SceneManager.LoadScene("MultiGame"),
            cancelAction: () => UI_Manager.Instance.popup.Show("멀티플레이를 취소하였습니다.", "확인")
        );
    }


    public void OnClick_Menu(int idx)
    {
        switch (idx)
        {
            case 0:
                Debug.Log("대국 시작"); // 싱글플레이인지 멀티플레이인지 선택하는 팝업뜸
                UI_Manager.Instance.Show(UI_Manager.PanelType.GameSelect);
                break;
            case 1:
                Debug.Log("내 기보");
                UI_Manager.Instance.Show(UI_Manager.PanelType.Record);
                break;
            case 2:
                Debug.Log("랭킹");
                UI_Manager.Instance.Show(UI_Manager.PanelType.Ranking);
                break;
            case 3:
                Debug.Log("상점");
                UI_Manager.Instance.Show(UI_Manager.PanelType.Shop);
                break;
            case 4:
                Debug.Log("설정");
                UI_Manager.Instance.Show(UI_Manager.PanelType.Option);
                break;
            default: Debug.Log("???"); break;
        }
    }

}
