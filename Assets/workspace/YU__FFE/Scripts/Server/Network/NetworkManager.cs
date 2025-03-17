using Newtonsoft.Json;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using workspace.YU__FFE.Scripts.Common;
using workspace.YU__FFE.Scripts.User;

namespace workspace.YU__FFE.Scripts.Server.Network {

    public class NetworkManager : Singleton<NetworkManager> {
        private const string ServerUrl = Constants.ServerURL;

        // 로그인 요청 함수
        public static IEnumerator SignInRequest(string userId, string password, System.Action<bool, string> callback) {
            string url = ServerUrl + "login"; // 로그인 API 엔드포인트
            WWWForm form = new WWWForm();
            form.AddField("userId", userId);
            form.AddField("password", password);

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                // 서버로부터 응답 받은 JSON 처리
                string jsonResponse = request.downloadHandler.text;
                var response = JsonConvert.DeserializeObject<LoginResponse>(jsonResponse);

                if (response.success) {
                    callback(true, response.message);
                }
                else {
                    callback(false, response.message);
                }
            }
            else {
                callback(false, "서버와의 연결이 실패했습니다.");
            }
        }

        // 세션 업데이트 요청 함수
        public static IEnumerator UpdateDataRequest(int coins, int grade, int rankPoint, System.Action<bool, string> callback) {
            string url = ServerUrl + "updateSession"; // 세션 업데이트 API 엔드포인트
            string SessionToken = Session.SessionManager.Instance.SessionToken;
            WWWForm form = new WWWForm();
            form.AddField("userId", SessionToken);
            form.AddField("coins", coins);
            form.AddField("grade", grade);
            form.AddField("rankPoint", rankPoint);

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                // 서버로부터 응답 받은 JSON 처리
                string jsonResponse = request.downloadHandler.text;
                var response = JsonConvert.DeserializeObject<BaseResponse>(jsonResponse);

                if (response.success) {
                    callback(true, response.message);
                }
                else {
                    callback(false, response.message);
                }
            }
            else {
                callback(false, "서버와의 연결이 실패했습니다.");
            }
        }

        // 자동 로그인 시도
        public static IEnumerator TryAutoLogin(System.Action<bool, string> callback) {
            string url = ServerUrl + "autoLogin"; // 자동 로그인 API 엔드포인트
            string SessionToken = PlayerPrefs.GetString("SessionToken", "");

            if (string.IsNullOrEmpty(SessionToken)) {
                callback(false, "저장된 로그인 정보가 없습니다.");
                yield break;
            }

            WWWForm form = new WWWForm();
            form.AddField("userId", SessionToken);

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                string jsonResponse = request.downloadHandler.text;
                var response = JsonConvert.DeserializeObject<LoginResponse>(jsonResponse);

                if (response.success) {
                    callback(true, response.message);
                }
                else {
                    callback(false, response.message);
                }
            }
            else {
                callback(false, "서버와의 연결이 실패했습니다.");
            }
        }

        // UserData 저장 요청 함수
        public IEnumerator SaveUserDataRequest(string userId, UserData userData, System.Action<bool, string> callback) {
            string url = ServerUrl + "saveUserData"; // UserData 저장 API 엔드포인트
            WWWForm form = new WWWForm();
            form.AddField("userId", userId);
            form.AddField("nickname", userData.Nickname);
            form.AddField("profileNum", userData.ProfileNum);
            form.AddField("coins", userData.Coins);
            form.AddField("grade", userData.Grade);
            form.AddField("rankPoint", userData.RankPoint);
            form.AddField("winCount", userData.WinCount);
            form.AddField("loseCount", userData.LoseCount);

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                // 서버로부터 응답 받은 JSON 처리
                string jsonResponse = request.downloadHandler.text;
                var response = JsonConvert.DeserializeObject<BaseResponse>(jsonResponse);

                if (response.success) {
                    callback(true, response.message);
                }
                else {
                    callback(false, response.message);
                }
            }
            else {
                callback(false, "서버와의 연결이 실패했습니다.");
            }
        }

        
        public static void SetUserData(string userId, string nickname, int profileNum,
                                       int coins, int grade, int rankPoint, int winCount, int loseCount) {
            PlayerManager.Instance.userData = new UserData(nickname, profileNum, coins, grade, rankPoint, winCount, loseCount);
            Debug.Log($"세션 추가: {userId} - {nickname}");
            //
            // SaveUserData(); // 저장
        }
        
        // UserData 가져오기 요청 함수
        public static IEnumerator GetUserDataRequest(string userId, System.Action<bool, UserData, string> callback) {
            string url = ServerUrl + "getUserData"; // UserData 가져오기 API 엔드포인트
            WWWForm form = new WWWForm();
            form.AddField("userId", userId);

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                // 서버로부터 응답 받은 JSON 처리
                string jsonResponse = request.downloadHandler.text;
                var response = JsonConvert.DeserializeObject<GetUserDataResponse>(jsonResponse);

                if (response.success) {
                    // 서버에서 가져온 UserData를 전달
                    UserData userData = new UserData(
                        response.nickname,
                        response.profileNum,
                        response.coins,
                        response.grade,
                        response.rankPoint,
                        response.winCount,
                        response.loseCount
                    );

                    callback(true, userData, response.message);
                }
                else {
                    callback(false, null, response.message);
                }
            }
            else {
                callback(false, null, "서버와의 연결이 실패했습니다.");
            }
        }


    }

    // 로그인 응답 구조체
    public class LoginResponse {
        public bool success;
        public string message;
    }

    // 기본 응답 구조체
    public class BaseResponse {
        public bool success;
        public string message;
    }

    // UserData 응답 구조체
    public class GetUserDataResponse : BaseResponse {
        public string nickname;
        public int profileNum;
        public int coins;
        public int grade;
        public int rankPoint;
        public int winCount;
        public int loseCount;
    }

}
