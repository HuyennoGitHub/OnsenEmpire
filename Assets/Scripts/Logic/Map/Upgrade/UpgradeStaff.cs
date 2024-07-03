using UnityEngine;
using TMPro;
using IPS;

public class UpgradeStaff : AUpgrade
{
    public StaffType staffType;
    public int nextLv;
    [SerializeField] TextMeshProUGUI lvText;

    private string idStaff;

    protected override void Start()
    {
        base.Start();
        int cost;
        if (staffType == StaffType.Cleaner)
        {
            idStaff = upgradeCtrl.GetCleaner().Info.Id;
        }
        else if (staffType == StaffType.Loader)
        {
            idStaff = upgradeCtrl.GetLoaderToUpgrade().Info.Id;
        }
        cost = GameData.Instance.GetObject(idStaff, nextLv).UnlockCost;
        lvText.text = $"Lv.{nextLv}";
        SetUpgradeInfo(type, cost);
    }
    protected override void OnEnable()
    {
        if (!CheckToShow()) gameObject.SetActive(false);
    }
    public bool CheckPrerequisite()
    {
        if (mapCtrl == null) mapCtrl = FindObjectOfType<MapCtrl>();
        if (staffType == StaffType.Cleaner)
        {
            int lv = upgradeCtrl.GetCleaner().Info.Level;
            if (lv != nextLv - 1)
            {
                if (lv > nextLv - 1)
                {
                    Completed = true;
                }
                return false;
            }
        }
        else if (staffType == StaffType.Loader)
        {
            if (upgradeCtrl.GetLoaderToUpgrade() == null) return false;
            int lv = upgradeCtrl.GetLoaderToUpgrade().Info.Level;
            if (lv != nextLv - 1)
            {
                if (lv > nextLv - 1)
                {
                    Completed = true;
                }
                return false;
            }
        }
        return true;
    }
    public override void OnCompleted()
    {
        UserData.SetCash(UserData.CurrentCash - InitMoney);
        if (staffType == StaffType.Cleaner)
        {
            upgradeCtrl.UpgradeCleaner();
        }
        else if (staffType == StaffType.Loader)
        {
            upgradeCtrl.UpgradeLoader();
        }
        this.Dispatch<EventDefine.OnUpgradeDone>();
        gameObject.SetActive(false);

    }

    public override bool CheckToShow()
    {
        return CheckPrerequisite();
    }
}
