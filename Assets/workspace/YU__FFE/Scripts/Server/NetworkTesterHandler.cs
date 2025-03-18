using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using workspace.YU__FFE.Scripts.User;

namespace workspace.YU__FFE.Scripts.Server {
    public class NetworkTesterHandler : MonoBehaviour {
        
        public TextMeshProUGUI coinText; // coins 값 출력: 서버 연결 테스트 확인용
        public TextMeshProUGUI sessionToken; // diamonds 값 출력: 서버 연결 테스트 확인용
        public TextMeshProUGUI freshToken; // diamonds 값 출력: 서버 연결 테스트 확인용
        
        public TMP_InputField id_email;
        public TMP_InputField nickname;
        public TMP_InputField password;
        public TMP_InputField imageNum;
        
        public Button registerButton;
        public Button loginButton;
        public Button updateToken;
        
        public TextMeshProUGUI statusText; // 로그인 상태 표시

        public void Start() {
            registerButton.onClick.AddListener(SignUp);
            loginButton.onClick.AddListener(SignIn);
            updateToken.onClick.AddListener(UpdateToken);
        }
        
        public void SignUp() {
            User.SignUpManager.Instance.TrySignUp(id_email.text, password.text, password.text, nickname.text,  int.Parse(imageNum.text),
                (b, s) => {
                    Debug.Log(s);
                    statusText.text = s;
                });
        }
        
        public void SignIn() {
            User.SignInManager.TryLogin(id_email.text, password.text, (b, s) => {
                Debug.Log(s);
                statusText.text = s;
            });
        }
        
        public void UpdateToken() {
            coinText.text = PlayerManager.Instance.playerData.coins.ToString();
            sessionToken.text = Session.SessionManager.Instance.GetSessionToken();
            freshToken.text = Session.SessionManager.Instance.GetRefreshToken();
            statusText.text = "토큰 갱신 완료";
        }
        
    }
}
