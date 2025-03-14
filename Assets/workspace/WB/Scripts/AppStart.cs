using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WB
{
    public class AppStart : MonoBehaviour
    {

        void Awake()
        {
            SessionManager.LoadAllSessions();
        }
        void Start()
        {
            //앱 초기 세팅
            Debug.Log("App Start");
            if (!string.IsNullOrEmpty(SessionManager.currentUserId))
            {
                UI_Manager.Instance.Show(UI_Manager.PanelType.Main);
                gameObject.SetActive(false);
            }
            else
            {
                UI_Manager.Instance.Show(UI_Manager.PanelType.Login);
                gameObject.SetActive(false);
            }
        }

        public void Onclick_AppStart()
        {

            UI_Manager.Instance.Show(UI_Manager.PanelType.Login);

            gameObject.SetActive(false);
        }

    }
}
