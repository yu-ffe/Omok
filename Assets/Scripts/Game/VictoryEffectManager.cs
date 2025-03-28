using System.Collections;
using UnityEngine;
using DG.Tweening;

public class VictoryEffectManager : Singleton<VictoryEffectManager>
{
    [Header("Victory Effect References")]
    public GameObject overlayPanel;
    public ParticleSystem fireworks;

    private bool isEffectPlaying = false;

    protected override void Awake()
    {
        base.Awake(); // Singleton 설정 (중복 생성 방지)
    }
    
    public void ShowVictoryEffectDelayed(float delay = 0.1f)
    {
        StartCoroutine(PlayAfterDelay(delay));
    }
    
    private IEnumerator PlayAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("[이펙트 실행됨]"); // 🔍 이거 꼭 확인

        Camera cam = Camera.main;
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 5f);
        Vector3 worldPos = cam.ScreenToWorldPoint(screenCenter);
        fireworks.transform.position = worldPos;

        fireworks.gameObject.SetActive(true);
        fireworks.Play();
    
        SoundManager.Instance.PlayWinSound();
    }

    public void ShowVictoryEffect()
    {
        if (isEffectPlaying) return;
        isEffectPlaying = true;
        
        Camera cam = Camera.main;
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 1f);
        Vector3 worldPos = cam.ScreenToWorldPoint(screenCenter);
        fireworks.transform.position = worldPos;

        fireworks.Play();
        SoundManager.Instance.PlayWinSound();
    }

    public void ResetEffect()
    {
        isEffectPlaying = false;
        overlayPanel.SetActive(false);
        fireworks.Stop();
    }
    
    
}