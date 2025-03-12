using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WB;

public class WBTest : MonoBehaviour
{
    public string key;

    [ContextMenu("ShowPanel")]
    void ShowPanel()
    {
        UI_Manager.Instance.Show(key);
    }

    [ContextMenu("HidePanel")]
    void HidePanel()
    {
        UI_Manager.Instance.Hide(key);
    }


    public BasicPopup basicPopup;
    public string popupMsg;
    [ContextMenu("Popup Test")]
    void PopupTest()
    {
        basicPopup.Show(
            msg: popupMsg,
            okText: "okok",
            cancelText: "nono",
            800, 1000,
            () => { Debug.Log("Click OK"); },
            () => { Debug.Log("Click Cancel"); });
    }
}
