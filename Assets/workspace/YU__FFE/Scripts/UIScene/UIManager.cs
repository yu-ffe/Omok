using System.Collections.Generic;
using UnityEngine;

namespace workspace.YU__FFE.Scripts.UIScene {
    public class UIManager : Common.Doubleton<UIManager> {
        private readonly Dictionary<string, GameObject> _uiInstances = new Dictionary<string, GameObject>();

        public GameObject ShowUI(string key, GameObject prefab, Transform parent) {
            if (_uiInstances.TryGetValue(key, out GameObject ui)) {
                return ui;  // 이미 UI가 생성되어 있다면 기존 UI를 반환
            }

            GameObject uiInstance = Instantiate(prefab, parent);  // UI 생성
            _uiInstances[key] = uiInstance;  // 생성된 UI를 관리
            return uiInstance;
        }

        public GameObject GetUI(string key) {
            return _uiInstances.GetValueOrDefault(key);  // UI 인스턴스를 반환
        }

        public void HideUI(string key) {
            if (_uiInstances.TryGetValue(key, out GameObject uiInstance)) {
                Destroy(uiInstance);  // UI 인스턴스 삭제
                _uiInstances.Remove(key);  // 관리 목록에서 제거
            }
        }

        public void ClearAllUI() {
            foreach (GameObject ui in _uiInstances.Values) {
                Destroy(ui);  // 모든 UI 삭제
            }
            _uiInstances.Clear();  // UI 인스턴스 목록 비우기
        }
    }
}
