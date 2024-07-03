using TMPro;
using UnityEngine;

public class LevelUpPanel : Panel
{
    [SerializeField] TextMeshProUGUI level;
    [SerializeField] TextMeshProUGUI rewardTxt;
    [SerializeField] ButtonEffect btnClose;
    [SerializeField] ButtonEffect btnGold;
    [SerializeField] ButtonEffect btnAds;
    private int reward;

    protected override void OnEnable()
    {
        base.OnEnable();
        btnClose.Button.onClick.AddListener(OnClickCloseBtn);
        btnGold.Button.onClick.AddListener(OnClickGoldBtn);
        btnAds.Button.onClick.AddListener(OnClickAdsBtn);
    }

    private void OnClickCloseBtn()
    {
        Hide();
        UserData.SetCash(UserData.CurrentCash + reward);
        UserData.SetFakeCash(UserData.CurrentShowingCash + reward);
    }
    private void OnClickGoldBtn()
    {
        Hide();
        UserData.SetCash(UserData.CurrentCash + reward * 2);
        UserData.SetFakeCash(UserData.CurrentShowingCash + reward * 2);
    }
    private void OnClickAdsBtn()
    {
        Hide();
        UserData.SetCash(UserData.CurrentCash + reward * 2);
        UserData.SetFakeCash(UserData.CurrentShowingCash + reward * 2);
    }
    public void SetPanelText(int level, int reward)
    {
        this.level.text = $"{level}";
        this.rewardTxt.text = $"{reward}";
        this.reward = reward;
    }
}
