using UnityEngine;


    abstract public class UI_Panel : MonoBehaviour
    {
        // UI패널 만드실 때 UI_Panel을 상속시켜주시고
        // 필요한 필드값들을 선언하시면 됩니다.
        // public string panelKey;
        public UI_Manager.PanelType panelType;
        
        protected virtual void Awake()
        {
            if (panelType == UI_Manager.PanelType.None)
                return;

            if (UI_Manager.Instance != null)
            {
                UI_Manager.Instance.AddPanel(panelType, this);
                Debug.Log($"[UI_Panel] {panelType} 패널 자동 등록됨 (Awake)");
            }
        }
        
        abstract public void Show();
        abstract public void Hide();
        abstract public void OnEnable();
        abstract public void OnDisable();
        public void LoadPlayerDataAndInitializeUI() {
        }
    }


