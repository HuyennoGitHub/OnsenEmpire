using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sauna : MonoBehaviour
{
    public ObjectData Info;
    public GameObject cashPrefab;
    [SerializeField] int useFee;
    [SerializeField] SortMoneyInContainer sort;
    [SerializeField] private RefillByAmount[] saunaSlots;
    [SerializeField] private SaunaSlot[] slots;
    [SerializeField] private bool lookLeft;
    public Transform center;
    public GameObject[] hideWalls;
    public Transform[] waitingForSaunaPoss;
    List<Customer> waitingCustomers = new();

    public bool LookAtTheLeftSide => lookLeft;
    [ContextMenu("BindObject")]
    private void BindObject()
    {
        if (saunaSlots.Length == 0) saunaSlots = GetComponentsInChildren<RefillByAmount>();
        if (slots.Length == 0) slots = GetComponentsInChildren<SaunaSlot>(true);
    }
    private void OnEnable()
    {
        foreach (var wall in hideWalls)
        {
            wall.SetActive(false);
        }
        StartCoroutine(ManageToilet());
    }
    private void OnTriggerEnter(Collider other)
    {
        var customer = other.GetComponent<Customer>();
        if (customer == null) return;
        if (customer.payedForSauna)
        {
            //customer.ChangeClothes(false);
            return;
        }
        PayToUseToilet();
        customer.payedForSauna = true;
    }
    public void PayToUseToilet()
    {
        var cash = Instantiate(cashPrefab, sort.transform);
        var money = cash.GetComponent<CashObject>();
        money.SetValue(useFee);
        cash.transform.position = waitingForSaunaPoss[0].position;
        sort.Sort(cash, money);
    }
    public void GetAmount()
    {
        var obj = GameData.Instance.GetObjectInSauna(Info.Id);
        for (int i = 0; i < 3; i++)
        {
            saunaSlots[i].RemainAmount = obj.GetAmountByIndex(i);
        }
    }
    public void SetAmount()
    {
        var obj = GameData.Instance.GetObjectInSauna(Info.Id);
        for (int i = 0; i < 3; i++)
        {
            obj.SetRemainAmountInSauna(i, saunaSlots[i].RemainAmount);
        }
        ObjectInSauna saveObj = new(Info.Id, saunaSlots[0].RemainAmount, saunaSlots[1].RemainAmount, saunaSlots[2].RemainAmount);
        UserData.Inventory.SaveRemainAmountInSauna(saveObj, true);
    }
    public void InitAmount()
    {
        foreach (var slot in saunaSlots)
        {
            slot.RemainAmount = 0;
        }
    }
    public bool ProcessCustomerUseSaunaRequest(Customer customer)
    {
        if (waitingCustomers.Contains(customer)) return true;
        if (waitingCustomers.Count == 3)
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
        return waitingForSaunaPoss[waitingCustomers.Count - 1].position;
    }
    IEnumerator ManageToilet()
    {
        while (true)
        {
            while (waitingCustomers.Count == 0) yield return new WaitForEndOfFrame();
            var toilet = System.Array.Find(slots, s => s.CanUse);
            while (toilet == null)
            {
                toilet = System.Array.Find(slots, s => s.CanUse);
                yield return new WaitForEndOfFrame();
            }
            Customer customer = waitingCustomers[0];
            waitingCustomers.RemoveAt(0);
            customer.MoveToSauna(toilet, center);
            customer.isWaiting = false;
            ArrangeWaitingCustomerQueue();
            yield return new WaitForEndOfFrame();
        }
    }
    public void DeleteCustomerFromWaitingQueue(Customer customer)
    {
        waitingCustomers.Remove(customer);
        ArrangeWaitingCustomerQueue();
    }
    public void ArrangeWaitingCustomerQueue()
    {
        for (int i = 0; i < waitingCustomers.Count; i++)
        {
            waitingCustomers[i].MoveTo(waitingForSaunaPoss[i].position);
        }
    }
    public void Unlock(bool save = true)
    {
        gameObject.SetActive(true);
        GameData.Instance.GetObject(Info.Id, 1).Unlock();
        Info.SetLevel(1);
        InitAmount();
        if (save) SaveInventory();
    }
    private void SaveInventory()
    {
        UserData.Inventory.SaveItem(new ObjectData(Info.Id, ObjectType.Sauna, 1), true);
    }
}
