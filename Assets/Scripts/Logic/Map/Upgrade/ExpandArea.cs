using IPS;
using UnityEngine;

public class ExpandArea : AUpgrade
{
    public GameObject lockCanvas;
    public GameObject expandArea;
    public AreaType areaType;

    public enum AreaType { RoomArea, Restaurant}
    protected override void Start()
    {
        base.Start();
        string id = "";
        int cost;
        switch (areaType)
        {
            case AreaType.RoomArea:
                id = expandArea.transform.GetComponent<Area>().Info.Id;
                break;
            case AreaType.Restaurant:
                id = expandArea.transform.GetComponent<Restaurant>().Info.Id;
                break;
        }
        cost = GameData.Instance.GetObject(id, 1).UnlockCost;
        SetUpgradeInfo(type, cost);
    }
    protected override void OnEnable()
    {
        if (lockCanvas != null) lockCanvas.SetActive(false);
        if (!CheckToShow()) gameObject.SetActive(false);
    }
    public override void OnCompleted()
    {
        gameObject.SetActive(false);
        MapCtrl mapCtrl = FindObjectOfType<MapCtrl>();
        expandArea.SetActive(true);
        switch (areaType)
        {
            case AreaType.RoomArea:
                Area area = expandArea.GetComponent<Area>();
                mapCtrl.ExpandArea(area);
                UserData.Inventory.SaveItem(new ObjectData(area.Info.Id, ObjectType.Area, 1), true);
                this.Dispatch(new EventDefine.OnExpandedArea { areaId = area.Info.Id });
                break;
            case AreaType.Restaurant:
                Restaurant restaurant = expandArea.GetComponent<Restaurant>();
                mapCtrl.ExpandRestaurant();
                restaurant.Unlock();
                UserData.Inventory.SaveItem(new ObjectData(restaurant.Info.Id, ObjectType.Restaurant, 1), true);
                break;
        }
        UserData.SetCash(UserData.CurrentCash - InitMoney);
        this.Dispatch<EventDefine.OnUpgradeDone>();
    }
    public override bool CheckToShow()
    {
        if (expandArea.activeInHierarchy) return false;
        else return true;
    }
}
