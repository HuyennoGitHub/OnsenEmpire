using UnityEngine.UI;
using UnityEngine;
using System;

public class UpgradeRoomOptionFree : UpgradeRoomOption
{
    [SerializeField] Button getBtn;

    private void Start()
    {
        getBtn.onClick.AddListener(ChooseOption);
    }
    public void UpdateOption(Sprite avatar, int sakura, int money, Action callBack)
    {
        SetAvatar(avatar);
        SetRewardMoneyText(money);
        SetSakuraRewardText(sakura);
        this.callBack = callBack;
    }
}
