using IPS;
using UnityEngine;

public class RefillByAmount : ARefillable
{
    [SerializeField] private int fillAmountPerObject;
    [SerializeField] private int remainAmount;
    [SerializeField] protected RefillObjectType type;
    [SerializeField] protected Transform fillTarget;
    [SerializeField] protected Transform refillTarget;
    [SerializeField] GameObject needRefillIcon;
    [SerializeField] GameObject refillAreaIcon;
    [SerializeField] private CapsuleCollider refillCollider;

    public int RemainAmount
    {
        get
        {
            return remainAmount;
        }
        set
        {
            remainAmount = value;
            CheckNeedRefill();
        }
    }
    private Sauna parent;
    public Transform FillTarget => fillTarget ?? transform;
    public override bool NeedRefill => remainAmount == 0;
    public bool CanUse => RemainAmount > 0;

    public RefillObjectType NeedType => type;

    protected override void Start()
    {
        base.Start();
        CheckNeedRefill();
        parent = transform.parent.GetComponent<Sauna>();
    }
    public override void OnCompleted()
    {
        HideNeedRefill();
        EnableInteract();
    }
    public void Fill()
    {
        Filled();
        RemainAmount = fillAmountPerObject;
        if (parent != null) parent.SetAmount();
        refillCollider.enabled = false;
    }
    public override void OnInteract(IInteractable other)
    {
        if (other is Customer)
        {
            remainAmount--;
            parent.SetAmount();
            Invoke(nameof(CheckNeedRefill), 5);
        }
    }
    public void CheckNeedRefill()
    {
        if (remainAmount == 0)
        {
            SetNeedRefill();
            CallNeedRefill();
            ShowNeedRefill();
        }
        else HideNeedRefill();
    }
    private void ShowNeedRefill()
    {
        needRefillIcon.SetActive(true);
        refillAreaIcon.SetActive(true);
        refillCollider.enabled = true;
    }
    private void HideNeedRefill()
    {
        needRefillIcon.SetActive(false);
        refillAreaIcon.SetActive(false);
        refillCollider.enabled = false;
    }
    public void CallNeedRefill()
    {
        this.Dispatch(new EventDefine.NeedRefillEvent { needRefill = this, pos = refillTarget.position, type = type });
    }
}
