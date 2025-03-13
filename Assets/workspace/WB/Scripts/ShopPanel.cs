using System.Collections;
using System.Collections.Generic;
using KimHyeun;
using UnityEngine;

public class ShopPanel : WB.UI_Panel
{
    public ScrollViewSet scrollView;


    public override void Show()
    {
        // 매니저 스크롤 뷰에 해당 스크롤 뷰 할당
        // ShopManager.Instance.SetScrollView(scrollView);
        ShopManager.Instance.GetItemData();
    }

    public override void Hide()
    {
    }

    public override void OnDisable()
    {
    }

    public override void OnEnable()
    {
    }

}
