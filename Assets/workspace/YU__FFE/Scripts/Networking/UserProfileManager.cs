using Commons;
using System;
using UnityEngine;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEditor.PackageManager.Requests;

namespace YU_FFEE
{
/// <summary>
/// 서버에서 프로필 이미지 URL 가져오기
/// </summary>
public class UserProfileManager : Singleton<UserProfileManager> {
    public IEnumerator GetProfileImage(string userId, Action<Sprite> callback) {
        string url = $"{Constants.ServerURL}/uer/profile?userId={userId}";
        string accessToken = workspace.YU__FFE.Scripts.Networking.SessionManager.Instance.GetAccessToken();

        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url)) {
            request.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                var response = JsonConvert.DeserializeObject<ProfileResponse>(request.downloadHandler.text);
                if (!string.IsNullOrEmpty(response.profileUrl)) {
                    StopCoroutine(DownloadProfileImage(response.profileUrl, callback));
                }
                else {
                    callback?.Invoke(null);
                }
            }
            else {
                Debug.LogError($"[UserProfileManager] 프로필 이미지 불러오기 실패: {request.error}");
                callback?.Invoke(null);
            }
        }
    }

    /// <summary>
    /// 이미지 다운로드 및 Sprite 변환
    /// </summary>
    /// <param name="imageUrl">다운 받을 이미지 주소</param>
    /// <param name="callback">Sprite로 변환</param>
    /// <returns></returns>
    private IEnumerator DownloadProfileImage(string imageUrl, Action<Sprite> callback) {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl)) {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                callback?.Invoke(sprite);
            }
            else {
                Debug.LogError($"[UserProfileManager] 이미지 다운로드 실패: {request.error}");
                callback?.Invoke(null);
            }
        }
    }

    [Serializable]
    private class ProfileResponse {
        public string profileUrl;
    }
}
}