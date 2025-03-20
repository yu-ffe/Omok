namespace workspace.YU__FFE.Scripts.Networking {

    public class LogoutManager : Singleton<LogoutManager> {

        // ======================================================
        //                      로그아웃
        // ======================================================

        public void SignOut() {
            SessionManager.Instance.ClearTokens();
            UI_Manager.Instance.Show(UI_Manager.PanelType.Login);
        }
    }

}
