using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WB;


namespace WB
{
    public class LoginPanel : UI_Panel
    {
        public enum LoginResult : byte
        {
            // 성공
            Success_SignUp,
            Success_Login,

            // 실패 이유들..?구체적으로 필요할까요?
            Failed_Email,
            Failed_Password,
            Failed_CheckPassword,
            Failed_NickName,
        }
        public GameObject objSignup;

        public TextMeshProUGUI txtTitle;
        public TextMeshProUGUI txtMessage;


        public TMP_InputField inputEmail;
        public TMP_InputField inputPassword;
        public TMP_InputField inputCheckPassword;
        public TMP_InputField inputNickName;


        public Button btnUpper;
        public Button btnLower;
        public TextMeshProUGUI txtButtonUpper;
        public TextMeshProUGUI txtButtonLower;

        bool IsSignedIn => !string.IsNullOrEmpty(SessionManager.currentUserId);


        void Start()
        {
            UI_Manager.Instance.AddPanel(panelType, this);

            //Login 여부 확인

            gameObject.SetActive(false);
        }

        public override void Show()
        {
            //Login
            Show_LoginPhase();
            gameObject.SetActive(true);
        }


        public override void Hide()
        {
            gameObject.SetActive(false);
        }

        public override void OnEnable()
        {
        }

        public override void OnDisable()
        {
        }

        [ContextMenu("Show_LoginPhase")]
        void Show_LoginPhase()
        {
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
        }

        [ContextMenu("Show_SignUpPhase")]
        void Show_SignUpPhase()
        {
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
        }



        public void OnClick_Login()
        {
            //로그인 시도
            var result = TryLogin(inputEmail.text, inputPassword.text);

            if (result == LoginResult.Success_Login)
            {
                // UI_Manager.Instance.Show("Main");//... 아직 
            }
            else
                txtMessage.text = $"Failed : {result}";
        }

        public void OnClick_Signup_Show()
        {
            Show_SignUpPhase();
        }

        public void OnClick_SignUp_TrySignUp()
        {
            // 회원 가입 시도
            var result = TrySignUp(inputEmail.text, inputPassword.text, inputCheckPassword.text, inputNickName.text);
            if (result == LoginResult.Success_SignUp)
            {
                Show_LoginPhase();
                txtMessage.text = "SignUp Sucess";
            }
            else
                txtMessage.text = $"Failed : {result}";
        }

        public void OnClick_SignUp_Cancel()
        {
            Show_LoginPhase();
        }


        LoginResult TryLogin(string eMail, string password)
        {
            //로그인 시도


            return LoginResult.Success_Login;

        }

        LoginResult TrySignUp(string eMail, string password, string checkPassword, string nickName)
        {
            //회원가입 시도
            return LoginResult.Success_SignUp;
        }

    }

}
