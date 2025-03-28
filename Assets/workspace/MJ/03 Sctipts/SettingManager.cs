using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Wb;
using WB;

namespace MJ
{
    public class SettingManager : MonoBehaviour
    {
        [Header("슬라이더 연결")]
        public Slider bgmSlider;
        public Slider sfxSlider;
        
        [Header("자동 로그인 토글 연결")]
        public Toggle autoLoginToggle;
        
        [Header("세팅 패널 프리팹 ")]
        public GameObject settingPanelPrefab;
        
        private GameObject _currentSettingPanel;
        
        private const string SETTING_VOLUME_KEY = "SettingPanel";
        
        // ========= 사운드 초기화 (게임 시작 시) ========== 
        void Start()
        {
            LoadSoundSettings();
            
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
        
        // ========= 사운드 설정 로드 ========== 
        private void LoadSoundSettings()
        {
            float bgmVolume = PlayerPrefs.GetFloat("BGM_VOLUME", 1f);
            float sfxVolume = PlayerPrefs.GetFloat("SFX_VOLUME", 1f);
            
            bgmSlider.value = bgmVolume;
            sfxSlider.value = sfxVolume;
            
            // 슬라이더 이벤트 연결
            bgmSlider.onValueChanged.AddListener(SetBGMVolume);
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
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
        
        // ========= 설정 패널 열기 ==========
        public void OpenSettingPanel(Transform parent)
        {
            _currentSettingPanel = Instantiate(settingPanelPrefab, parent);
        }
        
        // ========= 설정 패널 닫기 ==========
        public void CloseSettingPanel()
        {
            // UI_Manager.Instance.HideUI(SETTING_VOLUME_KEY);
            _currentSettingPanel = null;
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
            return _currentSettingPanel != null;
        }

    }

}