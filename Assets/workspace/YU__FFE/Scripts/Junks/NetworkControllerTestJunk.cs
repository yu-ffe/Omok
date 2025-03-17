using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace workspace.YU__FFE.Scripts.Junks {
    public class NetworkControllerTestJunk : MonoBehaviour {
        private NetworkManagerJunk _networkManagerJunk;
        
        public TMP_InputField usernameInput;
        public TMP_InputField nicknameInput;
        public TMP_InputField passwordInput;
        public Button loginButton;
        public Button RegisterButton;
        public Button LogoutButton;

        public void Start() {
            _networkManagerJunk = NetworkManagerJunk.Instance;
            loginButton.onClick.AddListener(() => StartCoroutine(TestSignin()));
            RegisterButton.onClick.AddListener(() => StartCoroutine(TestSignup()));
            LogoutButton.onClick.AddListener(() => StartCoroutine(TestLogout()));
        }

        // 회원가입 테스트
        public IEnumerator TestSignup() {
            NetworkManagerJunk.SignupData signupData = new NetworkManagerJunk.SignupData {
                username = usernameInput.text,
                nickname = nicknameInput.text,
                password = passwordInput.text
            };

            yield return StartCoroutine(_networkManagerJunk.Signup(signupData, 
                () => {
                    Debug.Log("회원가입 성공");
                },
                () => {
                    Debug.Log("회원가입 실패");
                }));
        }

        // 로그인 테스트
        public IEnumerator TestSignin() {
            NetworkManagerJunk.SigninData signinData = new NetworkManagerJunk.SigninData {
                username = usernameInput.text,
                password = passwordInput.text
            };

            yield return StartCoroutine(_networkManagerJunk.Signin(signinData,
                () => { 
                    Debug.Log("로그인 성공"); 
                },
                (errorCode) => { 
                    Debug.Log("로그인 실패: " + errorCode); 
                }));
        }
        
        public IEnumerator TestLogout()
        {
            // Signout 메소드가 반환하는 IEnumerator를 올바르게 처리
            yield return StartCoroutine(_networkManagerJunk.Signout(
                () => {
                    Debug.Log("로그아웃 성공");
                },
                () => {
                    Debug.Log("로그아웃 실패");
                }));
        }

    }
}
