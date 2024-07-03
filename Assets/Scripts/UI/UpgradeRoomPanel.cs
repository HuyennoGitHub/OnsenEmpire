using TMPro;
using UnityEngine;

public class UpgradeRoomPanel : Panel
{
    [SerializeField] TextMeshProUGUI levelText;
    [Header("Visual Options")]
    [SerializeField] UpgradeRoomOptionFree option1;
    [SerializeField] UpgradeRoomOptionFree option2;
    [SerializeField] UpgradeRoomOptionVIP optionVIP;

    public void SetLevelTitleText(int nextLv)
    {
        levelText.text = $"LEVEL {nextLv}";
    }
    public void SetOption1(Sprite avatar, int sakura, int money)
    {
        option1.UpdateOption(avatar, sakura, money, Hide);
    }
    public void SetOption2(Sprite avatar, int sakura, int money)
    {
        option2.UpdateOption(avatar, sakura, money, Hide);
    }
    public void SetOptionVIP(Sprite avatar, int sakura, int money, int goldUse)
    {
        optionVIP.UpdateOption(avatar, sakura, money, goldUse, Hide);
    }
}
