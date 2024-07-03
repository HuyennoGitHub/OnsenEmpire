using IPS;
using UnityEngine;

public partial class Area : MonoBehaviour
{
    private MapCtrl mapCtrl;

    [SerializeField] private Room[] rooms;
    [SerializeField] private Sauna sauna;
    [SerializeField] private Cleaner cleaner;
    [SerializeField] private UpgradeCtrl upgradeCtrl;
    [SerializeField] uint region;

    public ObjectData Info;
    public GameObject[] hideWalls;

    public uint Region => region;

    private void Start()
    {
        mapCtrl = FindObjectOfType<MapCtrl>();
        this.AddListener<EventDefine.OnStartGame>(OnStartGame);
        this.AddListener<EventDefine.NeedCleanEvent>(CleanResponse);
        foreach (var wall in hideWalls)
        {
            wall.SetActive(false);
        }
        var inventory = UserData.Inventory;
        foreach (var item in inventory.unlocked)
        {
            switch (item.Type)
            {
                case ObjectType.Room:
                    foreach (var room in rooms)
                    {
                        if (room.Info.Id.Equals(item.Id))
                        {
                            room.gameObject.SetActive(true);
                            room.UnlockLevel(item.Level, item.Visual, false);
                            break;
                        }
                    }
                    break;
                case ObjectType.Cleaner:
                    if (cleaner.Info.Id.Equals(item.Id))
                    {
                        cleaner.UnlockLevel(item.Level, item.Visual);
                    }
                    break;
                case ObjectType.Sauna:
                    if (sauna.Info.Id.Equals(item.Id))
                    {
                        sauna.Unlock(false);
                        sauna.GetAmount();
                        mapCtrl.AddServableService(Service.Sauna);
                    }
                    break;
                default:
                    break;
            }
        }
        rooms = GetComponentsInChildren<Room>();
        upgradeCtrl.enabled = true;
    }
    private void OnStartGame()
    {
        CheckRoom();
    }
    private void CheckRoom()
    {
        foreach (var room in rooms)
        {
            room.CheckCanUseRoom();
        }
    }
    public void CleanResponse(EventDefine.NeedCleanEvent param)
    {
        if (!param.areaId.Equals(Info.Id)) return;
        cleaner.AddMission(param.needCleanRoom);
    }
    public bool CallCustomerToSauna(Customer customer)
    {
        if (!sauna.gameObject.activeInHierarchy)
        {
            return false;
        }
        else
        {
            return sauna.ProcessCustomerUseSaunaRequest(customer);
        }
    }
    public Cleaner GetCleaner()
    {
        return cleaner;
    }
    public Vector3 GetWaitingSauanSlot(Customer customer)
    {
        return sauna.GetWaitingSlot(customer);
    }
    public Transform GetLookAtSaunaTarget()
    {
        return sauna.center;
    }
}
