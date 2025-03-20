using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WB;
namespace WB
{
    public class BasicPopup : UI_Popup
    {
        bool isComponentConnected;
        
        public override void ShowStartEvent()
        {
            Debug.Log($"Show Start Event {gameObject.name} = Component Check");
            if (!isComponentConnected)
            {
                if (objPopup == null)
                    objPopup = this.gameObject;

                if (rectWindow == null)
                    rectWindow = transform.GetChild(0).GetComponent<RectTransform>();

                if (textMsg == null)
                    textMsg = rectWindow.GetChild(0).GetComponent<TextMeshProUGUI>();

                if (scoreBoard == null)
                    scoreBoard = rectWindow.GetChild(1).GetComponent<UserScorePopup>();

                if (btnOk == null)
                    btnOk = rectWindow.GetChild(2).GetComponent<Button>();

                if (btnCancel == null)
                    btnCancel = rectWindow.GetChild(3).GetComponent<Button>();

                if (textOk == null)
                    textOk = btnOk.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

                if (textCancel == null)
                    textCancel = btnCancel.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

                isComponentConnected = true;
            }
        }
    }


}
