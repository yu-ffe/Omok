using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI;

namespace MJ
{
    public class SignUpManager : MonoBehaviour
    {
        [Header("회원가입 UI 연결")] 
        public TMP_InputField inputId;
        public TMP_InputField inputPassword;
        public TMP_InputField inputPasswordCheck;
        public TMP_InputField inputNickname;
        public Button[] profileButtons; // 버튼 배열로 받기 (이미지 포함된 버튼)
        public Image[] profileImages; // 프로필 이미지 (아이콘용) 연결 (버튼에 있는 이미지 컴포넌트)

        [Header("알림 텍스트")] public TMP_Text alertText;

        [Header("프로필 스프라이트")] 
        public Sprite[] profileSprites; // 프로필 이미지들 연결 (게임 시작 시 초기화용)
        
        private int _profileNumber = -1; // 선택된 프로필 번호 (-1: 선택 안함)

        void Start()
        {
            // 모든 저장된 유저 세션 불러오기
            SessionManager.LoadAllSessions();
            Debug.Log("모든 유저 세션 로드 완료");

            // 프로필 스프라이트 초기화
            SessionManager.ProfileSprites = profileSprites;
            Debug.Log("프로필 스프라이트 초기화 완료");
        }
        
        // ========== 회원가입 버튼 클릭 시 호출 ==========
        public void OnClickSignUp()
        {
            if (ValidateInput())
            {
                SaveUserData();
                alertText.text = "회원가입 성공! 메인 화면으로 이동합니다.";
                Debug.Log("회원가입 완료");

                // 자동 로그인 처리
                AutoLogin(inputId.text);
            }
        }

        // ========== 유효성 검사 ==========
        bool ValidateInput()
        {
            string id = inputId.text.Trim();
            string password = inputPassword.text;
            string passwordCheck = inputPasswordCheck.text;
            string nickname = inputNickname.text.Trim();

            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(passwordCheck) || string.IsNullOrEmpty(nickname))
            {
                alertText.text = "모든 항목을 입력하세요.";
                return false;
            }

            if (!IsEmailFormat(id))
            {
                alertText.text = "올바른 이메일 형식이 아닙니다.";
                return false;
            }

            if (IsIdDuplicated(id))
            {
                alertText.text = "이미 존재하는 아이디입니다.";
                return false;
            }

            if (password != passwordCheck)
            {
                alertText.text = "비밀번호가 일치하지 않습니다.";
                return false;
            }

            return true;
        }

        // ========== 이메일 형식 체크 ==========
        bool IsEmailFormat(string email)
        {
            return email.Contains("@") && email.Contains(".");
        }

        // ========== 아이디 중복 확인 ==========
        bool IsIdDuplicated(string id)
        {
            string ids = PlayerPrefs.GetString("user_ids", "");
            string[] idArray = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return idArray.Contains(id);
        }

        // ========== 비밀번호 암호화 (Base64) ==========
        string EncryptPassword(string plainPassword)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainPassword);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        // ========== 데이터 저장 ==========
        void SaveUserData()
        {
            string id = inputId.text.Trim();
            string password = EncryptPassword(inputPassword.text);
            string nickname = inputNickname.text.Trim();
            int profile = _profileNumber;

            // 기본 값
            int coins = 1000;
            int grade = 18;       // 시작 급수
            int rankPoint = 0;    // 시작 포인트
            int winCount = 0;     // 시작 승리 수
            int loseCount = 0;    // 시작 패배 수

            // 데이터 저장
            string ids = PlayerPrefs.GetString("user_ids", "");
            ids = string.IsNullOrEmpty(ids) ? id : $"{ids},{id}";
            PlayerPrefs.SetString("user_ids", ids);
            PlayerPrefs.SetString($"user_{id}_password", password);
            PlayerPrefs.SetString($"user_{id}_nickname", nickname);
            PlayerPrefs.SetInt($"user_{id}_profile", profile);
            PlayerPrefs.SetInt($"user_{id}_coins", coins);
            PlayerPrefs.SetInt($"user_{id}_grade", grade);
            PlayerPrefs.SetInt($"user_{id}_rankPoint", rankPoint);
            PlayerPrefs.SetInt($"user_{id}_winCount", winCount);
            PlayerPrefs.SetInt($"user_{id}_loseCount", loseCount);
            PlayerPrefs.Save();

            // 세션 등록
            SessionManager.AddSession(id, nickname, profile, coins, grade, 
                rankPoint, winCount, loseCount);
        }

        // ========== 프로필 이미지 선택 ==========
        public void OnClickSelectProfile(int index)
        {
            _profileNumber = index;
            Debug.Log($"선택한 프로필 번호: {_profileNumber}");

            // 모든 버튼 초기화 (테두리 색 원상복구)
            for (int i = 0; i < profileButtons.Length; i++)
            {
                profileImages[i].color = Color.white; // 기본색 (선택 안됨)
            }

            // 선택한 버튼만 강조 (예: 초록색 테두리로 표시)
            profileImages[index].color = Color.green;
        }

        // ========== 자동 로그인 ==========
        void AutoLogin(string id)
        {
            SessionManager.currentUserId = id;
            Debug.Log($"자동 로그인 완료: {id}");
        }
    }
    // ========== 세션 매니저  ==========
    
    

}
