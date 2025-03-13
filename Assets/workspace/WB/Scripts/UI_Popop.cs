using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;



namespace WB
{
    public class UI_Popup : MonoBehaviour
    {
        public GameObject objPopup;     //root
        public RectTransform rectWindow;//창 크기
        public TextMeshProUGUI textMsg; //child 0
        public Button btnOk;            //child 1
        public Button btnCancel;        //chidl 2
        public TextMeshProUGUI textOk;  //child 1 - child 0
        public TextMeshProUGUI textCancel; //chidl 2 - child 0

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
            //상속받은 컴포넌트에서 추가적인 코드 필요시 실행
            ShowStartEvent();

            objPopup.SetActive(true);

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


        public virtual void ShowStartEvent()
        {

        }

        void HidePopup()
        {
            objPopup.SetActive(false);
        }
    }
}

