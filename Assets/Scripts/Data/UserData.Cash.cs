using UnityEngine;

public partial class UserData
{
    public static int CurrentCash => PlayerPrefs.GetInt(CurrentCashKey, 0);
    public static void SetCash(int value)
    {
        SetInt(CurrentCashKey, value);
    }
    
}
