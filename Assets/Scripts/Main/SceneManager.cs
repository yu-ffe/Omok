using UnityEngine;
using UnityEngine.SceneManagement;
using workspace.YU__FFE.Scripts.Common;

public class SceneManager : Singleton<SceneManager> {
    private SceneManager() {
    }

    public void LoadScene(string sceneName) {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void LoadSceneAsync(string sceneName) {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
    }

    public void ReloadCurrentScene() {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        UnityEngine.SceneManagement.SceneManager.LoadScene(currentScene);
    }
}
