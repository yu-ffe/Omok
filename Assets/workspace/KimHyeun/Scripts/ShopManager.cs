using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public Sprite itemSprite;
    public string itemName;
    public int price;




    void GetItemData() // ������ �˾� ���� �� ȣ��
    {
        // ������ ������ �ҷ����� �ʿ�
    }


    // TODO �� Ŭ�� �� ���� ȹ��











    public Sprite GetSprite()
    {
        return itemSprite;
    }

    public string GetName()
    {
        return itemName;
    }

    public int GetPrice()
    {
        return price;
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
