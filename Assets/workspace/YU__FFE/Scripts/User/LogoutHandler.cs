using WB;

namespace workspace.YU__FFE.Scripts.User {

    public class LogoutManager : Singleton<LogoutManager> {
        /// <summary>
        /// 1. Refresh Token 제거 - 클라이언트 내
        /// 2. Session Token 지우기
        /// 3. UI 로그인 화면으로 이동
        /// </summary>
        public void SignOut() {
            Server.Session.SessionManager.Instance.ClearTokens();
            UI_Manager.Instance.Show(UI_Manager.PanelType.Login);
        }
    }
}
