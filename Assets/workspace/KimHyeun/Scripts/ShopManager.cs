using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KimHyeun {
    public class ShopManager : Singleton<ShopManager>
    {
        [Header("상점 스크롤 뷰 필수 할당")]
        [SerializeField] ScrollViewSet scrollViewSet;

        [Header("필수 할당")]
        [SerializeField] Sprite[] itemSprites;
        [SerializeField] string[] itemNames;
        [SerializeField] int[] nums;
        [SerializeField] int[] prices;


        

        public void GetItemData() // 아이템 팝업 오픈 시 호출
        {
            scrollViewSet.StageSelectPopSet(GetMaxCellNum());
        }



        public void BuyCoin(int index) // 셀 클릭 시 코인 획득
        {
            UserSession userSession = SessionManager.GetSession(SessionManager.currentUserId);

            userSession.Coins = userSession.Coins + nums[index];

            SessionManager.UpdateSession(SessionManager.currentUserId, userSession.Coins, userSession.Grade, userSession.RankPoint);


            /* // 테스트 코드
            UserSession userSession = SessionManager.GetSession(SessionManager.GetAllUserIds()[0]);    
            userSession.Coins = userSession.Coins + nums[index];
            SessionManager.UpdateSession(SessionManager.GetAllUserIds()[0], userSession.Coins, userSession.Grade, userSession.RankPoint);
            */
        }








        public int GetMaxCellNum()
        {
            return itemNames.Length;
        }



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

        public int GetNum(int index)
        {
            if (nums.Length > index)
            {
                return nums[index];
            }

            else
            {
                return 0;
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










        /*

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
        }*/
    }
}

