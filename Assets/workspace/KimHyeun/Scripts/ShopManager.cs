using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] ScrollViewSet scrollViewSet;

    [SerializeField] Sprite[] itemSprites;
    [SerializeField] string[] itemNames;
    [SerializeField] int[] prices;


    
    

    public void GetItemData() // 아이템 팝업 오픈 시 호출
    {
        // 아이템 데이터 불러오기 필요

        //SessionManager.UserSession userSession = SessionManager.GetSession(SessionManager.currentUserId);



        scrollViewSet.StageSelectPopSet();
    }


    // TODO 셀 클릭 시 코인 획득











    public Sprite GetSprite(int index)
    {
        if (itemSprites.Length > index)
        {
            return itemSprites[index];
        }

        else
        {
            return null;
        }
    }

    public string GetName(int index)
    {
        if (itemNames.Length > index)
        {
            return itemNames[index];
        }

        else
        {
            return null;
        }
    }

    public int GetPrice(int index)
    {
        if (prices.Length > index)
        {
            return prices[index];
        }

        else
        {
            return 0;
        }
    }












    private static ShopManager _instance;

    public static ShopManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ShopManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(ShopManager).Name;
                    _instance = obj.AddComponent<ShopManager>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this as ShopManager;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
