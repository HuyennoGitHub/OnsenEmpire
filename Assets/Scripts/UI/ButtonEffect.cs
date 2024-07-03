using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

[RequireComponent(typeof(Button))]
public class ButtonEffect : MonoBehaviour, IPointerClickHandler
{
    Button btn;
    public Button Button
    {
        get
        {
            if (btn != null) return btn;
            btn = GetComponent<Button>();
            return btn;
        }
    }
    private bool CanTouch => btn != null && btn.interactable;
    private void Start()
    {
        btn = GetComponent<Button>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!CanTouch) return;
        SFX.Instance.PlayClickSound();
    }
}
