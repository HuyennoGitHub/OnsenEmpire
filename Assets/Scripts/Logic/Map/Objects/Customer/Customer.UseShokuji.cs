using System.Collections;
using UnityEngine;

public partial class Customer
{
    private ShokuOrder[] shokuOrders = new ShokuOrder[2];
    public ShokuTable usingTable;

    public void GoToRestaurant(Vector3 waitingSlot, Transform lookAtTarget)
    {
        MoveTo(waitingSlot);
        isWaiting = true;
        usingService = Service.Restaurant;
        WaitTable(lookAtTarget);
    }
    private void RandomOrder()
    {
        int maxItemAmount = Random.Range(1, 5);
        int vegetableAmount = Random.Range(1, maxItemAmount + 1);
        int meatAmount = maxItemAmount - vegetableAmount;
        shokuOrders[0].SetInfo(RefillObjectType.Vegetable, vegetableAmount);
        shokuOrders[1].SetInfo(RefillObjectType.Meat, meatAmount);
    }
    public void ShokuJi()
    {
        animator.Play("Eat");
    }
    public void PayForMeal()
    {
        var cash = Instantiate(cashPrefab, usingTable.moneyContainer.transform);
        var money = cash.GetComponent<CashObject>();
        money.SetValue(100);
        cash.transform.position = usingTable.UsePos.position;
        usingTable.moneyContainer.Sort(cash, money);
    }
    public void MoveToShokuJiSlot(ShokuTable table)
    {
        isWaiting = false;
        actionDoing = ActionDoing.GoingToMeal;
        usingTable = table;
        table.usedCustomer = this;
        MoveTo(usingTable.UsePos.position);
        StartCoroutine(OnUsingMeal(table.transform));
    }
    public void WaitTable(Transform lookAt)
    {
        StartCoroutine(OnWaitingTable(lookAt));
    }
    IEnumerator OnWaitingTable(Transform lookAt)
    {
        actionDoing = ActionDoing.GoingToMeal;
        while (isWaiting && !CheckReachDestination(destination))
        {
            yield return new WaitForEndOfFrame();
        }
        actionDoing = ActionDoing.Waiting;
        transform.LookAt(lookAt);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        yield return new WaitForEndOfFrame();
    }
    IEnumerator OnUsingMeal(Transform lookAt)
    {
        while (!CheckReachDestination(usingTable.UsePos.position))
        {
            yield return new WaitForEndOfFrame();
        }
        actionDoing = ActionDoing.UsingService;
        isMoving = false;
        animator.Play("Idle");
        transform.LookAt(lookAt);
        StartCoroutine(OnWaitingForFoodServed());
    }
    IEnumerator OnWaitingForFoodServed()
    {
        isMoving = false;
        transform.LookAt(usingTable.transform.position);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        animator.Play("StandSit");
        RandomOrder();
        usingTable.AddOrder(shokuOrders[0].amount, shokuOrders[1].amount);
        while (usingTable.NeedRefill)
        {
            yield return new WaitForEndOfFrame();
        }
        ShokuJi();
        Invoke(nameof(GoHome), 5f);
        Invoke(nameof(PayForMeal), 5f);
    }
    public void FreeTable()
    {
        usingTable.Using = false;
    }
}
