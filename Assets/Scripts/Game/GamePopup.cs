using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class GamePopup : MonoBehaviour
{
    [Header("UI Components")]
    public TMP_Text popupText;
    public TMP_Text confirmButtonText;
    public TMP_Text cancelButtonText;

    public Button confirmButton;
    public Button cancelButton;

    /// <summary>
    /// 팝업 초기 설정 메서드
    /// </summary>
    public void Setup(string message, string confirmText, UnityAction confirmAction, 
        string cancelText = null, UnityAction cancelAction = null)
    {
        popupText.text = message;
        
        confirmButtonText.text = confirmText;
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(confirmAction);
        confirmButton.onClick.AddListener(ClosePopup);

        if (!string.IsNullOrEmpty(cancelText))
        {
            cancelButton.gameObject.SetActive(true);
            cancelButtonText.text = cancelText;
            cancelButton.onClick.RemoveAllListeners();
            if (cancelAction != null)
                cancelButton.onClick.AddListener(cancelAction);
            cancelButton.onClick.AddListener(ClosePopup);
        }
        else
        {
            cancelButton.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 팝업 활성화
    /// </summary>
    public void OpenPopup()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 팝업 비활성화
    /// </summary>
    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }
}
