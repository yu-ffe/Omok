using Commons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

