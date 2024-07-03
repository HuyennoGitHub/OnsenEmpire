using UnityEngine;

public partial class UserData : MonoBehaviour
{
    public const string CurrentCashKey = "CurrentCash";
    public const string InventoryKey = "Inventory";
    public const string SakuraKey = "Sakura";
    public const string SakuraLevelKey = "SakuraLevel";
    public const string SoundEnableKey = "SoundEnable";
    public const string MusicEnableKey = "MusicEnable";
    public const string VibrateEnableKey = "VibrateEnable";

    public static void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }
    public static int GetInt(string key, int defaultValue = 0) { 
        return PlayerPrefs.GetInt(key, defaultValue);
    }
    public static void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }
    public static string GetString(string key) { 
        return PlayerPrefs.GetString(key);
    }
    public static void SetBool(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
    }
    public static bool GetBool(string key) { 
        return GetInt(key, 1) == 1;
    }
}
