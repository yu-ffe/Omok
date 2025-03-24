using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using workspace.Ham6._03_Sctipts.Game;

namespace KimHyeun {
    public class GameTest : MonoBehaviour
    {
        [SerializeField] Button testButton;

        private void Start()
        {
            testButton.onClick.AddListener(()=> { GameStartTest(); });
        }

        public void GameStartTest()
        {
            GameManager.Instance.ChangeToGameScene(Constants.GameType.SinglePlayer);
        }
    }
}

