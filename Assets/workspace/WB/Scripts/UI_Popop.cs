using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace WB
{
    public class UI_Popup : MonoBehaviour
    {
        public GameObject objPopup;     //root
        public RectTransform rectWindow;//창 크기
        public TextMeshProUGUI textMsg; //child 0
        public UserScorePopup scoreBoard; // child 1
        public Button btnOk;            //child 2
        public Button btnCancel;        //chidl 3
        public TextMeshProUGUI textOk;  //child 2 - child 0
        public TextMeshProUGUI textCancel; //chidl 3 - child 0



        void Awake()
        {
            if (UI_Manager.Instance != null)
            {
                UI_Manager.Instance.popup = this;
                Debug.Log("[UI_Popup] Popup이 UI_Manager에 정상적으로 등록됨");
            }
            else
            {
                Debug.LogError("[UI_Popup] UI_Manager가 초기화되지 않았습니다.");
            }

        }
        void Start()
        {
            UI_Manager.Instance.AddCallback("result", Show_WithScore);
            gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            UI_Manager.Instance.RemoveCallback("result");
        }

        public virtual void ShowStartEvent() { }


        /// <summary> 팝업창을 띄웁니다. </summary>
        /// <param name="msg">내용</param>
        /// <param name="okText">확인버튼 텍스트</param>
        /// <param name="cancelText">취소버튼 텍스트</param>
        /// <param name="width">창크기 가로</param>
        /// <param name="height">창크기 세로</param>
        /// <param name="okAction">확인 버튼 누를시 실행될 함수</param>
        /// <param name="cancelAction">취소 버튼 누를시 실행될 함수</param>
        public void Show(
            string msg,  // 메세지
            string okText = null, string cancelText = null, // 확인(위쪽) 버튼 메세지, 취소(아래쪽) 버튼 메세지
                                                            // //// float width = 600, float height = 600, //창 크기 (삭제)
            UnityAction okAction = null, UnityAction cancelAction = null)   // 확인,취소 각각 누를시 실행도리 이벤트,
        {
            ShowStartEvent();

            scoreBoard.gameObject.SetActive(false);
            //상속받은 컴포넌트에서 추가적인 코드 필요시 실행

            objPopup.SetActive(true);

            textMsg.text = msg;


            //버튼 이벤트 초기화
            btnOk.onClick.RemoveAllListeners();
            btnCancel.onClick.RemoveAllListeners();
            btnOk.onClick.AddListener(HidePopup);
            btnCancel.onClick.AddListener(HidePopup);
            // //창 크기, 메시지 설정
            //// rectWindow.sizeDelta = new Vector2(width, height);
            //// textMsg.text = msg;
            //버트 이벤트 연결
            if (okAction != null)
                btnOk.onClick.AddListener(okAction);
            if (cancelAction != null)
                btnCancel.onClick.AddListener(cancelAction);
            //버튼 메세지 변경 
            textOk.text = string.IsNullOrEmpty(okText) ? "확인" : okText;
            textCancel.text = string.IsNullOrEmpty(cancelText) ? "취소" : cancelText;

            // 연결된 이벤트가 둘 다 없으면 확인 버튼만 활성화
            btnCancel.gameObject.SetActive(okAction != null || cancelAction != null);
        }


        public void Show_WithScore()
        {
            //게임 결과 불러오기
            string result = "승리";// "패배"
            int value = 0;
            string get = value > 0 ? "얻었" : "잃었";
            string textBtn = "";
            string resultMsg = $"게임에서 {result}했습니다.{value}승급 포인트를 {get}습니다.";

            UnityAction nextAction = ShowAskRecord;
            Show(
                resultMsg,
                "확인", textBtn,
                okAction: ExitToMain,
                cancelAction: nextAction);

            scoreBoard.ShowScore(value);//Next Score X

        }


        void ShowAskRecord()
        {
            //게임 로직에 따라 변경
            UI_Manager.Instance.popup.Show(
                $"현재게임의 기보를 저장하시겠습니까?",
                "저장", "저장 안 함",
                okAction: () =>
                {
                    //기보 저장
                },
                cancelAction: () =>
                {
                    //나가기
                    ExitToMain();
                }
                );
        }

        void ExitToMain()
        {
            // UI_Manager.Instance.Show(UI_Manager.PanelType.Main);
            SceneManager.LoadScene("Main");
        }

        public void HidePopup()
        {
            objPopup.SetActive(false);
        }


        /// <summary>
        /// 확인 버튼을 누르면 특정 함수를 실행하는 팝업을 띄운다.
        /// </summary>
        public void Show(string message, Action onConfirm)
        {
            textMsg.text = message;
            gameObject.SetActive(true);

            btnOk.onClick.RemoveAllListeners();
            btnOk.onClick.AddListener(() =>
            {
                Hide(); // 팝업 닫기
                onConfirm?.Invoke(); // 확인 버튼을 누르면 지정된 함수 실행
            });

            if (btnCancel != null)
            {
                btnCancel.onClick.RemoveAllListeners();
                btnCancel.onClick.AddListener(Hide);
            }
        }
        
        /// <summary>
        /// 팝업을 숨긴다.
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}

