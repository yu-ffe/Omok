using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using workspace.Ham6._03_Sctipts.Game;

namespace MyNamespace {

    public class MainPanelController : MonoBehaviour {
        public void OnClickSinglePlayButton() {
            Debug.Log($"SinglePlayer 버튼이 눌림");
            GameManager.Instance.ChangeToGameScene(Constants.GameType.SinglePlayer);
        }

        public void OnClickDualPlayButton() {
            Debug.Log($"DualPlayer 버튼이 눌림");
            GameManager.Instance.ChangeToGameScene(Constants.GameType.DualPlayer);
        }

        public void OnClickMultiplayButton() {
            Debug.Log($"MultiPlayer 버튼이 눌림");
            GameManager.Instance.ChangeToGameScene(Constants.GameType.MultiPlayer);
        }
    }
}
