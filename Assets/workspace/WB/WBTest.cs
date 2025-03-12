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
        UI_Manager.Get.Show(key);
    }
}
