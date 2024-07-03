using TMPro;
using UnityEngine.UI;
using UnityEngine;
using IPS;

public class SakuraCurrencyListener : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private Image progressBar;
    [SerializeField] private TextMeshProUGUI currentSakuraText;
    [SerializeField] private TextMeshProUGUI nextLevelSakuraText;

    void Start()
    {
        this.AddListener<EventDefine.OnUpLevel>(LevelUp);
        this.AddListener<EventDefine.OnSakuraChanged>(OnCurrencyChange);
        UpdateVisual();
    }
    private void OnCurrencyChange()
    {
        UpgradeProgress();
    }
    private void UpgradeProgress()
    {
        int current = UserData.CurrentSakura;
        int finish = GameData.Instance.GetSakuraRateOfLevel(UserData.CurrentSakuraLevel + 1).UpPoint;
        progressBar.fillAmount = (float)current / finish;
        currentSakuraText.text = current.ToString();
        nextLevelSakuraText.text = finish.ToString();
    }
    private void LevelUp()
    {
        int level = UserData.CurrentSakuraLevel;
        int reward = GameData.Instance.GetSakuraRateOfLevel(level).Reward;
        UICtrl.Instance.Get<LevelUpPanel>().SetPanelText(level, reward);
        UICtrl.Instance.Show<LevelUpPanel>();
        UpdateVisual();
    }
    private void UpdateVisual()
    {
        currencyText.text = UserData.CurrentSakuraLevel.ToString();
        UpgradeProgress();
    }
}
