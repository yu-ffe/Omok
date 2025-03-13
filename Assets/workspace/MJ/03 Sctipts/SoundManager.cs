using System;

namespace MJ
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set;}
        
        [Header("오디오 소스")]
        public AudioSource bgmSource;
        public AudioSource[] sfxSources;
        
        private const string BGM_VOLUME_KEY = "BGM_VOLUME";
        private const string SFX_VOLUME_KEY = "SFX_VOLUME";

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            InitialixVolume();
        }

        void InitialixVolume()
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
            if (index >= 0 && index < sfxSources.Length)
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

        public void SetBgmVolume(float volume)
        {
            bgmSource.volume = volume;
            PlayerPrefs.SetFloat(BGM_VOLUME_KEY, volume);
            PlayerPrefs.Save();
        }

        public void SetSfxVolume(float volume)
        {
            foreach (var sgx in sfxSources)
            {
                if(sgx != null) sgx.volume = volume;
            }
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