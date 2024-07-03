using UnityEngine;

public class SettingPanel : Panel
{
    [SerializeField] ButtonEffect btnClose;

    protected override void OnEnable()
    {
        base.OnEnable();
        btnClose.Button.onClick.AddListener(OnClickCloseButton);
    }

    private void OnClickCloseButton()
    {
        Hide();
    }
}
