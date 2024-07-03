using IPS;

public partial class Area
{
    public void Unlock()
    {
        GameData.Instance.GetObject(Info.Id, 1).Unlock();
        Info.SetLevel(1);
    }
    public void ExpandSauna()
    {
        //mapCtrl.tutCam.MoveToToiletView();
        mapCtrl.AddServableService(Service.Sauna);
        sauna.Unlock();
    }
    public void ExpandRoom(Room newroom)
    {
        newroom.gameObject.SetActive(true);
        rooms = GetComponentsInChildren<Room>();
        newroom.UnlockLevel();
        newroom.Info.SetLevel(1);
        this.Dispatch(new EventDefine.CanUseEvent { room = newroom });
    }
    public void HireCleaner()
    {
        cleaner.gameObject.SetActive(true);
        cleaner.Upgrade();
        foreach (Room room in rooms)
        {
            cleaner.AddMission(room);
        }
    }
    public void UpgradeCleaner()
    {
        cleaner.Upgrade();
    }
    public void UpgradeRoom(Room room, uint optionVisual)
    {
        room.Upgrade(optionVisual);
    }
}
