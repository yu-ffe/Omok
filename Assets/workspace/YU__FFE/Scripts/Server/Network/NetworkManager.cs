using Newtonsoft.Json;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using workspace.YU__FFE.Scripts.Common;
using workspace.YU__FFE.Scripts;
using workspace.YU__FFE.Scripts.User;

namespace workspace.YU__FFE.Scripts.Server.Network {
    public class NetworkManager : Singleton<NetworkManager> {
        private const string ServerUrl = Constants.ServerURL;
        
        // 회원가입 요청 함수
        public IEnumerator SignUpRequest(System.Action<SignUpResponse> callback) {
            Debug.Log("회원가입 요청: NetworkManager");
            string url = $"{ServerUrl}/auth/signup"; // 회원가입 API 엔드포인트
            PlayerData playerData = PlayerManager.Instance.playerData;
            WWWForm form = new WWWForm();
            form.AddField("id", playerData.id);
            form.AddField("password", playerData.password); // 비밀번호만 먼저 전송
            form.AddField("nickname", playerData.nickname); // 닉네임은 나중에 추가할 수 있음 (회원가입 성공 후 처리)
            form.AddField("profileNum", playerData.profileNum);

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                // 서버로부터 응답 받은 JSON 처리
                string jsonResponse = request.downloadHandler.text;
                var response = JsonConvert.DeserializeObject<SignUpResponse>(jsonResponse);

                if (response.success) {
                    callback(response);
                }
                else {
                    callback(response);
                }
            }
            else {
                callback(new SignUpResponse(false, "서버와의 연결에 실패하였습니다.", null, null));
            }
        }
        
        // 아이디 또는 닉네임 중복 체크 요청 함수
        public IEnumerator CheckDuplicateRequest(string type, string value, System.Action<bool, string> callback) {
            Debug.Log($"{type} 중복 체크 요청: NetworkManager");
            string url = $"{ServerUrl}/auth/signup/check"; // 중복 체크 API 엔드포인트
            WWWForm form = new WWWForm();
            form.AddField("type", type);    // 'nickname' 또는 'id'를 type으로 전달
            form.AddField("value", value);  // 중복 체크할 아이디 또는 닉네임

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                // 서버로부터 응답 받은 JSON 처리
                string jsonResponse = request.downloadHandler.text;
                var response = JsonConvert.DeserializeObject<CheckResponse>(jsonResponse);

                if (response.success) {
                    callback(true, response.message); // 사용 가능한 값
                }
                else {
                    callback(false, response.message); // 이미 존재하는 값
                }
            }
            else {
                callback(false, "서버와의 연결이 실패했습니다.");
            }
        }

// 아이디 중복 체크 요청 함수 (리팩토링)
        public IEnumerator CheckIdRequest(string id, System.Action<bool, string> callback) {
            yield return CheckDuplicateRequest("id", id, callback); // 아이디 중복 체크
        }

// 닉네임 중복 체크 요청 함수 (리팩토링)
        public IEnumerator CheckNicknameRequest(string nickname, System.Action<bool, string> callback) {
            yield return CheckDuplicateRequest("nickname", nickname, callback); // 닉네임 중복 체크
        }


        // 로그인 요청 함수
        public IEnumerator SignInRequest(System.Action<LoginInfo, UserData> callback) {
            string url = $"{ServerUrl}/login"; // 로그인 API 엔드포인트
            PlayerData playerData = PlayerManager.Instance.playerData;
            WWWForm form = new WWWForm();
            form.AddField("id", playerData.id);
            form.AddField("password", playerData.password);

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                // 서버로부터 응답 받은 JSON 처리
                string jsonResponse = request.downloadHandler.text;
                var response = JsonConvert.DeserializeObject<SignInResponse>(jsonResponse);

                // 로그인 성공 시 응답 처리
                if (response.login.success) {

                    // 콜백 호출 (로그인 성공)
                    callback(response.login, response.data);
                }
                else {
                    // 로그인 실패 시 콜백 호출
                    callback(response.login, null);
                }
            }
            else {
                // 서버와 연결 실패 시 콜백 호출
                callback(null, null);
            }
        }

       
        // 자동 로그인 시도
        public static IEnumerator TryAutoLogin(System.Action<LoginInfo, UserData> callback) {
            string url = $"{ServerUrl}/autoLogin"; // 자동 로그인 API 엔드포인트
            string sessionToken = PlayerPrefs.GetString("SessionToken", "");

            if (string.IsNullOrEmpty(sessionToken)) {
                callback(null, null);
                yield break;
            }

            WWWForm form = new WWWForm();
            form.AddField("userId", sessionToken);

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                string jsonResponse = request.downloadHandler.text;
                var response = JsonConvert.DeserializeObject<SignInResponse>(jsonResponse);

                if (response.login.success) {
                    // 자동 로그인 성공 시 콜백 호출
                    callback(response.login, response.data);
                }
                else {
                    // 자동 로그인 실패 시 콜백 호출
                    callback(null, null);
                }
            }
            else {
                callback(null, null);
            }
        }

        // UserData 저장 요청 함수
        public IEnumerator SaveUserDataRequest(System.Action<bool, string> callback) {
            string url = $"{ServerUrl}saveUserData"; // UserData 저장 API 엔드포인트
            string token = Session.SessionManager.Instance.SessionToken;

            // 세션 검증을 위한 헤더 추가
            if (string.IsNullOrEmpty(token)) {
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

        // UserData 가져오기 요청 함수
        public static IEnumerator GetUserDataRequest(string userId, System.Action<bool, PlayerData, string> callback) {
            string url = $"{ServerUrl}getUserData"; // UserData 가져오기 API 엔드포인트
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
                else {
                    callback(false, null, response.message);
                }
            }
            else {
                callback(false, null, "서버와의 연결이 실패했습니다.");
            }
        }
    }

    public class SignInResponse {
        public LoginInfo login;
        public UserData data;

        public SignInResponse(LoginInfo login, UserData data) {
            this.login = login;
            this.data = data;
        }
    }

    public class LoginInfo {
        public bool success;
        public string message;
        public string accessToken;
        public string refreshToken;

        public LoginInfo(bool success, string message, string accessToken, string refreshToken) {
            this.success = success;
            this.message = message;
            this.accessToken = accessToken;
            this.refreshToken = refreshToken;
        }
    }

    public class UserData {
        public string nickname;
        public int profileNum;
        public int coins;
        public int grade;
        public int rankPoint;
        public int winCount;
        public int loseCount;
        public string sessionToken;
        public string refreshToken;

        public UserData(string nickname, int profileNum, int coins, int grade, int rankPoint, int winCount, 
                        int loseCount, string sessionToken, string refreshToken) {
            this.nickname = nickname;
            this.profileNum = profileNum;
            this.coins = coins;
            this.grade = grade;
            this.rankPoint = rankPoint;
            this.winCount = winCount;
            this.loseCount = loseCount;
            this.sessionToken = sessionToken;
            this.refreshToken = refreshToken;
        }
    }

    
    public class BaseResponse {
        public bool success;
        public string message;
    }
    
    public class CheckResponse {
        public bool success;  // 요청 성공 여부
        public string message; // 메시지 (사용 가능한 아이디/닉네임인지 여부)
    }


    public class SignUpResponse {
        public bool success;
        public string message;
        public string accessToken;
        public string refreshToken;
        public SignUpResponse(bool success, string message, string accessToken, string refreshToken) {
            this.success = success;
            this.message = message;
            this.accessToken = accessToken;
            this.refreshToken = refreshToken;
        }
    }

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
