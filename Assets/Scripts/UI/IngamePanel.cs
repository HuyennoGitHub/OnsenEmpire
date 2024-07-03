using UnityEngine;

public class IngamePanel : Panel
{
    [SerializeField] ButtonEffect settingButton;
    [SerializeField] BoosterClock clock;

    protected override void OnEnable()
    {
        base.OnEnable();
        settingButton.Button.onClick.AddListener(OnClickSettingButton);
    }

    private void OnClickSettingButton()
    {
        UICtrl.Instance.Show<SettingPanel>();
    }
    public void ShowClock(float time)
    {
        clock.gameObject.SetActive(true);
        clock.AddTime(time);
        clock.StopCounting();
        clock.StartCounting();
    }
}
