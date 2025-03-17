using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using SimpleJSON; // JSON 파싱 라이브러리

namespace workspace.YU__FFE.Scripts.Server {
    public class LoginManager : MonoBehaviour
    {
        public TextMeshProUGUI loginIdText; // 로그인 ID 표시
        public TMP_InputField nicknameInput;
        public TMP_InputField passwordInput;
        public Button loginButton;
        public TextMeshProUGUI statusText; // 로그인 상태 표시

        private string serverUrl = "http://localhost"; // Express 서버 URL

        void Start()
        {
            loginButton.onClick.AddListener(() => StartCoroutine(TryLogin()));
        }

        IEnumerator TryLogin()
        {
            string nickname = nicknameInput.text;
            string password = passwordInput.text;

            if (string.IsNullOrEmpty(nickname) || string.IsNullOrEmpty(password))
            {
                statusText.text = "닉네임과 비밀번호를 입력하세요.";
                yield break;
            }

            // JSON 데이터 생성
            string jsonData = $"{{\"nickname\":\"{nickname}\",\"password\":\"{password}\"}}";
            byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);

            // 서버에 요청
            using (UnityWebRequest request = new UnityWebRequest($"{serverUrl}/auth/login", "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(jsonBytes);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string responseText = request.downloadHandler.text;
                    Debug.Log("로그인 성공: " + responseText);

                    // JSON 파싱
                    var json = JSON.Parse(responseText);
                    string userId = json["_id"]; // MongoDB ObjectId

                    // 로그인 성공 메시지 출력
                    statusText.text = "✅ 로그인 성공!";
                    loginIdText.text = $"ID: {userId}";
                }
                else
                {
                    statusText.text = "❌ 로그인 실패";
                    Debug.LogError("로그인 실패: " + request.error);
                    loginIdText.text = "로그인 실패";
                }
            }
        }
    }
}
