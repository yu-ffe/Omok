using System.Collections;
using UnityEngine;

public class GameSceneController : MonoBehaviour
{
    IEnumerator Start()
    {
        Debug.Log("[GameSceneController] Start 진입");

        yield return new WaitUntil(() => UI_Manager.Instance != null);
        Debug.Log("[GameSceneController] UI_Manager 인스턴스 확인됨");

        yield return new WaitUntil(() => UI_Manager.Instance.HasPanel(UI_Manager.PanelType.GameEnd));
        Debug.Log("[GameSceneController] EndGamePanel 등록 완료됨");

        yield return new WaitUntil(() => UI_Manager.Instance.popup != null);
        Debug.Log("[GameSceneController] PopupPanel 등록 완료됨");
    }
}
