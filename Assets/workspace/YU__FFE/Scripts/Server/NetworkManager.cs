using Newtonsoft.Json;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using workspace.YU__FFE.Scripts.Common;

namespace workspace.YU__FFE.Scripts.Server
{

    public static class NetworkManager {
        private const string ServerUrl = Constants.ServerURL; // 서버 URL

        // 로그인 요청 함수
        public static IEnumerator LoginRequest(string userId, string password, System.Action<bool, string> callback)
        {
            string url = ServerUrl + "login"; // 로그인 API 엔드포인트
            WWWForm form = new WWWForm();
            form.AddField("userId", userId);
            form.AddField("password", password);

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // 서버로부터 응답 받은 JSON 처리
                string jsonResponse = request.downloadHandler.text;
                var response = JsonConvert.DeserializeObject<LoginResponse>(jsonResponse);

                if (response.success)
                {
                    callback(true, response.message);
                }
                else
                {
                    callback(false, response.message);
                }
            }
            else
            {
                callback(false, "서버와의 연결이 실패했습니다.");
            }
        }

        // 세션 업데이트 요청 함수
        public static IEnumerator UpdateSessionRequest(string userId, int coins, int grade, int rankPoint, System.Action<bool, string> callback)
        {
            string url = ServerUrl + "updateSession"; // 세션 업데이트 API 엔드포인트
            WWWForm form = new WWWForm();
            form.AddField("userId", userId);
            form.AddField("coins", coins);
            form.AddField("grade", grade);
            form.AddField("rankPoint", rankPoint);

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // 서버로부터 응답 받은 JSON 처리
                string jsonResponse = request.downloadHandler.text;
                var response = JsonConvert.DeserializeObject<BaseResponse>(jsonResponse);

                if (response.success)
                {
                    callback(true, response.message);
                }
                else
                {
                    callback(false, response.message);
                }
            }
            else
            {
                callback(false, "서버와의 연결이 실패했습니다.");
            }
        }

        // 자동 로그인 시도
        public static IEnumerator TryAutoLogin(System.Action<bool, string> callback)
        {
            string url = ServerUrl + "autoLogin"; // 자동 로그인 API 엔드포인트
            string userId = PlayerPrefs.GetString("AutoLoginId", "");

            if (string.IsNullOrEmpty(userId))
            {
                callback(false, "저장된 로그인 정보가 없습니다.");
                yield break;
            }

            WWWForm form = new WWWForm();
            form.AddField("userId", userId);

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                var response = JsonConvert.DeserializeObject<LoginResponse>(jsonResponse);

                if (response.success)
                {
                    callback(true, response.message);
                }
                else
                {
                    callback(false, response.message);
                }
            }
            else
            {
                callback(false, "서버와의 연결이 실패했습니다.");
            }
        }
    }

    // 로그인 응답 구조체
    public class LoginResponse
    {
        public bool success;
        public string message;
    }

    // 기본 응답 구조체
    public class BaseResponse
    {
        public bool success;
        public string message;
    }
}
