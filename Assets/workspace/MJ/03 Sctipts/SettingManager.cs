using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WB;
using workspace.YU__FFE.Scripts;
using WB;

namespace MJ
{
    public class SettingManager : UI_Panel
    {
        [Header("슬라이더 연결")]
        public Slider bgmSlider;
        public Slider sfxSlider;
        
        [Header("자동 로그인 토글 연결")]
        public Toggle autoLoginToggle;
        
<<<<<<< Updated upstream
        // ========= 사운드 초기화 (게임 시작 시) ========== 
        void Start()
        {
            UI_Manager.Instance.AddPanel(UI_Manager.PanelType.Option, this);
            gameObject.SetActive(false);
            Debug.Log("SettingPanel 등록 완료");
            
            LoadSoundSettings();
            InitAutoLoginToggle();
        }
        
        public override void Show()
        {
            gameObject.SetActive(true);
        }

        public override void Hide()
        {
            gameObject.SetActive(false);
        }

        public override void OnEnable() { }

        public override void OnDisable() { }
        
        // ========= 사운드 설정 로드 ========== 
        private void LoadSoundSettings()
        {
            float bgmVolume = SoundManager.Instance.GetSavedBgmVolume();
            float sfxVolume = SoundManager.Instance.GetSavedSfxVolume();
            
            bgmSlider.value = bgmVolume;
            sfxSlider.value = sfxVolume;
            
            // 슬라이더 값 바뀌면 SoundManager에 위임
            bgmSlider.onValueChanged.AddListener(SoundManager.Instance.SetBgmVolume);
            sfxSlider.onValueChanged.AddListener(SoundManager.Instance.SetSfxVolume);
        }
        
        // 자동 로그인 토글 초기화
        private void InitAutoLoginToggle()
        {
            // 자동 로그인 토클 초기화 (저장된 상태 불러오기)
            bool isAutoLoginEnabled = AutoLogin.GetAutoLogin();
            autoLoginToggle.isOn = isAutoLoginEnabled;
            
            // 토글 변경 시 자동 로그인 설정 저장
=======
        private bool isInitialized = false;
        
        // ========= 사운드 초기화 (게임 시작 시) ========== 
        void Start()
        {
            //  사운드 슬라이더 초기화 (값 설정만, 저장 X)
            LoadSoundSettings();

            //  슬라이더 조작 시 저장 (유저 조작만 저장)
            bgmSlider.onValueChanged.AddListener(OnBgmSliderChanged);
            sfxSlider.onValueChanged.AddListener(OnSfxSliderChanged);

            //  자동 로그인 초기화
            InitAutoLogin();
>>>>>>> Stashed changes
            autoLoginToggle.onValueChanged.AddListener(OnAutoLoginToggleChanged);

            //  UI 패널 등록
            UI_Manager.Instance.AddPanel(panelType, this);
            gameObject.SetActive(false); // 처음엔 안 보이게

            //  초기화 완료 표시 (이후부터 유저 조작 간주)
            isInitialized = true;
        }
        
        /// <summary>
        /// 자동 로그인 값 초기화 (처음엔 무조건 ON)
        /// </summary>
        private void InitAutoLogin()
        {
            autoLoginToggle.isOn = AutoLogin.GetAutoLogin(); // 저장된 값으로 초기화
        }

        private void OnAutoLoginToggleChanged(bool isOn)
        {
            AutoLogin.SetAutoLogin(isOn);
            Debug.Log("[SettingManager] 자동 로그인 설정: " + isOn);
        }
        
<<<<<<< Updated upstream
=======
        // ========= 사운드 설정 로드 ========== 
        private void LoadSoundSettings()
        {
            float bgmVolume = PlayerPrefs.GetFloat("BGM_VOLUME", 1f);
            float sfxVolume = PlayerPrefs.GetFloat("SFX_VOLUME", 1f);
            
            // 콜백 없이 값 설정
            bgmSlider.value = bgmVolume;
            sfxSlider.value = sfxVolume;
        }
        
        /// <summary>
        /// BGM 슬라이더 조작 시
        /// </summary>
        private void OnBgmSliderChanged(float value)
        {
            if (!isInitialized) return; // 초기화 중엔 무시
            
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.SetBgmVolume(value); // 내부에서 저장
            }
        }

        /// <summary>
        /// SFX 슬라이더 조작 시
        /// </summary>
        /// <summary>
        /// SFX 슬라이더 사용자 조작 시 저장
        /// </summary>
        private void OnSfxSliderChanged(float value)
        {
            if (!isInitialized) return; // 초기화 중엔 무시

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.SetSfxVolume(value); // 내부에서 저장
            }
        }
        
        // ========= 배경음 볼륨 설정 ==========
        public void SetBGMVolume(float volume)
        {
            SoundManager.Instance.SetBgmVolume(volume);
        }
        
        // ========= 효과음 볼륨 설정 ==========
        public void SetSFXVolume(float volume)
        {
            SoundManager.Instance.SetSfxVolume(volume);
        }
        
        public override void Show() => gameObject.SetActive(true);
        public override void Hide() => gameObject.SetActive(false);
        public override void OnEnable(){}
        public override void OnDisable(){}
        
>>>>>>> Stashed changes
        // ========= 설정 패널 열기 ==========
        public void OpenSettingPanel()
        {
            UI_Manager.Instance.Show(UI_Manager.PanelType.Option);
        }
        
        // ========= 설정 패널 닫기 ==========
        public void CloseSettingPanel()
        {
            UI_Manager.Instance.Hide(UI_Manager.PanelType.Option);
        }
        
        // ========= 확인 버튼  ==========
        public void OnClickConfirmButton()
        {
            CloseSettingPanel();
        }
        
        // ========= 닫기 버튼 ==========
        public void OnClickCloseButton()
        {
            CloseSettingPanel();
        }
<<<<<<< Updated upstream
        
        // ========= 설정 패널 상태 확인 ==========
        public bool IsSettingPanelOpen()
        {
            return UI_Manager.Instance.nowShowingPanelType == UI_Manager.PanelType.Option;
        }
=======
>>>>>>> Stashed changes

        
    }

}