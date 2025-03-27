using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Commons;

public class LoginPanel : UI_Panel
{

    [Header("타이틀/메세지")]
    public TextMeshProUGUI txtTitle;
    public TextMeshProUGUI txtMessage;

    [Header("텍스트입력")]
    public TMP_InputField inputEmail;
    public TMP_InputField inputPassword;
    public TMP_InputField inputCheckPassword;
    public TMP_InputField inputNickName;

    [Header("프로필 이미지 선택")]
    public GameObject objProfileImages;

    [Header("하단 버튼 2개")]
    public Button btnUpper;
    public Button btnLower;
    public TextMeshProUGUI txtButtonUpper;
    public TextMeshProUGUI txtButtonLower;
    
    
    // TODO: 아직 사용하지 않으므로 추가적인 기능 X
    // bool IsSignedIn => !string.IsNullOrEmpty(SessionManager.currentUserId);



    [Header("프로필 이미지 관련 > SessionManager에 전달 됨")]
    // public Button[] profileButtons; // 버튼 배열로 받기 (이미지 포함된 버튼)
    public Image[] profileImages; // 프로필 이미지 (아이콘용) 연결 (버튼에 있는 이미지 컴포넌트)
    public Sprite[] profileSprites;
    int selectedImageIndex = -1;
    private string uploadedImageUrl = "";
    
    //  `Constants.ServerURL`을 사용하여 API URL 설정
    private string baseUrl => Constants.ServerURL;
    private string uploadProfileUrl => $"{Constants.ServerURL}/api/upload_profile";
    private string signUpUrl => $"{Constants.ServerURL}/auth/signup";
    private string getProfileUrl(string userId) => $"{baseUrl}/api/get_profile?userId={userId}";

    void Start() {
        UI_Manager.Instance.AddPanel(UI_Manager.PanelType.Login, this);
        
        gameObject.SetActive(true);
        Show_LoginPhase();

        // 로그인 상태 초기화
        Show_LoginPhase();
        
        //Login 여부 확인

        // 프로필 스프라이트 초기화
        // TODO: 스프라이트 저장 클래스는 다른곳으로.
        // SessionManager.ProfileSprites = profileSprites;

        // 버튼 안 이미지 초기화 (다른 스크립트에서 접근 가능)
        // TODO: 이미지도 동일
        // SessionManager.ProfileButtonImages = profileImages;

    }

    public override void Show() {
        //Login
        
        Debug.Log("[LoginPanel] Show() 실행됨");
        Show_LoginPhase();
        gameObject.SetActive(true);
    }
    
    public override void Hide() {
        gameObject.SetActive(false);
    }

    public override void OnEnable() {
    }

    public override void OnDisable() {
    }

    [ContextMenu("Show_LoginPhase")]
    void Show_LoginPhase() {
        btnUpper.onClick.RemoveAllListeners();
        btnUpper.onClick.AddListener(OnClick_Login);
        btnLower.onClick.RemoveAllListeners();
        btnLower.onClick.AddListener(OnClick_Signup_Show);

        inputCheckPassword.gameObject.SetActive(false);
        inputNickName.gameObject.SetActive(false);

        txtTitle.text = "Login";
        
        txtMessage.text = string.Empty;

        inputEmail.text = string.Empty;
        inputPassword.text = string.Empty;
        inputCheckPassword.text = string.Empty;
        inputNickName.text = string.Empty;

        txtButtonUpper.text = "Login";
        txtButtonLower.text = "SignUp";

        objProfileImages.SetActive(false);
    }

    [ContextMenu("Show_SignUpPhase")]
    void Show_SignUpPhase() {
        btnUpper.onClick.RemoveAllListeners();
        btnUpper.onClick.AddListener(OnClick_SignUp_TrySignUp);
        btnLower.onClick.RemoveAllListeners();
        btnLower.onClick.AddListener(OnClick_SignUp_Cancel);
        inputCheckPassword.gameObject.SetActive(true);
        inputNickName.gameObject.SetActive(true);

        txtTitle.text = "SignUp";
        txtMessage.text = string.Empty;

        inputEmail.text = string.Empty;
        inputPassword.text = string.Empty;
        inputCheckPassword.text = string.Empty;
        inputNickName.text = string.Empty;

        txtButtonUpper.text = "SignUp";
        txtButtonLower.text = "Cancel";

        objProfileImages.SetActive(true);
    }
    
