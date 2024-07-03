using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class OfferPanel : Panel
{
    [Header("Button")]
    [SerializeField] ButtonEffect closeBtn;
    [SerializeField] ButtonEffect useGoldBtn;
    [SerializeField] ButtonEffect watchAdsBtn;

    [Header("Text")]
    [SerializeField] TextMeshProUGUI titlePopup;
    [SerializeField] TextMeshProUGUI content;
    [SerializeField] TextMeshProUGUI value;
    [SerializeField] TextMeshProUGUI useTime;

    [Header("Image")]
    [SerializeField] Image avatar;
    [SerializeField] GameObject moneyUnitIcon;
    [SerializeField] GameObject useTimeBanner;

    private Action<BoosterCollectable> removeOnCallBack;
    private BoosterCollectable boosterObj;

    private bool isBooster;

    protected override void OnEnable()
    {
        base.OnEnable();
        closeBtn.Button.onClick.AddListener(OnClickCloseBtn);
        useGoldBtn.Button.onClick.AddListener(OnClickUseGoldBtn);
        watchAdsBtn.Button.onClick.AddListener(OnClickWatchAdsBtn);
    }
    private void OnClickCloseBtn()
    {
        Hide();
    }
    private void OnClickUseGoldBtn()
    {
        Debug.Log("UseGold");
        if (isBooster)
        {
            if (removeOnCallBack != null) removeOnCallBack(boosterObj);
            boosterObj.UseGold();
        }
        Hide();
    }
    private void OnClickWatchAdsBtn()
    {
        Debug.Log("WatchAds");
        if (isBooster)
        {
            if (removeOnCallBack != null) removeOnCallBack(boosterObj);
        }
        Hide();
    }
    public void SetPanelInfo(string title, string content, int value, float useTime, Sprite avatar, Action<BoosterCollectable> callBack, BoosterCollectable obj)
    {
        titlePopup.text = title;
        this.content.text = content;
        if (obj.Type == BoosterType.Transport) this.value.text = $"+{value}%";
        else this.value.text = $"+{value}";
        this.useTime.text = $"{useTime}m";
        this.avatar.sprite = avatar;
        removeOnCallBack = callBack;
        boosterObj = obj;
        if (boosterObj.Type == BoosterType.Money)
        {
            moneyUnitIcon.SetActive(true);
            useTimeBanner.SetActive(false);

        }
        else
        {
            moneyUnitIcon.SetActive(false);
            useTimeBanner.SetActive(true);
        }
        isBooster = true;
    }
}
