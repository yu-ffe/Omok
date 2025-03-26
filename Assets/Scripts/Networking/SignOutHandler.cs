

using UnityEngine;

public class SignOutHandler : MonoBehaviour {

    // ======================================================
    //                      로그아웃
    // ======================================================
    
    public void SignOut() {
        TokenManager.Instance.ClearTokens();
        UI_Manager.Instance.Hide(UI_Manager.PanelType.Main);
        UI_Manager.Instance.Show(UI_Manager.PanelType.Login);
    }
}
