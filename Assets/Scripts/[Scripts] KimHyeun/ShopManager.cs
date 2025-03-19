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
    [SerializeField] string[] itemNames =
    {
        "Small Coin Pack", "Medium Coin Pack" , "Large Coin Pack",
        "Match Win Rate Analysis Ticket", "Ranking Score Protection Ticket",
        "Premium Profile Icon", "Exclusive Chat Theme", "Special Event Entry Ticket"
        // 소형 코인 팩, 중형 코인 팩, 대형 코인 팩, 경기 승률 분석권, 
        // 랭킹 점수 보호권, 프리미엄 프로필 아이콘,
        // 특별한 채팅 테마, 특별 이벤트 참가권 
    };
    [SerializeField] public int[] nums = { 100, 500, 1000, 0, 0, 0, 0, 0 }; //코인지급량
    [SerializeField] public int[] prices = { 1000, 4500, 8500, 3000, 5000, 2000, 4500, 6000 }; // 원화 가격
    [SerializeField] public bool[] isCoinItem = { true, true, true, false, false, false, false, false }; // 코인 아이템 여부
    
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
    
    public override void Show()
    {
        UI_Manager.Instance.panels[UI_Manager.PanelType.Main].gameObject.SetActive(true);

        gameObject.SetActive(true);
        //RefreshShopItems();
        UpdateShopItems();
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
    
    /// <summary>
    /// 'ScrollViewSet'에 상점 아이템 데이터 설정
    /// </summary>
    /// <param name="scrollViewSet"></param>
    public void SetScrollView(ScrollViewSet scrollViewSet)
    {
        this.scrollViewSet = scrollViewSet;
    }

    public void GetItemData() // 아이템 팝업 오픈 시 호출
    {
        scrollViewSet.StageSelectPopSet(GetMaxCellNum());
    }

    public bool BuyCoin(int index)
    {
        bool paymentSuccess = ProcessPayment(prices[index]);

        if (paymentSuccess)
        {
            if (isCoinItem[index])
            {
                // TODO: 구매 관련은 서버에서 동작하는게 좋을듯.
                // PlayerData playerData = SessionManager.GetSession(SessionManager.currentUserId);
                // playerData.coins += nums[index];
                // SessionManager.UpdateSession(SessionManager.currentUserId, userSession.Coins, userSession.Grade, userSession.RankPoint);
            }
            else
            {
                GrantSpecialItem(index);
            }

            return true; // 구매 성공
        }

        return false; // 구매 실패
    }
    
    /// <summary>
    /// 비코인 아이템 구매 처리 (특별 아이템)
    /// </summary>
    private void GrantSpecialItem(int index)
    {
        switch (index)
        {
            case 3:
                UI_Manager.Instance.popup.Show($"경기 승률 분석권 지급 완료! 분석 기능을 활성화하세요.");
                // 추가 기능 연동 필요
                break;
            case 4:
                UI_Manager.Instance.popup.Show($"랭킹 점수 보호권 지급 완료! 다음 패배 시 랭킹 점수 보호.");
                // 추가 기능 연동 필요
                break;
            case 5:
                UI_Manager.Instance.popup.Show($"프리미엄 프로필 아이콘 지급 완료! 설정에서 변경하세요.");
                // 추가 기능 연동 필요
                break;
            case 6:
                UI_Manager.Instance.popup.Show($"특별한 채팅 테마 지급 완료! 채팅 설정에서 활성화하세요.");
                // 추가 기능 연동 필요
                break;
            case 7:
                UI_Manager.Instance.popup.Show($"특별 이벤트 참가권 지급 완료! 이벤트 창에서 참가하세요.");
                // 추가 기능 연동 필요
                break;
            default:
                UI_Manager.Instance.popup.Show($"아이템 구매에 실패하였습니다.");
                Debug.LogError("알 수 없는 아이템입니다.");
                break;
        }
    }
    
    /// <summary>
    /// `ScrollViewSet`에 상점 아이템 데이터 전달
    /// </summary>
    public void UpdateShopItems()
    {
        scrollViewSet.StageSelectPopSet(GetMaxCellNum());
    }

    /// <summary>
    /// 결제 처리 (임의로 추가, 실제 결제 API 연동 필요)
    /// </summary>
    private bool ProcessPayment(int amount)
    {
        Debug.Log($"{amount} 원 결제");
        return true;
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


   
}