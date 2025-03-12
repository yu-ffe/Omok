using System.Collections.Generic;
using UnityEngine;
using workspace.YU__FFE.Scripts.Data.workspace.YU__FFE.Scripts.Data;

namespace workspace.YU__FFE.Scripts {
    public class UIController : MonoBehaviour {
        [SerializeField]
        private List<UIItem> uiItems;

        private string _currentUIKey = null;

        private void Start() {
            // UI가 초기화 될 때 모든 UI를 표시하도록 설정
            foreach (UIItem uiItem in uiItems) {
                ShowUI(uiItem.uiKey);
            }
        }

        public void ShowUI(string key) {
            // 현재 표시된 UI가 있으면 숨기기
            if (_currentUIKey != null && _currentUIKey != key) {
                HideUI(_currentUIKey);
            }

            // UIKey에 맞는 UIItem을 찾아 ShowUI를 호출하여 화면에 표시
            UIItem uiItem = uiItems.Find(item => item.uiKey == key);
            if (uiItem != null) {
                // UI를 보여주기 위해 UIManager의 ShowUI 메서드를 호출
                UIManager.Instance.ShowUI(key, uiItem.prefab, null);
                _currentUIKey = key;  // 현재 표시된 UI를 업데이트
            }
        }

        public void HideUI(string key) {
            UIManager.Instance.HideUI(key);
            // 숨긴 UI가 현재 UI라면 currentUIKey를 초기화
            if (_currentUIKey == key) {
                _currentUIKey = null;
            }
        }

        public void HideAllUI() {
            foreach (UIItem uiItem in uiItems) {
                UIManager.Instance.HideUI(uiItem.uiKey);
            }
            _currentUIKey = null;  // 모든 UI를 숨기고 currentUIKey 초기화
        }
    }
}
