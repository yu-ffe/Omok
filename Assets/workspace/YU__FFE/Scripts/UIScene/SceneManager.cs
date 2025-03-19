using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace workspace.YU__FFE.Scripts.UIScene {
    public class SceneManager : Singleton<SceneManager> {

        private SceneManager() {
        }

        // 씬 인덱스를 사용하여 씬을 로드
        public void LoadScene(int sceneNum) {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneNum);
        }

        // 씬 이름을 사용하여 씬을 로드
        public void LoadScene(string sceneName) {
            Debug.Log($"[SceneManager] LoadScene: {sceneName}");
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }

        // 현재 씬을 다시 로드
        public void ReloadCurrentScene() {
            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            UnityEngine.SceneManagement.SceneManager.LoadScene(currentScene);
        }

        bool n = true;
        public void repeatScene() {
            string scene1 = "DataTestScene";
            string scene2 = "ServerTestScene";
            UnityEngine.SceneManagement.SceneManager.LoadScene(n ? scene1 : scene2);
            n = !n;
        }

    }
}
