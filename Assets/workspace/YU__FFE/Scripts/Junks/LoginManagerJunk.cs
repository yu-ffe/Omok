using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

// 서버 연결 -> 로그인 테스트
<<<<<<< HEAD:Assets/workspace/YU__FFE/Scripts/Junks/LoginManagerJunk.cs
namespace workspace.YU__FFE.Scripts.Junks {
    /// <summary>
    /// 더 이상 동작하지 않음: 서버 코드 수정
    /// </summary>
    public class LoginManagerJunk : MonoBehaviour {
=======
namespace workspace.YU__FFE.Scripts.Server {
    public class LoginManager : MonoBehaviour {
>>>>>>> parent of b203007 (add: NetworkManager 기능 추가 및 구현 (테스트 X)):Assets/workspace/YU__FFE/Scripts/Server/LoginManager.cs
        public TextMeshProUGUI loginIdText; // coins 값 출력
        public TMP_InputField nicknameInput;
        public TMP_InputField passwordInput;
        public Button loginButton;
        public TextMeshProUGUI statusText; // 로그인 상태 표시

        private string serverUrl = "http://localhost/auth/login_json"; // Express 서버 URL

        void Start() {
            Debug.Log("LoginManager 시작");
            loginButton.onClick.AddListener(() => StartCoroutine(TryLogin()));
        }

        IEnumerator TryLogin() {
            Debug.Log("로그인 시도 중...");
            string nickname = nicknameInput.text;
            string password = passwordInput.text;

            if (string.IsNullOrEmpty(nickname) || string.IsNullOrEmpty(password)) {
                Debug.LogWarning("닉네임이나 비밀번호가 비어있음");
                statusText.text = "닉네임과 비밀번호를 입력하세요.";
                yield break;
            }

            string jsonData = $"{{\"nickname\":\"{nickname}\",\"password\":\"{password}\"}}";
            Debug.Log("전송할 JSON 데이터: " + jsonData);
            byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);

            using (UnityWebRequest request = new UnityWebRequest(serverUrl, "POST")) {
                request.uploadHandler = new UploadHandlerRaw(jsonBytes);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                Debug.Log("서버에 요청 전송 중...");
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success) {
                    string responseText = request.downloadHandler.text;
                    Debug.Log("서버 응답: " + responseText);

                    // JSON 응답인지 확인
                    if (!responseText.Trim().StartsWith("{")) {
                        statusText.text = "서버에서 예상치 못한 응답을 보냈습니다.";
                        Debug.LogError("서버 응답이 JSON이 아닙니다: " + responseText);
                        yield break;
                    }

                    try {
                        Debug.Log("응답을 JSON으로 파싱 중...");
                        UserSession userSession = JsonUtility.FromJson<UserSession>(responseText);
                        loginIdText.text = $"Coins: {userSession.Coins}";
                        statusText.text = "✅ 로그인 성공!";
                        Debug.Log("로그인 성공: " + userSession.Nickname);
                    }
                    catch (System.Exception e) {
                        Debug.LogError("JSON 파싱 오류: " + e.Message);
                        statusText.text = "서버 응답 처리 중 오류 발생";
                    }
                }
                else {
                    statusText.text = "❌ 로그인 실패";
                    Debug.LogError("로그인 실패: " + request.error);
                    loginIdText.text = "로그인 실패";
                }
            }
        }

    }
}
