using Commons.Patterns;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class VictoryEffectManager : Singleton<VictoryEffectManager>
{
    [Header("Victory Effect References")]
    //public GameObject overlayPanel;
    //public ParticleSystem fireworks;
    
    public GameObject[] starPrefabs; // Spark_Star 프리팹
    public RectTransform canvasRect; // UI 기준 RectTransform
    public int burstCount = 12; // 몇 개 뿌릴지
    
    private bool isEffectPlaying = false;

    protected override void Awake()
    {
        base.Awake(); // Singleton 설정 (중복 생성 방지)
    }
    public void ShowVictoryEffect()
    {
        for (int i = 0; i < burstCount; i++)
        {
            // 랜덤으로 프리팹 선택
            GameObject randomStarPrefab = starPrefabs[Random.Range(0, starPrefabs.Length)];

            GameObject star = Instantiate(randomStarPrefab, canvasRect);

            // 랜덤 위치 (중앙 기준 약간 퍼지게)
            Vector2 randPos = Random.insideUnitCircle * 150f;
            star.GetComponent<RectTransform>().anchoredPosition = randPos;

            // 랜덤 회전
            float rot = Random.Range(0f, 360f);
            star.transform.rotation = Quaternion.Euler(0, 0, rot);

            // 선택적으로 랜덤 크기
            float scale = Random.Range(0.8f, 1.2f);
            star.transform.localScale = Vector3.one * scale;
        }

        SoundManager.Instance.PlayWinSound();
    }
    
    /*public void ShowVictoryEffectDelayed(float delay = 0.1f)
    {
        StartCoroutine(PlayAfterDelay(delay));
    }
    
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

    
    

    public void ResetEffect()
    {
        isEffectPlaying = false;
        overlayPanel.SetActive(false);
        fireworks.Stop();
    }
    */
    
    
    
}