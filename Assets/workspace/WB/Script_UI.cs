using System.Collections.Generic;
using UnityEngine.Events;



public interface IPopopUI
{
    public void Show(string msg, string okText = null, string cancelText = null, UnityAction okAction = null, UnityAction cancelAction = null);
    public void Hide();
    // public void Refresh();
}

public interface IObserverUI
{
    string NotifyKey { get; set; }
    public void OnNotify();
    public void OnNotify(string msg);
}
