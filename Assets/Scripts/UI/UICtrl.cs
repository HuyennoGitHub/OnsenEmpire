using System;
using UnityEngine;

public class UICtrl : MonoBehaviour
{
    public static UICtrl Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    [SerializeField] Panel[] panels;
    public void Show<T>() where T : Panel
    {
        var panel = Get<T>();
        if (panel == null) return;
        if (panel.isActiveAndEnabled) return;
        panel.gameObject.SetActive(true);
    }
    public T Get<T>() where T : Panel
    {
        var p = Array.Find(panels, panel => panel is T);
        if (p != null) return (T)p;
        return null;
    }
}
