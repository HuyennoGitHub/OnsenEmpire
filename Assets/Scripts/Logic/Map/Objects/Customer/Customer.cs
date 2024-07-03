using DG.Tweening;
using IPS;
using UnityEngine;

public partial class Customer : Bot, IInteractable
{
    [SerializeField] Material[] skinMats;
    [SerializeField] GameObject[] skinLayers;
    [SerializeField] Transform model;
    [SerializeField] Transform hand;

    public CustomerManager manager;
    public Vector3 Pos;
    public GameObject cashPrefab;
    public bool isMoving;
    public bool isWaiting;
    public Service usingService;
    private ActionDoing actionDoing;

    public enum ActionDoing
    {
        Waiting,
        GoingToChangeClothes,
        GoingToOnsen,
        GoingToSauna,
        GoingToMeal,
        CallingOrder,
        UsingService,
        GoingHome
    }
    public bool IsServed => isSpecialOrderServed;
    //public uint Region => usingRoom.Region;
    public ActionDoing Doing { get { return actionDoing; } set { actionDoing = value; } }
    public bool IsInteractEnable => throw new System.NotImplementedException();
    protected override void Start()
    {
        servedCollider.enabled = false;
        payedForSauna = false;
        usingService = Service.None;
        int rand = Random.Range(0, skinMats.Length);
        foreach (var layer in skinLayers)
        {
            layer.GetComponent<SkinnedMeshRenderer>().sharedMaterial = skinMats[rand];
        }
        isMoving = false;
        isWaiting = true;
        actionDoing = ActionDoing.Waiting;
    }
    protected virtual void LateUpdate()
    {
        if (isMoving && isWaiting)
        {
            if (CalculateDistance(transform.position, destination) < 0.01f)
            {
                animator.Play("Idle");
                isMoving = false;
            }
            else
            {
                animator.Play("Walking");
            }
        }
    }
    public override void MoveTo(Vector3 target)
    {
        base.MoveTo(target);
        isMoving = true;
        animator.Play("Walking");
    }
    public virtual void DropCash(int cashValue)
    {
        var cash = Instantiate(cashPrefab, transform.parent);
        var money = cash.GetComponent<CashObject>();
        money.SetValue(cashValue);
        cash.transform.position = transform.position;
        Vector3 randomPos = usingRoom.center.position + Random.onUnitSphere * 3;
        randomPos.y = 0;
        cash.transform.DOMove(randomPos, .5f);
    }

    private bool CheckReachDestination(Vector3 destination)
    {
        if (CalculateDistance(transform.position, destination) < 0.01f) return true;
        return false;
    }
    public void GoHome()
    {
        usingRoom.Using = false;
        MoveTo(new Vector3(-7, 0, -50));
        isMoving = true;
        actionDoing = ActionDoing.GoingHome;
        Invoke(nameof(DestroyItself), 60);
    }
    public virtual void DestroyItself()
    {
        Destroy(gameObject);
    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other == null) return;
        if (other.CompareTag("Gate"))
        {
            isMoving = true;
            animator.Play("Walking");
        }
        else if (other.CompareTag("Finish"))
        {
            if (Doing == ActionDoing.GoingHome)
            {
                ChangeClothes(true);
            }
        }


        var player = other.GetComponent<PlayerInputCtrl>();
        if (player != null)
        {
            if (!haveSpecialOrder) return;
            if (isSpecialOrderServed) return;
            var collector = other.GetComponent<RefillCollector>();
            if (!collector.FindObjectByTypeInCollecteds(orderType)) return;
            var ordered = collector.GetObjectFromCollectedsByType(orderType);
            ordered.BeFill(hand);
            isSpecialOrderServed = true;
            this.Dispatch<EventDefine.DoneSakeOrder>();
            DropCash(usingRoom.RoomOrder);
        }
        var obj = other.GetComponent<Usable>();
        if (obj && other is BoxCollider && actionDoing == ActionDoing.GoingToOnsen)
        {
            other.enabled = false;
            animator.Play("StandSit");
            isMoving = false;
            Invoke(nameof(MoveOutOfRoom), 5f);
            StartCoroutine(WaitForReachingOnsen());
            return;
        }
        var obj2 = other.GetComponent<SaunaSlot>();
        if (obj2 != usingSaunaSlot) return;
        var obj1 = other.GetComponent<RefillByAmount>();
        if (obj1)
        {
            if (other.GetInstanceID() != obj1.boxCollider.GetInstanceID()) return;
            obj1.OnInteract(this);
            return;
        }
    }
    protected virtual void OnTriggerExit(Collider other)
    {
        if (other == null) return;
        
    }
    public void ChangeClothes(bool defaultClothes = false)
    {
        skinLayers[0].SetActive(defaultClothes);
        skinLayers[1].SetActive(!defaultClothes);
    }
    private void FindNextService()
    {
        manager.FindNextService(this);
    }
    public void OnInteract(IInteractable other)
    {

    }

    public void EnableInteract()
    {
    }

    public void DisableInteract()
    {
    }
}
