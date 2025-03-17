using Newtonsoft.Json;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using workspace.YU__FFE.Scripts.Common;
using workspace.YU__FFE.Scripts.User;

namespace workspace.YU__FFE.Scripts.Server.Network
{
    public class NetworkManager : Singleton<NetworkManager>
    {
        private const string ServerUrl = Constants.ServerURL;

        // 회원가입 요청 함수
        public IEnumerator SignUpRequest(System.Action<bool, string, string, string> callback)
        {
            string url = $"{ServerUrl}signup"; // 회원가입 API 엔드포인트
            PlayerData playerData = PlayerManager.Instance.playerData;
            WWWForm form = new WWWForm();
            form.AddField("id", playerData.id);
            form.AddField("password", playerData.password); // 비밀번호만 먼저 전송
            form.AddField("nickname", playerData.nickname); // 닉네임은 나중에 추가할 수 있음 (회원가입 성공 후 처리)

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // 서버로부터 응답 받은 JSON 처리
                string jsonResponse = request.downloadHandler.text;
                var response = JsonConvert.DeserializeObject<SignUpResponse>(jsonResponse);

                if (response.success)
                {
                    callback(true, response.message, response.refreshToken, response.sessionToken);
                }
                else
                {
                    callback(false, response.message, null, null);
                }
            }
            else
            {
                callback(false, "서버와의 연결이 실패했습니다.", null, null);
            }
        }

        // 로그인 요청 함수
        public IEnumerator SignInRequest(System.Action<bool, string> callback)
        {
            string url = $"{ServerUrl}login"; // 로그인 API 엔드포인트
            PlayerData playerData = PlayerManager.Instance.playerData;
            WWWForm form = new WWWForm();
            form.AddField("id", playerData.id);
            form.AddField("password", playerData.password);

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // 서버로부터 응답 받은 JSON 처리
                string jsonResponse = request.downloadHandler.text;
                var response = JsonConvert.DeserializeObject<SignInResponse>(jsonResponse);

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
        public static IEnumerator UpdateDataRequest(int coins, int grade, int rankPoint, System.Action<bool, string> callback)
        {
            string url = $"{ServerUrl}updateSession"; // 세션 업데이트 API 엔드포인트
            string SessionToken = Session.SessionManager.Instance.SessionToken;
            WWWForm form = new WWWForm();
            form.AddField("userId", SessionToken);
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
            string url = $"{ServerUrl}autoLogin"; // 자동 로그인 API 엔드포인트
            string SessionToken = PlayerPrefs.GetString("SessionToken", "");

            if (string.IsNullOrEmpty(SessionToken))
            {
                callback(false, "저장된 로그인 정보가 없습니다.");
                yield break;
            }

            WWWForm form = new WWWForm();
            form.AddField("userId", SessionToken);

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                var response = JsonConvert.DeserializeObject<SignInResponse>(jsonResponse);

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

        // UserData 저장 요청 함수
        public IEnumerator SaveUserDataRequest(System.Action<bool, string> callback)
        {
            string url = $"{ServerUrl}saveUserData"; // UserData 저장 API 엔드포인트
            string token = Session.SessionManager.Instance.SessionToken;

            // 세션 검증을 위한 헤더 추가
            if (string.IsNullOrEmpty(token))
            {
                callback(false, "세션 토큰이 유효하지 않습니다.");
                yield break;
            }

            PlayerData playerData = PlayerManager.Instance.playerData;
            
            WWWForm form = new WWWForm();
            form.AddField("userId", playerData.id);
            form.AddField("nickname", playerData.nickname);
            form.AddField("password", playerData.password);
            form.AddField("profileNum", playerData.profileNum);
            form.AddField("coins", playerData.coins);
            form.AddField("grade", playerData.grade);
            form.AddField("rankPoint", playerData.rankPoint);
            form.AddField("winCount", playerData.winCount);
            form.AddField("loseCount", playerData.loseCount);

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            request.SetRequestHeader("Authorization", "Bearer " + token); // 세션 토큰을 헤더에 추가

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

        // UserData 가져오기 요청 함수
        public static IEnumerator GetUserDataRequest(string userId, System.Action<bool, PlayerData, string> callback)
        {
            string url = $"{ServerUrl}getUserData"; // UserData 가져오기 API 엔드포인트
            WWWForm form = new WWWForm();
            form.AddField("userId", userId);

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // 서버로부터 응답 받은 JSON 처리
                string jsonResponse = request.downloadHandler.text;
                var response = JsonConvert.DeserializeObject<GetUserDataResponse>(jsonResponse);

                if (response.success)
                {
                    // 서버에서 가져온 UserData를 전달
                    PlayerData playerData = new PlayerData(
                        response.nickname,
                        response.profileNum,
                        response.coins,
                        response.grade,
                        response.rankPoint,
                        response.winCount,
                        response.loseCount
                    );

                    callback(true, playerData, response.message);
                }
                else
                {
                    callback(false, null, response.message);
                }
            }
            else
            {
                callback(false, null, "서버와의 연결이 실패했습니다.");
            }
        }
    }

    // 응답 구조체들
    public class SignInResponse { public bool success; public string message; }
    public class BaseResponse { public bool success; public string message; }
    public class SignUpResponse { public bool success; public string message; public string refreshToken; public string sessionToken; }
    public class GetUserDataResponse : BaseResponse { public string nickname; public int profileNum; public int coins; public int grade; public int rankPoint; public int winCount; public int loseCount; }
}
