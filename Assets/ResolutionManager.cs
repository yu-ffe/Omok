using UnityEngine;

public class ResolutionManager : MonoBehaviour {
    void Start() {
        // 1920x1080 창 모드 실행
        Screen.SetResolution(500, 1000, false);
    }
}
