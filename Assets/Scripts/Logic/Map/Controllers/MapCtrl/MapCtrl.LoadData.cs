using UnityEngine;

public partial class MapCtrl
{
    [SerializeField] private Area[] areas;
    [SerializeField] private Restaurant restaurant;
    [SerializeField] private Storage storage;
    [SerializeField] private Reception reception;
    [SerializeField] private UpgradeCtrl upgradeCtrl;
    [SerializeField] private BoosterCtrl boosterCtrl;
    private void Start()
    {
        GameData.Instance.GetDataRemainAmountInSauna();
        var inventory = UserData.Inventory;
        foreach (var item in inventory.unlocked)
        {
            switch (item.Type)
            {
                case ObjectType.Area:
                    foreach (var area in areas)
                    {
                        if (area.Info.Id.Equals(item.Id))
                        {
                            area.gameObject.SetActive(true);
                            area.Unlock();
                        }
                    }
                    break;
                case ObjectType.Restaurant:
                    restaurant.gameObject.SetActive(true);
                    restaurant.Unlock();
                    break;
                case ObjectType.Storage:
                    storage.gameObject.SetActive(true);
                    storage.Unlock();
                    break;
                default:
                    break;
            }
        }
        reception.gameObject.SetActive(true);
        if (restaurant.gameObject.activeInHierarchy)
        {
            AddServableService(Service.Restaurant);
        }
        upgradeCtrl.enabled = true;
        boosterCtrl.gameObject.SetActive(true);
    }
}
