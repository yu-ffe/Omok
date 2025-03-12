using UnityEngine;


namespace WB
{
    abstract public class UI_Panel : MonoBehaviour
    {
        // UI패널 만드실 때 UI_Panel을 상속시켜주시고
        // 필요한 필드값들을 선언하시면 됩니다.
        abstract public void Show();
        abstract public void Hide();
        abstract public void OnEnable();
        abstract public void OnDisable();
    }
}

