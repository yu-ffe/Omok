using TMPro;

public class ExamplePanel : UI_Panel {
    // public string panelKey = "Exmaple"; // 패널 호출 할 수 있는 키값?
    public TextMeshProUGUI txtCoin;
    public TextMeshProUGUI txtWin;
    public TextMeshProUGUI txtLose;


    void Start() {
        UI_Manager.Instance.AddPanel(panelType, this);
    }
    public override void Show() {
        gameObject.SetActive(true);
    }
    public override void Hide() {
        gameObject.SetActive(false);
    }

    public override void OnDisable() {
        UI_Manager.Instance.AddCallback("coin", RefreshCoin);
        UI_Manager.Instance.AddCallback("winlose", RefreshWinLoseStat);
    }

    public override void OnEnable() {
        UI_Manager.Instance.RemoveCallback("coin");
        UI_Manager.Instance.RemoveCallback("winlose");
    }

    public void RefreshCoin() {

    }

    public void RefreshWinLoseStat() {

    }

}
