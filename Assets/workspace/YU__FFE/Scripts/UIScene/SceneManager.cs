namespace workspace.YU__FFE.Scripts {
    public class SceneManager : Singleton<SceneManager> {
        private SceneManager() { }

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

        private string scene1 = "DataTestScene";
        private string scene2 = "ServerTestScene";
        private bool isScene1 = true;
        public void RepeatScene() {

            if (isScene1) {
                LoadScene(scene1);
            } else {
                LoadScene(scene2);
            }
            isScene1 = !isScene1;
        }
    }
}
