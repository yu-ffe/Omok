using System.Collections;
using System.Collections.Generic;
using KimHyeun;
using UnityEngine;
using WB;

public class RecordPanel : UI_Panel
{
    public ScrollViewSet scrollView;


    public override void Show()
    {
        // 매니저 스크롤 뷰에 해당 스크롤 뷰 할당
        // RecordManager.Instance.SetScrollView(scrollView);
        RecordManager.Instance.GetRecordData();
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
