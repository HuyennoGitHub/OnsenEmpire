using IPS;
using UnityEngine;

public enum RoomType
{
    Onsen,
    Sauna
}
public class ExpandRoom : AUpgrade
{
    public GameObject room;
    public RoomType roomType;
    public Area area;

    protected override void Start()
    {
        base.Start();
        string id = "";
        int cost;
        switch (roomType)
        {
            case RoomType.Onsen:
                id = room.GetComponent<Room>().Info.Id;
                break;
            case RoomType.Sauna:
                id = room.GetComponent<Sauna>().Info.Id;
                break;
        }
        cost = GameData.Instance.GetObject(id, 1).UnlockCost;
        SetUpgradeInfo(type, cost);
    }
    protected override void OnEnable()
    {
        if (!CheckToShow()) gameObject.SetActive(false);
    }

    public override void OnCompleted()
    {
        switch (roomType)
        {
            case RoomType.Onsen:
                upgradeCtrl.ExpandRoom(room.GetComponent<Room>());
                upgradeCtrl.ShowNextUpgradeRoomBox();
                this.Dispatch(new EventDefine.RewardSakura { sakura = 2 });
                break;
            case RoomType.Sauna:
                upgradeCtrl.ExpandToilet();
                this.Dispatch(new EventDefine.RewardSakura { sakura = 4 });
                break;
        }
        UserData.SetCash(UserData.CurrentCash - InitMoney);
        this.Dispatch<EventDefine.OnUpgradeDone>();
        gameObject.SetActive(false);
    }

    public override bool CheckToShow()
    {
        if (room.activeInHierarchy) return false;
        else return true;
    }
}
