using IPS;
using UnityEngine;

public partial class UserData
{
    public static int CurrentSakura => PlayerPrefs.GetInt(SakuraKey, 0);
    public static void SetSakura(int value)
    {
        SetInt(SakuraKey, value);
        EventDispatcher.Instance.Dispatch<EventDefine.OnSakuraChanged>();
        if (CurrentSakura >= GameData.Instance.GetSakuraRateOfLevel(CurrentSakuraLevel + 1).UpPoint)
        {
            SetSakuraLevel(CurrentSakuraLevel + 1);
        }
    }
    public static void AddSakura(int add)
    {
        SetSakura(CurrentSakura + add);
    }
    public static int CurrentSakuraLevel => GetInt(SakuraLevelKey, 1);
    public static void SetSakuraLevel(int value)
    {
        SetInt(SakuraLevelKey, value);
        SetCash(CurrentCash + GameData.Instance.GetSakuraRateOfLevel(CurrentSakuraLevel).Reward);
        SetFakeCash(CurrentShowingCash + GameData.Instance.GetSakuraRateOfLevel(CurrentSakuraLevel).Reward);
        EventDispatcher.Instance.Dispatch<EventDefine.OnUpLevel>();
    }
}
