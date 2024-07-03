public partial class UserData
{
    public static bool SoundEnable
    {
        get
        {
            return GetBool(SoundEnableKey);
        }
        set
        {
            SetBool(SoundEnableKey, value);
        }
    }
    public static bool MusicEnable
    {
        get
        {
            return GetBool(MusicEnableKey);
        }
        set
        {
            SetBool(MusicEnableKey, value);
        }
    }
    public static bool VibrateEnable
    {
        get
        {
            return GetBool(VibrateEnableKey);
        }
        set
        {
            SetBool(VibrateEnableKey, value);
        }
    }
}
