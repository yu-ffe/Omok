using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : UI_Panel {

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

    void Start() {
        UI_Manager.Instance.AddPanel(panelType, this);

        //Login 여부 확인

        // 프로필 스프라이트 초기화
        // TODO: 스프라이트 저장 클래스는 다른곳으로.
        // SessionManager.ProfileSprites = profileSprites;
        Debug.Log("프로필 스프라이트 초기화 완료");

        // 버튼 안 이미지 초기화 (다른 스크립트에서 접근 가능)
        // TODO: 이미지도 동일
        // SessionManager.ProfileButtonImages = profileImages;
        Debug.Log("프로필 버튼 이미지 초기화 완료");


        gameObject.SetActive(false);
    }

    public override void Show() {
        //Login
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


    public void OnClick_PrifileImage(int index) {
        selectedImageIndex = index;
        for (int i = 0; i < profileImages.Length; i++)
            profileImages[i].color = i == index ? Color.white : Color.gray;

    }


    public void OnClick_Login() {
        //로그인 시도
        // 콜백이 더 어울릴듯
        var (isSuccsess, message) = SignInHandler.Instance.TrySignIn(inputEmail.text.Trim(), inputPassword.text);

        txtMessage.text = message;

        if (isSuccsess)
            UI_Manager.Instance.Show(UI_Manager.PanelType.Main);
    }

    public void OnClick_Signup_Show() {
        Show_SignUpPhase();
    }

    public void OnClick_SignUp_TrySignUp() {
        if (selectedImageIndex == -1) {
            txtMessage.text = "프로필 이미지를 선택해주세요";
            return;
        }
        // 회원 가입 시도
        var (isSuccsess, message) = SignUpHandler.Instance.TrySignUp(
            id: inputEmail.text.Trim(),
            password: inputPassword.text,
            passwordCheck: inputCheckPassword.text,
            nickname: inputNickName.text.Trim(),
            imgIndex: selectedImageIndex
        );

        txtMessage.text = message;
        if (isSuccsess) {
            // Show_LoginPhase();
            UI_Manager.Instance.Show(UI_Manager.PanelType.Main);
        }
    }

    public void OnClick_SignUp_Cancel() {
        Show_LoginPhase();
    }


}
