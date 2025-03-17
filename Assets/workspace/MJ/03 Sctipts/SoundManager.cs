using UnityEngine.Audio;

namespace MJ
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class SoundManager : Singleton<SoundManager>
    {
        [Header("오디오 소스")]
        public AudioSource bgmSource;
        public AudioSource[] sfxSources;
        
        [Header("오디오 믹서")]
        public AudioMixer audioMixer;
        
        private const string BGM_VOLUME_KEY = "BGM_VOLUME";
        private const string SFX_VOLUME_KEY = "SFX_VOLUME";
        
        protected override void Awake()
        {
            base.Awake();
            InitializeVolume();
        }
        
        /// <summary> 
        /// 볼륨 초기화 (PlayerPrebs 저장 된 값 불러오기)
        /// </summary>
        void InitializeVolume()
        {
            float saveBgmVolume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 1.0f);
            float saveSfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1.0f);
            
            SetBgmVolume(saveBgmVolume);
            SetSfxVolume(saveSfxVolume);
        }

        public void PlayBGM(AudioClip clip, bool loop = true)
        {
            bgmSource.clip = clip;
            bgmSource.loop = loop;
            bgmSource.Play();
        }

        public void PlaySFX(int index)
        {
            if (index >= 0 && index < sfxSources.Length && sfxSources[index] != null)
                sfxSources[index].Play();
        }
        
        // [상황별] 바둑돌 놓기 효과음 호출 함수
        public void PlayPlacingSound()
        {
            PlaySFX(0);
        }
        
        // [상황별] Win 효과음 호출 함수
        public void PlayWinSound()
        {
            PlaySFX(1);
        }
        
        // [상황별] GameOver 효과음 호출 함수
        public void PlayGameOverSound()
        {
            PlaySFX(2);
        }
        
        // [상황별] Timer 효과음 호출 함수
        public void PlayTimerSound()
        {
            PlaySFX(3);
        }

        public void SetBgmVolume(float value)
        {
            float volume = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
            audioMixer.SetFloat("BGMVolume", volume);
            PlayerPrefs.SetFloat(BGM_VOLUME_KEY, volume);
            PlayerPrefs.Save();
        }

        public void SetSfxVolume(float value)
        {
            float volume = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
            audioMixer.SetFloat("SFXVolume", volume);
            PlayerPrefs.SetFloat(SFX_VOLUME_KEY, volume);
            PlayerPrefs.Save();
        }

        public void MuteAll(bool mute)
        {
            bgmSource.mute = mute;
            foreach (var sfx in sfxSources)
            {
                if (sfx != null) sfx.mute = mute;
            }
        }
        
    }
}