using System.Collections;
using System.Collections.Generic;
using KimHyeun;
using UnityEngine;
using WB;

public class RecordPanel : UI_Panel
{
    public ScrollViewSet scrollView;

    void Start()
    {
        UI_Manager.Instance.AddPanel(UI_Manager.PanelType.Record, this);
        gameObject.SetActive(false);
    }

    public override void Show()
    {
        // 매니저 스크롤 뷰에 해당 스크롤 뷰 할당
        RecordManager.Instance.SetScrollView(scrollView);
        RecordManager.Instance.GetRecordData();
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
