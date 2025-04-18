using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;
    private Vector3 originalScale;
    public float scaleFactor = 1.1f; // 확대 비율
    public float duration = 0.2f; // 애니메이션 속도

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
    }

    // 게임 오브젝트가 활성화될 때 원래 상태로 복원
    private void OnEnable()
    {
        rectTransform.localScale = originalScale;
    }

    // 게임 오브젝트가 비활성화될 때 원래 상태로 복원
    private void OnDisable()
    {
        rectTransform.localScale = originalScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(ScaleButton(originalScale * scaleFactor));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(ScaleButton(originalScale));
    }

    private IEnumerator ScaleButton(Vector3 targetScale)
    {
        float time = 0f;
        Vector3 startScale = rectTransform.localScale;

        while (time < duration)
        {
            time += Time.deltaTime;
            rectTransform.localScale = Vector3.Lerp(startScale, targetScale, time / duration);
            yield return null;
        }
        rectTransform.localScale = targetScale;
    }
}
