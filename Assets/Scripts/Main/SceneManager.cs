using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManager : Singleton<SceneManager> {
    [SerializeField] private GameObject loadingScreen; // 로딩 화면 UI
    [SerializeField] private Slider progressBar; // 로딩 진행 바
    private Canvas loadingCanvas;

    private SceneManager() {}

    public void LoadScene(string sceneName) {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void LoadSceneAsync(string sceneName) {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    public void ReloadCurrentScene() {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        LoadSceneAsync(currentScene);
    }

    private IEnumerator LoadSceneCoroutine(string sceneName) {
        ShowLoadingScreen();

        AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false; // 로딩 완료 후 자동 전환 방지

        while (!operation.isDone) {
            float progress = Mathf.Clamp01(operation.progress / 0.9f); // 로딩 진행도 (0 ~ 1)
            progressBar.value = progress;

            if (operation.progress >= 0.9f) { // 씬 로딩이 끝났다면
                yield return new WaitForSeconds(0.5f); // 잠깐 대기 (부드러운 전환을 위해)
                operation.allowSceneActivation = true; // 씬 활성화
            }

            yield return null;
        }

        HideLoadingScreen();
    }

    private void ShowLoadingScreen() {
        if (loadingCanvas == null) {
            loadingCanvas = new GameObject("LoadingCanvas").AddComponent<Canvas>();
            loadingCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            loadingCanvas.sortingOrder = 1000;

            GameObject panel = new GameObject("Panel", typeof(Image));
            panel.transform.SetParent(loadingCanvas.transform);
            panel.GetComponent<Image>().color = new Color(0, 0, 0, 0.7f); // 반투명 배경

            GameObject sliderObj = new GameObject("ProgressBar", typeof(Slider));
            sliderObj.transform.SetParent(panel.transform);
            progressBar = sliderObj.GetComponent<Slider>();
            progressBar.minValue = 0;
            progressBar.maxValue = 1;
        }

        loadingCanvas.gameObject.SetActive(true);
    }

    private void HideLoadingScreen() {
        if (loadingCanvas != null) {
            loadingCanvas.gameObject.SetActive(false);
        }
    }
}