    // IEnumerator UploadProfileImage(Texture2D texture, string userId)
    // {
    //     Texture2D readableTexture = GetReadableTexture(texture);
    //     if (readableTexture == null)
    //     {
    //         Debug.LogError("[UploadProfileImage] Readable Texture 변환 실패!");
    //         yield break;
    //     }
    //
    //     byte[] imageBytes = readableTexture.EncodeToPNG();
    //     WWWForm form = new WWWForm();
    //     form.AddBinaryData("file", imageBytes, "profile.png", "image/png");
    //     form.AddField("userId", userId);
    //
    //     using (UnityWebRequest www = UnityWebRequest.Post(uploadProfileUrl, form))
    //     {
    //         yield return www.SendWebRequest();
    //
    //         //  서버 응답 로그 추가
    //         string responseText = www.downloadHandler.text;
    //         Debug.Log($"[UploadProfileImage] 서버 응답: {responseText}");
    //
    //         if (www.result == UnityWebRequest.Result.Success)
    //         {
    //             // JSON인지 확인 후 파싱
    //             if (responseText.StartsWith("{") && responseText.EndsWith("}"))
    //             {
    //                 try
    //                 {
    //                     UploadResponse response = JsonUtility.FromJson<UploadResponse>(responseText);
    //                     uploadedImageUrl = response.imageUrl; //  JSON에서 URL 추출
    //                     Debug.Log($"[UploadProfileImage] 업로드된 이미지 URL: {uploadedImageUrl}");
    //                 }
    //                 catch (System.Exception ex)
    //                 {
    //                     Debug.LogError($"[UploadProfileImage] JSON 파싱 오류: {ex.Message}");
    //                 }
    //             }
    //             else
    //             {
    //                 Debug.LogError("[UploadProfileImage] 서버 응답이 JSON 형식이 아닙니다: " + responseText);
    //             }
    //         }
    //         else
    //         {
    //             Debug.LogError("[UploadProfileImage] 프로필 이미지 업로드 실패: " + www.error);
    //         }
    //     }
    // }

    //  서버 응답을 처리할 클래스 추가 (JSON 파싱용)
    [System.Serializable]
    public class UploadResponse
    {
        public bool success;
        public string imageUrl;
    }
    
    
    public void OnClick_PrifileImage(int index)
    {
        if (profileSprites == null || profileSprites.Length == 0)
        {
            Debug.LogError("[LoginPanel] profileSprites 배열이 비어있습니다.");
            return;
        }

        if (index < 0 || index >= profileSprites.Length)
        {
            Debug.LogError("[LoginPanel] 잘못된 index 값: " + index);
            return;
        }

        selectedImageIndex = index;
        for (int i = 0; i < profileImages.Length; i++)
        {
            profileImages[i].color = i == index ? Color.white : Color.gray;
        }

        // StartCoroutine(UploadProfileImage(profileSprites[index].texture, inputEmail.text.Trim()));
    }

    
    private Texture2D GetReadableTexture(Texture2D sourceTex)
    {
        if (sourceTex == null)
        {
            Debug.LogError("[LoginPanel] Texture가 null입니다.");
            return null;
        }

        RenderTexture renderTex = RenderTexture.GetTemporary(
            sourceTex.width, sourceTex.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);

        Graphics.Blit(sourceTex, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;

        Texture2D readableTexture = new Texture2D(sourceTex.width, sourceTex.height);
        readableTexture.ReadPixels(new Rect(0, 0, sourceTex.width, sourceTex.height), 0, 0);
        readableTexture.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);

        return readableTexture;
    }



    
    public void OnClick_Login()
    {
        txtMessage.text = "로그인 중...";

        SignInHandler.Instance.AttemptSignIn(inputEmail.text.Trim(), inputPassword.text, (isSuccess, message) =>
        {
            txtMessage.text = message;

            if (isSuccess)
            {
                Debug.Log("[LoginPanel] 로그인 성공, 메인 화면으로 이동");
                UI_Manager.Instance.Show(UI_Manager.PanelType.Main);
            }
        });
    }

    public void OnClick_Signup_Show() {
        Show_SignUpPhase();
    }

    public void OnClick_SignUp_TrySignUp()
    {
        if (selectedImageIndex == -1)
        {
            txtMessage.text = "프로필 이미지를 선택해주세요.";
            return;
        }

        txtMessage.text = "회원가입 중...";
        SignUpHandler.Instance.AttemptSignUp(inputEmail.text.Trim(), inputPassword.text, inputCheckPassword.text, inputNickName.text.Trim(), selectedImageIndex, (isSuccess, message) =>
        {
            txtMessage.text = message;

            if (isSuccess)
            {
                Debug.Log("[LoginPanel] 회원가입 성공, 로그인 화면으로 이동");
                OnClick_Login();
            }
        });
    }

    void ShowSignInPanel()
    {
        UI_Manager.Instance.Show(UI_Manager.PanelType.Login);
    }
    
    IEnumerator GetProfileImage(string userId, Image profileImage)
    {
        string url = $"https://yourserver.com/get_profile?userId={userId}";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string json = www.downloadHandler.text;
                ProfileData data = JsonUtility.FromJson<ProfileData>(json);
                StartCoroutine(LoadImageFromUrl(data.profileUrl, profileImage));
            }
            else
            {
                Debug.LogError("프로필 이미지 불러오기 실패: " + www.error);
            }
        }
    }

    IEnumerator LoadImageFromUrl(string url, Image profileImage)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                profileImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
            else
            {
                Debug.LogError("프로필 이미지 다운로드 실패: " + www.error);
            }
        }
    }

    [System.Serializable]
    public class ProfileData
    {
        public string userId;
        public string profileUrl;
    }

    public void OnClick_SignUp_Cancel() {
        Show_LoginPhase();
    }


}
