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

        // 고정 위치로 설정 (Vector3.zero 기준에서 아래로 살짝)
        fireworks.transform.position = new Vector3(0f, -3.8f, 0f);

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