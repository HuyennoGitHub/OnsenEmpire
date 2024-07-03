using IPS;
using UnityEngine;

public class HireStaff : AUpgrade
{
    public GameObject staff;
    public StaffType staffType;
    protected override void Start()
    {
        base.Start();
        string id = "";
        int cost;
        if (staffType == StaffType.Cleaner)
        {
            id = staff.GetComponent<Cleaner>().Info.Id;
        }
        else if (staffType == StaffType.Loader)
        {
            id = staff.GetComponent<Loader>().Info.Id;
        }
        else if (staffType == StaffType.Receptionist)
        {
            id = staff.GetComponent<Receptionist>().Info.Id;
        }
        else if (staffType == StaffType.Waiter)
        {
            id = staff.GetComponent<Waiter>().Info.Id;
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
        staff.SetActive(true);
        if (staffType == StaffType.Receptionist)
        {
            upgradeCtrl.HireReceptionist();
        }
        else if (staffType == StaffType.Cleaner)
        {
            upgradeCtrl.HireCleaner();
        }
        else if (staffType == StaffType.Loader)
        {
            upgradeCtrl.HireMoreLoader();
        }
        else if (staffType == StaffType.Waiter)
        {
            upgradeCtrl.HireWaiter();
        }
        UserData.SetCash(UserData.CurrentCash - InitMoney);
        this.Dispatch<EventDefine.OnUpgradeDone>();
        gameObject.SetActive(false);
    }

    public override bool CheckToShow()
    {
        if (staff.activeInHierarchy) return false;
        else return true;
    }
}
public enum StaffType
{
    Cleaner,
    Loader,
    Receptionist,
    Waiter
}
