using System.Collections.Generic;
using UnityEngine;
using workspace.YU__FFE.Scripts.Data;
using workspace.YU__FFE.Scripts.Data.workspace.YU__FFE.Scripts.Data;

namespace workspace.YU__FFE.Scripts {
    public class UIController : MonoBehaviour {
        // UI 프리팹 목록을 인스펙터에서 관리하도록 설정
        [SerializeField]
        private List<UIItem> uiItems;

        private string _currentUIKey = null;  // 현재 표시된 UI의 key

        private void Start() {
            // 초기 UI 세팅: Start에서 자동으로 모든 UI를 표시
            foreach (UIItem uiItem in uiItems) {
                ShowUI(uiItem.uiKey);  // 모든 UI를 처음에 표시
            }
        }

        /// <summary>
        /// 특정 UI를 화면에 표시합니다. 기존 UI를 끄고 새 UI로 교체합니다.
        /// </summary>
        /// <param name="key">UI의 고유 키</param>
        public void ShowUI(string key) {
            // 이전에 표시된 UI가 있으면 숨김
            if (_currentUIKey != null && _currentUIKey != key) {
                HideUI(_currentUIKey);
            }

            // UIKey에 맞는 UIItem을 찾아 ShowUI를 호출하여 화면에 표시
            UIItem uiItem = uiItems.Find(item => item.uiKey == key);
            if (uiItem != null) {
                UIManager.Instance.ShowUI(key, uiItem.prefab, null);
                _currentUIKey = key;  // 현재 표시된 UI를 업데이트
            }
        }

        /// <summary>
        /// 특정 UI를 화면에서 숨깁니다.
        /// </summary>
        /// <param name="key">UI의 고유 키</param>
        public void HideUI(string key) {
            UIManager.Instance.HideUI(key);
            // 숨긴 UI는 currentUIKey에서 제거
            if (_currentUIKey == key) {
                _currentUIKey = null;
            }
        }

        /// <summary>
        /// 모든 UI를 화면에서 숨깁니다.
        /// </summary>
        public void HideAllUI() {
            foreach (UIItem uiItem in uiItems) {
                UIManager.Instance.HideUI(uiItem.uiKey);
            }
            _currentUIKey = null;  // 모든 UI를 숨기면 currentUIKey도 초기화
        }
    }
}
