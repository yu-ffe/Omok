using System.Collections;
using System.Collections.Generic;
using KimHyeun;
using UnityEngine;

public class RankingPanel : WB.UI_Panel
{
    public ScrollViewSet scrollView;


    public override void Show()
    {
        // 매니저 스크롤 뷰에 해당 스크롤 뷰 할당
        // RankingManager.Instance.SetScrollView(scrollView);
        RankingManager.Instance.GetUserData();
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
