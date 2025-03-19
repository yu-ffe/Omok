using TMPro;
using UnityEngine;
using UnityEngine.UI;
using workspace.YU__FFE.Scripts.User;

namespace workspace.YU__FFE.Scripts.Test {
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
        public Button autoLoginButton;
        public Button updateToken;
        
        public TextMeshProUGUI statusText; // 로그인 상태 표시
        
        public Image profileImage; // 프로필 이미지 추가

        public void Start() {
            //registerButton.onClick.AddListener(SignUp);
            //loginButton.onClick.AddListener(SignIn);
            //autoLoginButton.onClick.AddListener(AutoSignIn);
            //updateToken.onClick.AddListener(UpdateToken);
            
            if (Server.Session.SessionManager.Instance == null) {
                Debug.LogError("[NetworkTesterHandler] SessionManager가 초기화되지 않았습니다.");
                return;
            }

            if (profileImage == null) {
                Debug.LogError("[NetworkTesterHandler] Profile Image UI가 설정되지 않았습니다. Inspector에서 연결하세요.");
                return;
            }

            string testUserId = "testUser123"; // ✅ 임의의 테스트 유저 ID 사용
            Debug.Log($"[NetworkTesterHandler] 테스트용 userId: {testUserId}");

            // ✅ 프로필 이미지 로드
            Server.Session.SessionManager.Instance.GetUserProfileImage(testUserId, profileImage);
        }
        
        public void SignUp() {
            User.SignUpHandler.Instance.TrySignUp(id_email.text, password.text, password.text, nickname.text,  int.Parse(imageNum.text),
                (b, s) => {
                    Debug.Log(s);
                    statusText.text = s;
                });
        }
        
        public void SignIn() {
            User.SignInHandler.Instance.TrySignIn(id_email.text, password.text, (b, s) => {
                statusText.text = s;
            });
        }

        public void AutoSignIn() {
            SignInHandler.Instance.SetAutoLoginEnabled(true);
            StartCoroutine(SignInHandler.Instance.AttemptAutoLogin((b, s) => {
                statusText.text = s;
            }));
        }
        
        public void UpdateToken() {
            coinText.text = PlayerManager.Instance.playerData.coins.ToString();
            sessionToken.text = Server.Session.SessionManager.Instance.GetAccessToken();
            freshToken.text = Server.Session.SessionManager.Instance.GetRefreshToken();
            statusText.text = "토큰 갱신 완료";
        }
        
    }
}
