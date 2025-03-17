using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace workspace.YU__FFE.Scripts.Server {
    public class NetworkControllerTest : MonoBehaviour {
        private NetworkManager networkManager;
        
        public TMP_InputField usernameInput;
        public TMP_InputField nicknameInput;
        public TMP_InputField passwordInput;
        public Button loginButton;
        public Button RegisterButton;
        public Button LogoutButton;

        public void Start() {
            networkManager = NetworkManager.Instance;
            loginButton.onClick.AddListener(() => StartCoroutine(TestSignin()));
            RegisterButton.onClick.AddListener(() => StartCoroutine(TestSignup()));
            LogoutButton.onClick.AddListener(() => StartCoroutine(TestLogout()));
        }

        // 회원가입 테스트
        public IEnumerator TestSignup() {
            NetworkManager.SignupData signupData = new NetworkManager.SignupData {
                username = usernameInput.text,
                nickname = nicknameInput.text,
                password = passwordInput.text
            };

            yield return StartCoroutine(networkManager.Signup(signupData, 
                () => {
                    Debug.Log("회원가입 성공");
                },
                () => {
                    Debug.Log("회원가입 실패");
                }));
        }

        // 로그인 테스트
        public IEnumerator TestSignin() {
            NetworkManager.SigninData signinData = new NetworkManager.SigninData {
                username = usernameInput.text,
                password = passwordInput.text
            };

            yield return StartCoroutine(networkManager.Signin(signinData,
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
            yield return StartCoroutine(networkManager.Signout(
                () => {
                    Debug.Log("로그아웃 성공");
                },
                () => {
                    Debug.Log("로그아웃 실패");
                }));
        }

    }
}
