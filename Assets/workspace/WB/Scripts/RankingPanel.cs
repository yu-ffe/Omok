using System.Collections;
using System.Collections.Generic;
using KimHyeun;
using UnityEngine;
using WB;
using workspace.YU__FFE.Scripts;

namespace Wb {
public class RankingPanel : WB.UI_Panel
{
    public ScrollViewSet scrollView;


    void Start()
    {
        WB.UI_Manager.Instance.AddPanel(WB.UI_Manager.PanelType.Ranking, this);
        gameObject.SetActive(false);
    }

    public override void Show()
    {
        // 매니저 스크롤 뷰에 해당 스크롤 뷰 할당
        RankingManager.Instance.SetScrollView(scrollView);
        RankingManager.Instance.GetUserData();
        gameObject.SetActive(true);
    }
    public override void Hide()
    {
        gameObject.SetActive(false);
    }

    public override void OnDisable()
    {
    }

    public override void OnEnable()
    {
    }

    public void OnClick_Back() => UI_Manager.Instance.Show(UI_Manager.PanelType.Main);

}

    
}