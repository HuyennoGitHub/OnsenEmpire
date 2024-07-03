using IPS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Restaurant : MonoBehaviour
{
    public ObjectData Info;
    public GameObject[] hideWalls;
    public List<Transform> waitingPoss;
    public Transform lookAtTarget;
    public UpgradeCtrl upgradeCtrl;

    [SerializeField] uint mealFee;
    [SerializeField] private List<ShokuTable> tables;
    [SerializeField] private Waiter[] waiters;

    List<Customer> waitingCustomers = new();
    List<FoodOrder> orders = new();
    int availableWaitingPosAmount;
    public int WaitingPosAmount
    {
        get { return availableWaitingPosAmount; }
        set
        {
            if (value < availableWaitingPosAmount) return;
            availableWaitingPosAmount = value;
        }
    }

    private void Awake()
    {
        this.AddListener<EventDefine.OnStartGame>(OnStartGame);
    }
    private void Start()
    {
        waiters = FindObjectsOfType<Waiter>(true);
        var inventory = UserData.Inventory;
        foreach (var item in inventory.unlocked)
        {
            foreach (var waiter in waiters)
            {
                if (waiter.Info.Id.Equals(item.Id))
                {
                    waiter.gameObject.SetActive(true);
                }
            }
        }
        waiters = FindObjectsOfType<Waiter>();
    }
    private void OnStartGame()
    {
        foreach (var w in waiters)
        {
            StartCoroutine(AssignToWaiter(w));
        }
    }
    private void OnEnable()
    {
        this.AddListener<EventDefine.HaveOrder>(AddOrders);
        this.AddListener<EventDefine.ServedOrder>(DeleteOrderFromList);
        foreach (var wall in hideWalls)
        {
            wall.SetActive(false);
        }
        upgradeCtrl.enabled = true;
        WaitingPosAmount = 3;
        StartCoroutine(ManageRestaurant());
    }
    IEnumerator ManageRestaurant()
    {
        while (true)
        {
            while (waitingCustomers.Count == 0) yield return new WaitForEndOfFrame();
            var table = tables.Find(t => !t.Using);
            while (table == null)
            {
                table = tables.Find(t => !t.Using);
                yield return new WaitForEndOfFrame();
            }
            table.Using = true;
            tables.Remove(table);
            tables.Add(table);
            Customer customer = waitingCustomers[0];
            waitingCustomers.RemoveAt(0);
            customer.MoveToShokuJiSlot(table);
            customer.isWaiting = false;
            Transform pos = waitingPoss[0];
            waitingPoss.Remove(pos);
            waitingPoss.Add(pos);
            yield return new WaitForEndOfFrame();
        }
    }
    public void DeleteCustomerFromWaitingQueue(Customer customer)
    {
        for (int i = 0; i < waitingCustomers.Count; i++)
        {
            if (waitingCustomers[i] == customer)
            {
                Transform pos = waitingPoss[i];
                waitingPoss.Remove(pos);
                waitingPoss.Add(pos);
                break;
            }
        }
        waitingCustomers.Remove(customer);
    }
    public bool ProcessCustomerHaveMealRequest(Customer customer)
    {
        if (waitingCustomers.Contains(customer)) return true;
        if (waitingCustomers.Count == availableWaitingPosAmount || !gameObject.activeInHierarchy)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public Vector3 GetWaitingSlot(Customer customer)
    {
        waitingCustomers.Add(customer);
        return waitingPoss[waitingCustomers.Count - 1].position;
    }
    public void Unlock()
    {
        GameData.Instance.GetObject(Info.Id, 1).Unlock();
        Info.SetLevel(1);
    }
    public IEnumerator AssignToWaiter(Waiter waiter)
    {
        while (orders.Count == 0)
        {
            yield return new WaitForEndOfFrame();
        }
        while (waiter.MaxTarget > waiter.missions.Count && orders.Count > 0)
        {
            waiter.missions.Add(orders[0]);
            orders.RemoveAt(0);
        }
        waiter.StartMission();
    }
    private void AddOrders(EventDefine.HaveOrder param)
    {
        for (int i = 0; i < param.vegetable; i++)
        {
            FoodOrder order = new()
            {
                foodType = RefillObjectType.Vegetable,
                table = param.table
            };
            orders.Add(order);
        }
        for (int i = 0; i < param.meat; i++)
        {
            FoodOrder order = new()
            {
                foodType = RefillObjectType.Meat,
                table = param.table
            };
            orders.Add(order);
        }
    }
    public void DeleteOrderFromList(EventDefine.ServedOrder order)
    {
        if (order.waiter == null)
        {
            FoodOrder existOrder;
            existOrder = orders.Find(i => i.table == order.table && i.foodType == order.foodType);
            if (existOrder.foodType == RefillObjectType.Vegetable || existOrder.foodType == RefillObjectType.Meat)
            {
                orders.Remove(existOrder);
            }
            else
            {
                if (!waiters[0].CheckMissionList(order))
                {
                    waiters[1].CheckMissionList(order);
                }
            }
        }
        else
        {
            order.waiter.CheckMissionList(order);
        }
    }
    public void HireWaiter()
    {
        waiters = GetComponentsInChildren<Waiter>();
        foreach (Waiter waiter in waiters)
        {
            waiter.Hired();
            StartCoroutine(AssignToWaiter(waiter));
        }
    }
}
public struct FoodOrder
{
    public RefillObjectType foodType;
    public ShokuTable table;
}
