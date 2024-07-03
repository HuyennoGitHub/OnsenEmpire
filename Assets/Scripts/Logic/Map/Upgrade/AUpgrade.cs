using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum UpgradeType
{
    ExpandArea,
    ExpandRoom,
    UpgradeRoom,
    HireStaff,
    UpgradeStaff
}
[RequireComponent(typeof(BoxCollider))]
public abstract class AUpgrade : MonoBehaviour, IInteractable
{
    protected BoxCollider boxCollider;
    public UpgradeType type;
    int upgradeCost;
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI remainMoneyText;
    //private int nextLevel;
    protected MapCtrl mapCtrl;
    [SerializeField] protected UpgradeCtrl upgradeCtrl;
    [SerializeField] protected ConditionAllowUnlocking[] conditions;

    public bool Completed { get; set; } = false;
    public bool IsPaying { get; private set; } = false;
    private int remainMoney;
    public int MoneyRemaining => remainMoney;
    public int InitMoney => upgradeCost;

    protected BoxCollider BoxCollider
    {
        get
        {
            if (boxCollider == null) boxCollider = GetComponent<BoxCollider>();
            return boxCollider;
        }
    }

    public bool IsInteractEnable => BoxCollider.enabled;
    public const string UpgradeableTag = "Upgradeable";

    protected virtual void Start()
    {
        gameObject.tag = UpgradeableTag;
        mapCtrl = FindObjectOfType<MapCtrl>();
        //upgradeCtrl = transform.parent.parent.GetComponent<UpgradeCtrl>();
        //SetUpgradeInfo(type, upgradeCost, 1);
    }
    protected virtual void OnEnable()
    {

    }
    public virtual void SetUpgradeInfo(UpgradeType type, int cost)
    {
        this.type = type;
        upgradeCost = cost;
        //this.nextLevel = nextLevel;
        Completed = false;
        IsPaying = false;
        remainMoney = upgradeCost;
        UpdateRemainMoneyText(remainMoney);
    }
    private void UpdateRemainMoneyText(int money)
    {
        if (money < 1000)
        {
            remainMoneyText.text = money.ToString();
        }
        else
        {
            double real = Math.Round((double)money / 1000, 1);
            remainMoneyText.text = real.ToString() + "K";
        }
    }
    public virtual void OnCompleted()
    {
        DisableInteract();
        UserData.SetCash(UserData.CurrentCash - upgradeCost);
        //this.Dispatch(new CallShowBoosterOffer { type = BoosterType.Money });
        GameData.Instance.SaveInventory();
    }
    public virtual void StartUpgrade()
    {
        if (IsPaying) return;
        remainMoney = upgradeCost;
        IsPaying = true;
    }
    public virtual void OnUpgrading(int step)
    {
        if (!IsPaying || Completed) return;
        remainMoney -= step;
        if (remainMoney <= 0 && !Completed)
        {
            remainMoney = 0;
            OnCompleted();
        }
        UpdateRemainMoneyText(remainMoney);
    }
    public virtual void EndUpgrade()
    {

    }
    public bool CheckCondition()
    {
        for (int i = 0; i < conditions.Length; i++)
        {
            if (conditions[i].Id.Equals("sakura"))
            {
                if (UserData.CurrentSakuraLevel < conditions[i].Level)
                {
                    return false;
                }
            }
            else if (!GameData.Instance.GetObject(conditions[i].Id, conditions[i].Level).Unlocked)
            {
                return false;
            }
        }
        return true;
    }
    public abstract bool CheckToShow();
    public void DisableInteract()
    {
        BoxCollider.enabled = false;
    }

    public void EnableInteract()
    {
        BoxCollider.enabled = true;
    }

    public void OnInteract(IInteractable other)
    {
    }
}

[System.Serializable]
public struct ConditionAllowUnlocking
{
    [SerializeField] private string id;
    [SerializeField] private int level;

    public string Id => id;
    public int Level => level;
}
