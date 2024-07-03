using IPS;

public partial class UserData
{
    public static int CurrentShowingCash;
    public static void SetFakeCash(int cash)
    {
        CurrentShowingCash = cash;
        EventDispatcher.Instance.Dispatch<EventDefine.OnCashChanged>();
    }
    public static void InitShowingCash()
    {
        SetFakeCash(CurrentCash);
    }
}
