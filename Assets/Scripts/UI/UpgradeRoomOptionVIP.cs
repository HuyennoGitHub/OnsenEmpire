using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System;

public class UpgradeRoomOptionVIP : UpgradeRoomOption
{
    [SerializeField] TextMeshProUGUI useGoldTxt;
    [SerializeField] Button useGoldBtn;
    [SerializeField] Button watchAdsBtn;

    private void Start()
    {
        useGoldBtn.onClick.AddListener(UseGold);
        watchAdsBtn.onClick.AddListener(WatchAds);
    }
    private void UseGold()
    {
        Debug.Log("useGold");
        ChooseOption();
    }
    private void WatchAds()
    {
        Debug.Log("watch Ads");
        ChooseOption();
    }
    public void UpdateOption(Sprite avatar, int sakura, int money, int gold, Action callback)
    {
        SetAvatar(avatar);
        SetRewardMoneyText(money);
        SetSakuraRewardText(sakura);
        useGoldTxt.text = gold.ToString();
        callBack = callback;
    }
}
