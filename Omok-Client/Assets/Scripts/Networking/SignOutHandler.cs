

using UnityEngine;

public class SignOutHandler : MonoBehaviour {

    // ======================================================
    //                      로그아웃
    // ======================================================
    
    public void SignOut() {
        
        UI_Manager.Instance.popup.Show(
            "로그아웃 하시겠습니까?",
            "로그아웃", "취소",
            okAction: () =>
            {
                TokenManager.Instance.ClearTokens();
                UI_Manager.Instance.Hide(UI_Manager.PanelType.Main);
                UI_Manager.Instance.Show(UI_Manager.PanelType.Login);
                UI_Manager.Instance.Show(UI_Manager.PanelType.Login);
            },
            cancelAction: () => UI_Manager.Instance.popup.Show("로그아웃을 취소하였습니다.", "확인")
        );
        
        
        
    }
}
