using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WB;


public class ShopManager : UI_Panel
{
    public static ShopManager Instance{get; private set;}
    
    [Header("상점 스크롤 뷰 필수 할당")]
    [SerializeField] ScrollViewSet scrollViewSet;

    [Header("필수 할당")]
    [SerializeField] Sprite[] itemSprites;
    [SerializeField] string[] itemNames;
    [SerializeField] int[] nums;
    [SerializeField] int[] prices;

    public Button btnClose;
    
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
    }
    
    void Start()
    {
        UI_Manager.Instance.AddPanel(panelType, this);
        btnClose.onClick.AddListener(Hide);
        gameObject.SetActive(false);
    }
    
    public void SetScrollView(ScrollViewSet scrollViewSet)
    {
        this.scrollViewSet = scrollViewSet;
    }

    public void GetItemData() // 아이템 팝업 오픈 시 호출
    {
        scrollViewSet.StageSelectPopSet(GetMaxCellNum());
    }

    public void BuyCoin(int index) // 셀 클릭 시 코인 획득
    {
        UserSession userSession = SessionManager.GetSession(SessionManager.currentUserId);

        userSession.Coins = userSession.Coins + nums[index];

        SessionManager.UpdateSession(SessionManager.currentUserId, userSession.Coins, userSession.Grade, userSession.RankPoint);
    }

    public int GetMaxCellNum() => itemNames.Length;

    public Sprite GetSprite(int index) 
        => (itemSprites.Length > index) ? itemSprites[index] : null;
    public string GetName(int index) 
        => (itemNames.Length > index) ? itemNames[index] : null;

    public int GetNum(int index) 
        => (nums.Length > index) ? nums[index] : 0;

    public int GetPrice(int index)
        => (prices.Length > index) ? prices[index] : 0;


    public override void Show()
    {
        gameObject.SetActive(true);
        RefreshShopItems();
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
        UI_Manager.Instance.panels[UI_Manager.PanelType.Main].gameObject.SetActive(true);
    }

    public override void OnEnable() { }

    public override void OnDisable() { }
    
    public void RefreshShopItems()
    {
        scrollViewSet.StageSelectPopSet(GetMaxCellNum());
    }
}