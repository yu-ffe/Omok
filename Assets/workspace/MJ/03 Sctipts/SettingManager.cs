using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WB;
using workspace.YU__FFE.Scripts;

namespace MJ
{
    public class SettingManager : UI_Panel
    {
        [Header("슬라이더 연결")]
        public Slider bgmSlider;
        public Slider sfxSlider;
        
        [Header("자동 로그인 토글 연결")]
        public Toggle autoLoginToggle;
        
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
            autoLoginToggle.onValueChanged.AddListener(OnAutoLoginToggleChanged);
        }
        
        // 자동 로그인 토글 콜백
        private void OnAutoLoginToggleChanged(bool isOn)
        {
            AutoLogin.SetAutoLogin(isOn);
        }
        
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
        
        // ========= 설정 패널 상태 확인 ==========
        public bool IsSettingPanelOpen()
        {
            return UI_Manager.Instance.nowShowingPanelType == UI_Manager.PanelType.Option;
        }

        
    }

}