using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System;
using IPS;

public class UpgradeRoomOption : MonoBehaviour
{
    [SerializeField] uint optionNum;
    [SerializeField] Image avatar;
    [SerializeField] TextMeshProUGUI rewardSakuraTxt;
    [SerializeField] TextMeshProUGUI rewardMoneyTxt;
    protected Action callBack;
    public void SetAvatar(Sprite avatar)
    {
        if (avatar == null) return;
        this.avatar.sprite = avatar;
    }
    public void SetSakuraRewardText(int amount)
    {
        rewardSakuraTxt.text = $"+{amount}";
    }
    public void SetRewardMoneyText(int amount)
    {
        rewardMoneyTxt.text = $"x{amount}";
    }
    protected virtual void ChooseOption()
    {
        if (callBack != null) callBack();
        this.Dispatch(new EventDefine.OnChosenVisualOption { chosenOption = optionNum });
    }
}
