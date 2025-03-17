using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using workspace.YU__FFE.Scripts.Common;

namespace workspace.YU__FFE.Scripts.Junks {
    public class NetworkManagerJunk : Singleton<NetworkManagerJunk>
    {
        
        // 📌 회원가입 처리
        
        public struct SignupData
        {
            public string username;
            public string nickname;
            public string password;
        }
        public IEnumerator Signup(SignupData signupData, Action success, Action failure)
        {
            string jsonString = JsonUtility.ToJson(signupData);
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonString);

            using (UnityWebRequest www = new UnityWebRequest(Constants.ServerURL + "/auth/signup", UnityWebRequest.kHttpVerbPOST))
            {
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                www.downloadHandler = new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");

                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log("Error: " + www.error);

                    if (www.responseCode == 409)
                    {
                        // // 중복 사용자 처리
                        // GameManager.Instance.OpenConfirmPanel("이미 존재하는 사용자입니다.", () =>
                        // {
                        //     failure?.Invoke();
                        // });
                    }
                    else
                    {
                        failure?.Invoke();
                    }
                }
                else
                {
                    var result = www.downloadHandler.text;
                    Debug.Log("회원가입 성공: " + result);

                    // 회원가입 성공 팝업
                    // GameManager.Instance.OpenConfirmPanel("회원 가입이 완료되었습니다.", () =>
                    // {
                    //     success?.Invoke();
                    // });
                }
            }
        }

        // 📌 로그인 처리
        public struct SigninData
        {
            public string username;
            public string password;
        }
        public struct SigninResult
        {
            public int result;
        }
        public IEnumerator Signin(SigninData signinData, Action success, Action<int> failure)
        {
            string jsonString = JsonUtility.ToJson(signinData);
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonString);

            using (UnityWebRequest www = new UnityWebRequest(Constants.ServerURL + "/users/signin", UnityWebRequest.kHttpVerbPOST))
            {
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                www.downloadHandler = new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");

                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError) {
                    failure?.Invoke(-1); // 서버 오류
                    yield break;
                }

                var cookie = www.GetResponseHeader("set-cookie");
                if (!string.IsNullOrEmpty(cookie))
                {
                    int lastIndex = cookie.LastIndexOf(";");
                    string sid = cookie.Substring(0, lastIndex);
                    PlayerPrefs.SetString("sid", sid); // 로그인 세션 저장
                }

                var resultString = www.downloadHandler.text;
                var result = JsonUtility.FromJson<SigninResult>(resultString);

                if (result.result == 0)
                {
                    // 유저네임 유효하지 않음
                    // GameManager.Instance.OpenConfirmPanel("유저네임이 유효하지 않습니다.", () =>
                    // {
                    //     failure?.Invoke(0);
                    // });
                }
                else if (result.result == 1)
                {
                    // 패스워드 유효하지 않음
                    // GameManager.Instance.OpenConfirmPanel("패스워드가 유효하지 않습니다.", () =>
                    // {
                    //     failure?.Invoke(1);
                    // });
                }
                else if (result.result == 2)
                {
                    // 로그인 성공
                    // GameManager.Instance.OpenConfirmPanel("로그인에 성공하였습니다.", () =>
                    // {
                    //     success?.Invoke();
                    // });
                }
            }
        }
        
        public IEnumerator Signout(Action success, Action failure)
        {
            string sid = PlayerPrefs.GetString("sid", "");
    
            if (string.IsNullOrEmpty(sid))
            {
                failure?.Invoke(); // 세션이 없으면 바로 실패 처리
                yield break; // 즉시 종료
            }

            using (UnityWebRequest www = new UnityWebRequest(Constants.ServerURL + "/auth/signout", UnityWebRequest.kHttpVerbPOST))
            {
                www.SetRequestHeader("Cookie", sid);
                www.downloadHandler = new DownloadHandlerBuffer();

                // 로그아웃 요청
                yield return StartCoroutine(SendSignoutRequest(www, success, failure));
            }
        }

        private IEnumerator SendSignoutRequest(UnityWebRequest www, Action success, Action failure)
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("로그아웃 실패: " + www.error);
                failure?.Invoke();
            }
            else
            {
                // 세션 종료
                PlayerPrefs.DeleteKey("sid");

                // 로그아웃 성공
                Debug.Log("로그아웃 성공");
                success?.Invoke();
            }
        }

        
        //
        // // 📌 점수 조회
        // public IEnumerator GetScore(Action<ScoreResult> success, Action failure)
        // {
        //     using (UnityWebRequest www = new UnityWebRequest(Constants.ServerURL + "/users/score", UnityWebRequest.kHttpVerbGET))
        //     {
        //         www.downloadHandler = new DownloadHandlerBuffer();
        //
        //         string sid = PlayerPrefs.GetString("sid", "");
        //         if (!string.IsNullOrEmpty(sid))
        //         {
        //             www.SetRequestHeader("Cookie", sid);
        //         }
        //
        //         yield return www.SendWebRequest();
        //
        //         if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        //         {
        //             if (www.responseCode == 403)
        //             {
        //                 Debug.Log("로그인이 필요합니다.");
        //             }
        //
        //             failure?.Invoke();
        //         }
        //         else
        //         {
        //             var result = www.downloadHandler.text;
        //             var userScore = JsonUtility.FromJson<ScoreResult>(result);
        //             success?.Invoke(userScore);
        //         }
        //     }
        // }

        // // 📌 리더보드 조회
        // public IEnumerator GetLeaderboard(Action<Scores> success, Action failure)
        // {
        //     using (UnityWebRequest www = new UnityWebRequest(Constants.ServerURL + "/leaderboard", UnityWebRequest.kHttpVerbGET))
        //     {
        //         www.downloadHandler = new DownloadHandlerBuffer();
        //
        //         string sid = PlayerPrefs.GetString("sid", "");
        //         if (!string.IsNullOrEmpty(sid))
        //         {
        //             www.SetRequestHeader("Cookie", sid);
        //         }
        //
        //         yield return www.SendWebRequest();
        //
        //         if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        //         {
        //             if (www.responseCode == 403)
        //             {
        //                 Debug.Log("로그인이 필요합니다.");
        //             }
        //
        //             failure?.Invoke();
        //         }
        //         else
        //         {
        //             var result = www.downloadHandler.text;
        //             var scores = JsonUtility.FromJson<Scores>(result);
        //             success?.Invoke(scores);
        //         }
        //     }
        // }
    }
}
