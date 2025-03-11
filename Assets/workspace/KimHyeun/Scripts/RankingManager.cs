using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankingManager : MonoBehaviour
{
    public Sprite profileSprite;
    public string nickName;
    public int level;
    public int win;
    public int lose;



    void GetUserData() // ��ŷ �˾� ���� �� ȣ��
    {
        // ���� ������ �ҷ����� �ʿ�  (playerpref ��� �ҷ�����)
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

    public int GetLevel()
    {
        return level;
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
