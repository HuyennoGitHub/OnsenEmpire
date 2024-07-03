using UnityEngine;
using TMPro;
using IPS;

public class ShokuTable : ARefillable
{
    [SerializeField] Transform usePos;
    [SerializeField] Transform fillTarget;
    [SerializeField] CapsuleCollider capsuleCollider;
    [SerializeField] ShokuOrder[] customerOrder;
    public SortMoneyInContainer moneyContainer;
    public GameObject orderCanvas;
    public GameObject vegetableOrder;
    public GameObject meatOrder;
    public TextMeshProUGUI vegetableAmountTxt;
    public TextMeshProUGUI meatAmountTxt;
    private bool needFood;
    public Customer usedCustomer;

    public Transform FillTarget => fillTarget ?? transform;
    public Transform UsePos => usePos ?? transform;
    public bool Using { get; set; } = false;
    public override bool NeedRefill
    {
        get
        {
            needFood = customerOrder[0].amount > 0 || customerOrder[1].amount > 0;
            return needFood;
        }
    }

    public RefillObjectType NeedObjectType
    {
        get
        {
            if (customerOrder[0].amount > 0 && customerOrder[1].amount > 0)
            {
                return RefillObjectType.FoodBoth;
            }
            if (customerOrder[0].amount > 0)
            {
                return RefillObjectType.Vegetable;
            }
            if (customerOrder[1].amount > 0)
            {
                return RefillObjectType.Meat;
            }
            return RefillObjectType.None;
        }
    }
    protected override void Start()
    {
        base.Start();
        //UpdateOrderVisual();
    }
    public override void OnCompleted()
    {

    }
    public void Fill(RefillObjectType refillType, Waiter waiter)
    {
        if (refillType == RefillObjectType.Vegetable)
        {
            customerOrder[0].amount--;
        }
        else if (refillType == RefillObjectType.Meat)
        {
            customerOrder[1].amount--;
        }
        this.Dispatch(new EventDefine.ServedOrder { table = this, foodType = refillType, waiter = waiter });
        DisableInteract();
        UpdateOrderVisual();
    }
    public override void OnInteract(IInteractable other)
    {

    }
    public void AddOrder(int vegetableAmount, int meatAmount)
    {
        customerOrder[0].amount = vegetableAmount;
        customerOrder[1].amount = meatAmount;
        UpdateOrderVisual();
        this.Dispatch(new EventDefine.HaveOrder { table = this, vegetable = vegetableAmount, meat = meatAmount });
    }
    public void UpdateOrderVisual()
    {
        if (!NeedRefill)
        {
            orderCanvas.SetActive(false);
            Invoke(nameof(SetTableFree), 5f);
        }
        else
        {
            orderCanvas.SetActive(true);
            EnableInteract();
        }
        if (customerOrder[0].amount == 0)
        {
            vegetableOrder.SetActive(false);
        }
        else
        {
            vegetableOrder.SetActive(true);
            vegetableAmountTxt.text = $"x{customerOrder[0].amount}";
        }
        if (customerOrder[1].amount == 0)
        {
            meatOrder.SetActive(false);
        }
        else
        {
            meatOrder.SetActive(true);
            meatAmountTxt.text = $"x{customerOrder[1].amount}";
        }
    }
    public override void EnableInteract()
    {
        capsuleCollider.enabled = true;
    }
    public override void DisableInteract()
    {
        //capsuleCollider.enabled = false;
    }
    private void SetTableFree()
    {
        Using = false;
    }
}
[System.Serializable]
public struct ShokuOrder
{
    public RefillObjectType type;
    public int amount;

    public void SetInfo(RefillObjectType type, int amount)
    {
        this.type = type;
        this.amount = amount;
    }
}
