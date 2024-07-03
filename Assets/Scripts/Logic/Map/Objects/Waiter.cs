using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waiter : Bot,IInteractable
{
    [SerializeField] RefillCollector myRefillCollector;
    [SerializeField] float collectTime;

    public List<FoodOrder> missions = new List<FoodOrder>();
    public Transform vegetableStore;
    public Transform meatStore;

    public Transform relaxPos;
    public ObjectData Info;
    public GameObject appearVFX;
    public Transform lookAtTransform;
    public Restaurant restaurant;
    public Recycle recycle;
    public BoxCollider boxCollider;
    public BoxCollider BoxCollider
    {
        get
        {
            if (boxCollider != null) return boxCollider;
            else
            {
                boxCollider = GetComponent<BoxCollider>();
                return boxCollider;
            }
        }
    }
    public int MaxTarget => myRefillCollector.MaxTarget;
    private List<Vector3> collectPoss = new List<Vector3>();
    private int index = -1;
    protected override void Start()
    {
        base.Start();
        ShowVFX(true);

        Vector3 tmp = relaxPos.position;
        tmp.y = transform.position.y;
        relaxPos.position = tmp;

        destination = relaxPos.position;
    }
    private void ShowVFX(bool show = false)
    {
        appearVFX.SetActive(show);
    }
    private void LateUpdate()
    {
        if (CalculateDistance(destination, transform.position) < 0.01f)
        {
            EnableInteract();
            if (myRefillCollector.CollectedTotal == 0)
            {
                animator.Play("Idle");
            }
            else
            {
                animator.Play("Grab");
            }
            if (CalculateDistance(destination, relaxPos.position) < 0.01f)
            {
                transform.LookAt(lookAtTransform);
            }
        }
        else
        {
            DisableInteract();
            if (myRefillCollector.CollectedTotal == 0)
            {
                animator.Play("Walking");
            }
            else
            {
                animator.Play("GoGrab");
            }
        }
    }
    public void StartMission()
    {
        collectPoss.Clear();
        foreach (var mission in missions)
        {
            if (mission.foodType == RefillObjectType.Vegetable)
            {
                collectPoss.Add(vegetableStore.position);
            }
            else if (mission.foodType == RefillObjectType.Meat)
            {
                collectPoss.Add(meatStore.position);
            }
        }
        index = 0;
        StartCoroutine(RunMissionCollect());
    }
    IEnumerator RunMissionCollect()
    {
        while (myRefillCollector.CollectedTotal < missions.Count)
        {
            MoveTo(collectPoss[index]);
            if (myRefillCollector.CollectedTotal > index)
            {
                index++;
            }
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(RunMissionRefill());
    }
    IEnumerator RunMissionRefill()
    {
        index = -1;
        animator.Play("GoGrab");
        while (missions.Count > 0 && myRefillCollector.CollectedTotal > 0)
        {
            MoveTo(missions[0].table.RefillPos);
            if (!missions[0].table.NeedRefill)
            {
                missions.RemoveAt(0);
                CompleteMission();
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        if (myRefillCollector.CollectedTotal == 0 || missions.Count == 0)
        {
            CompleteMission();
        }
    }
    public void CompleteMission()
    {
        if (missions.Count == 0)
        {
            destination = relaxPos.position;
            if (myRefillCollector.CollectedTotal == 0)
            {
                animator.Play("Walking");
            }
            else
            {
                animator.Play("GoGrab");
            }

            StartCoroutine(restaurant.AssignToWaiter(this));
        }
        else if (myRefillCollector.CollectedTotal == 0)
        {
            StartMission();
        }
    }
    public void Hired()
    {
        Info.SetLevel(1);
        GameData.Instance.GetObject(Info.Id, Info.Level).Unlock();
        UserData.Inventory.SaveItem(new ObjectData(Info.Id, ObjectType.Waiter, Info.Level), true);
        if (missions.Count == 0 && myRefillCollector.CollectedTotal == 0)
        {
            StartCoroutine(restaurant.AssignToWaiter(this));
        }
    }
    public bool CheckMissionList(EventDefine.ServedOrder order)
    {
        FoodOrder existOrder;
        existOrder.foodType = RefillObjectType.None;
        existOrder = missions.Find(m => m.table == order.table && m.foodType == order.foodType);
        if (existOrder.foodType == RefillObjectType.None || existOrder.table == null)
        {
            return false;
        }
        else
        {
            missions.Remove(existOrder);
            if (missions.Count == 0)
            {
                CompleteMission();
            }
            return true;
        }
    }

    public void ThrowFood()
    {
        StopCoroutine(RunMissionRefill());
        MoveTo(recycle.RefillPos);
        StartCoroutine(WaitToThrowFood());
    }
    IEnumerator WaitToThrowFood()
    {
        while (myRefillCollector.CollectedTotal > 0)
        {
            yield return new WaitForEndOfFrame();
        }
        StartMission();
    }
    public void AddMoreFood(RefillObjectType foodType)
    {
        if (foodType == RefillObjectType.Vegetable)
        {
            MoveTo(vegetableStore.position);
        }
        else
        {
            MoveTo(meatStore.position);
        }
    }

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
    public bool IsInteractEnable => throw new System.NotImplementedException();
}
