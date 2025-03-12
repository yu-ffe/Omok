using TMPro;





namespace WB
{
    public class ExamplePanel : WB.UI_Panel
    {
        public string panelKey = "Exmaple"; // 패널 호출 할 수 있는 키값?
        public TextMeshProUGUI txtCoin;
        public TextMeshProUGUI txtWin;
        public TextMeshProUGUI txtLose;


        public override void Show()
        {
            gameObject.SetActive(true);
        }
        public override void Hide()
        {
            gameObject.SetActive(false);
        }

        public override void OnDisable()
        {
            WB.UI_Manager.Get.AddCallback("coin", RefreshCoin);
            WB.UI_Manager.Get.AddCallback("winlose", RefreshWinLoseStat);
        }

        public override void OnEnable()
        {
            WB.UI_Manager.Get.RemoveCallback("coin", RefreshCoin);
            WB.UI_Manager.Get.RemoveCallback("winlose", RefreshWinLoseStat);
        }

        public void RefreshCoin()
        {

        }

        public void RefreshWinLoseStat()
        {

        }

    }
}

