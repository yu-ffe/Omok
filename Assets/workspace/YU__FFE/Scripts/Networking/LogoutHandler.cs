namespace workspace.YU__FFE.Scripts.Networking {

    public class LogoutManager : Singleton<LogoutManager> {

        public void SignOut() {
            
            TokenManager.Instance.ClearTokens();
            UI_Manager.Instance.Show(UI_Manager.PanelType.Login);
        }
    }

}
