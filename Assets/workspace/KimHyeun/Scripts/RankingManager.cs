using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RankingManager : MonoBehaviour
{
    [SerializeField] ScrollViewSet scrollViewSet;

    public Sprite profileSprite;
    public string nickName;
    public int Grade;
    public int win;
    public int lose;

    
    public void GetUserData() // ��ŷ �˾� ���� �� ȣ��
    {
        // ���� ������ �ҷ����� �ʿ�  (playerpref ��� �ҷ�����) - ȸ������, ������ ���� (�г���) (�޼�,��,��)

        
        // ��� ���� id ã��, �ش� �������� ����(��ųʸ� ����) ����


        List<string> userIdList = SessionManager.GetAllUserIds();

        for (int i = 0; i < userIdList.Count; i++)
        {
            SessionManager.UserSession userSession = SessionManager.GetSession(userIdList[i]);

            nickName = userSession.Nickname;
            Grade = userSession.Grade;
           // win = userSession.
            // lose = userSession.
        }


        



        scrollViewSet.StageSelectPopSet();
    }


    


    // TODO �޼�(level) ��� ��ŷ(���� ���� ����), ���� �޼� �� �·� �켱





    public Sprite GetSprite()
    {
        return profileSprite;
    }

    public string GetName()
    {
        return nickName;
    }

    public int GetGrade()
    {
        return Grade;
    }

    public int GetWin()
    {
        return win;
    }

    public int GetLose()
    {
        return lose;
    }











    private static RankingManager _instance;

    public static RankingManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<RankingManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(RankingManager).Name;
                    _instance = obj.AddComponent<RankingManager>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this as RankingManager;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
