using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingPanel : UI_Panel
{
    void Start()
    {
        UI_Manager.Instance.AddPanel(UI_Manager.PanelType.Loading, this);
    }
    public override void Show()
    {
        gameObject.SetActive(true);

    }

    public override void Hide()
    {
        gameObject.SetActive(false);
    }

    public override void OnEnable()
    {
    }

    public override void OnDisable()
    {
    }
}
