using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using workspace.YU__FFE.Scripts.Common;
using workspace.YU__FFE.Scripts.Server.Network;

namespace workspace.YU__FFE.Scripts.Server.Session {

    public class SessionManager : Singleton<SessionManager> {

        private const string ServerUrl = Constants.ServerURL;
        public string SessionToken { get; set; }
        
        
        private void Awake() {
            LoadLocalSessionToken(); // 게임 실행 시 세션 로드
        }
        
        // ========== 로컬에서 세션 토큰 불러오기 ==========
        private void LoadLocalSessionToken() {
            if (PlayerPrefs.HasKey("SessionToken")) {
                SessionToken = PlayerPrefs.GetString("SessionToken");
                Debug.Log($"[SessionManager] 로컬 세션 토큰 로드: {SessionToken}");
            }
            else {
                Debug.Log("[SessionManager] 저장된 세션 토큰이 없습니다.");
            }
        }
        

        public static Sprite[] ProfileSprites;
        // 스프라이트 배열도 따로 선언 필요 (게임 시작 시 SignUpManager가 초기화)
        public static Image[] ProfileButtonImages;

        // 버튼 안 이미지 Sprite 반환 함수
        public static Sprite GetProfileButtonSprite(int index) {
            if (ProfileButtonImages == null || ProfileButtonImages.Length == 0) {
                Debug.LogWarning("[SessionManager] 프로필 버튼 이미지가 초기화되지 않았습니다.");
                return null;
            }

            if (index >= 0 && index < ProfileButtonImages.Length) {
                return ProfileButtonImages[index].sprite;
            }
            else {
                Debug.LogWarning($"[SessionManager] 잘못된 프로필 인덱스 요청: {index}");
                return null;
            }
        }


        // ========== 특정 유저 세션 가져오기 ==========
        // GetSession(string userId) ->GetSession()
        public string GetSession() {
            return SessionToken;
        }

        // 더 이상 지원하지 않음: 여러명의 유저 세션을 저장할 수 없음
        // ========== 모든 유저 세션 불러오기 (게임 시작 시) ==========
        // public static void LoadAllSessions() {
        //     userSessions.Clear();
        //     foreach (string userId in GetAllStoredUserIds()) {
        //         string json = PlayerPrefs.GetString(userId, "");
        //         if (!string.IsNullOrEmpty(json)) {
        //             UserSession session = JsonUtility.FromJson<UserSession>(json);
        //             userSessions[userId] = session;
        //             Debug.Log($"세션 로드: {userId} - {session.Nickname}");
        //         }
        //     }
        //     Debug.Log($"총 {userSessions.Count}명의 유저 세션 로드 완료");
        // }

        // ========== 특정 유저 세션 삭제 ===========
        public IEnumerator RemoveSession(System.Action<bool, string> callback) {
            string url = ServerUrl + "removeSession"; // 세션 삭제 API 엔드포인트
            WWWForm form = new WWWForm();
            form.AddField("sessionToken", SessionToken);

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                string jsonResponse = request.downloadHandler.text;
                var response = JsonUtility.FromJson<BaseResponse>(jsonResponse);

                if (response.success) {
                    callback(true, "세션이 성공적으로 삭제되었습니다.");
                    Debug.Log($"세션 삭제: {SessionToken}");
                }
                else {
                    callback(false, response.message);
                }
            }
            else {
                callback(false, "서버와의 연결이 실패했습니다.");
            }
        }

        // ========== 프로필 이미지 반환 함수 ==========
        public static Sprite GetUserProfileSprite(int profileNum) {
            if (ProfileSprites == null || ProfileSprites.Length == 0) {
                Debug.LogWarning("[SessionManager] 프로필 스프라이트가 설정되지 않았습니다.");
                return null;
            }

            if (profileNum >= 0 && profileNum < ProfileSprites.Length) {
                return ProfileSprites[profileNum];
            }
            else {
                Debug.LogWarning($"[SessionManager] 잘못된 프로필 번호 요청: {profileNum}");
                return null;
            }
        }

        // ========== 새로운 유저 ID 저장 ==========
        private void SaveLocalSessionToken(string userId) {
            PlayerPrefs.SetString("SessionToken", string.Join(",", SessionToken));
            PlayerPrefs.Save();

        }

        // ========== 유저 ID 삭제 ========== 
        private static void RemoveLocalUserId(string userId) {
            // 세션 토큰 삭제 (유저 ID와 관련된 세션 토큰을 삭제)
            string sessionTokenKey = "SessionToken_" + userId; // 각 유저 ID에 고유한 세션 토큰을 사용
            PlayerPrefs.DeleteKey(sessionTokenKey); // 유저 ID에 해당하는 세션 토큰 삭제
            PlayerPrefs.Save();
        }



    }


}
