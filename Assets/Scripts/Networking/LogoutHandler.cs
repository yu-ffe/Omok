

public class LogoutManager : Singleton<LogoutManager> {

    // ======================================================
    //                      로그아웃
    // ======================================================
    
    /// <summary>
    /// 로그아웃 과정에서 로컬의 데이터를 서버에 업로드하는 기능은 없습니다. (보안)
    ///
    /// 1. Refresh Token 제거 - 클라이언트 내
    /// 2. Login 창으로 이동
    /// </summary>
    public void SignOut() {
        SessionManager.Instance.ClearTokens();
        UI_Manager.Instance.Show(UI_Manager.PanelType.Login);
    }
}
