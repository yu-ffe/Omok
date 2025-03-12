using System.Collections.Generic;
using UnityEngine.Events;


namespace WB
{
    public interface IPopopUI
    {
        public void Show(string msg, string okText = null, string cancelText = null, UnityAction okAction = null, UnityAction cancelAction = null);
        public void Hide();
        // public void Refresh();
    }
}
