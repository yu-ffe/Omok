using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Wb;

namespace KimHyeun {
    public class SettingManager : MonoBehaviour
    {
        [Header("슬라이더 연결")]
        public Slider bgmSlider;
        public Slider sfxSlider;

        [Header("세팅 패널 프리팹 ")]
        public GameObject settingPanelPrefab;

        private GameObject _currentSettingPanel;

        private const string SETTING_VOLUME_KEY = "SettingPanel";

        // ========= 사운드 초기화 (게임 시작 시) ========== 
        void Start()
        {
            LoadSoundSettings();
        }

        // ========= 사운드 설정 로드 ========== 
        private void LoadSoundSettings()
        {
            float bgmVolume = PlayerPrefs.GetFloat(SETTING_VOLUME_KEY, 1f);
            float sfxVolume = PlayerPrefs.GetFloat(SETTING_VOLUME_KEY, 1f);

            bgmSlider.value = bgmVolume;
            sfxSlider.value = sfxVolume;

            // 슬라이더 이벤트 연결
            bgmSlider.onValueChanged.AddListener(SetBGMVolume);
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        // ========= 배경음 볼륨 설정 ==========
        public void SetBGMVolume(float volume)
        {
           // SoundManager.Instance.SetBgmVolume(volume);
        }

        // ========= 효과음 볼륨 설정 ==========
        public void SetSFXVolume(float volume)
        {
           // SoundManager.Instance.SetSfxVolume(volume);
        }

        // ========= 설정 패널 열기 ==========
        public void OpenSettingPanel(Transform parent)
        {
            _currentSettingPanel = Instantiate(settingPanelPrefab, parent);
        }

        // ========= 설정 패널 닫기 ==========
        public void CloseSettingPanel()
        {
           // UIManager.Instance.HideUI(SETTING_VOLUME_KEY);
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




        
        public void SetLoginOnOff() // 디폴트 값 true
        {
            Debug.Log($"자동 로그인 {AutoLogin.GetAutoLogin()} 상태에서 {!AutoLogin.GetAutoLogin()}로 변경");
            AutoLogin.SetAutoLogin(!AutoLogin.GetAutoLogin());
        }
    }

}

