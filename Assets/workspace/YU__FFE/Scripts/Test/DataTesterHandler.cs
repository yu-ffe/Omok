using TMPro;
using UnityEngine;
using UnityEngine.UI;
using workspace.YU__FFE.Scripts.Server.Network;

namespace workspace.YU__FFE.Scripts.Test {
    public class DataTesterHandler : MonoBehaviour {
        public TextMeshProUGUI coinText; // coins 값 출력: 서버 연결 테스트 확인용
        public TextMeshProUGUI message; // 서버로부터 받은 값
        
        public Button gameWin; // 이 버튼은 게임 승/패를 서버로 전송하는 버튼
        public Button gameLose; // 게임 패를 서버로 전송하는 버튼
        
        public Button getInfo; // 서버로부터 정보를 가져오는 버튼

        public Button ranks; // 서버로부터 랭킹 정보를 가져오는 버튼
        
        public void Start() {
            gameWin.onClick.AddListener(() => gameEnd(true));  // 승리일 경우
            gameLose.onClick.AddListener(() => gameEnd(false)); // 패배일 경우
            getInfo.onClick.AddListener(updateInfo);
            ranks.onClick.AddListener(getRanks);
        }
        
        public void gameEnd(bool result) {
            // 게임 결과 보내기
            NetworkManager.SendGameResult(result);
            // 게임 결과에 따른 값 받아오기 (이 로직 없이 승/패를 더해도 문제 없음)
            updateInfo();
        }
        
        public void updateInfo() {
            // 서버로부터 정보를 가져오는 메서드
            // 서버로부터 받은 정보를 출력
            NetworkManager.GetUserInfoRequest( (response) => {
                message.text = response.ToString();
            });
        }
        
        public void getRanks() {
            NetworkManager.GetRanksRequest( (response) => {
                message.text = response.ToString();
            });
            // 서버로부터 랭킹 정보를 가져오는 메서드
            // 서버로부터 받은 랭킹 정보를 출력
            message.text = "랭킹 정보";
        }
    }
}
