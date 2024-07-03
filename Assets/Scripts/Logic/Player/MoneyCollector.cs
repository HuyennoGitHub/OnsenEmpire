using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyCollector : ACollector, IMoneyCollector
{
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(BoosterCollectable.BoosterCollectableTag)) return;
        base.OnTriggerEnter(other);
    }
    protected override void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(BoosterCollectable.BoosterCollectableTag)) return;
        base.OnCollisionEnter(collision);
    }
    public void CollectMoney(int value)
    {
        UserData.SetCash(UserData.CurrentCash + value);
        UserData.SetFakeCash(UserData.CurrentShowingCash + value);
    }

    public void SpendMoney(int value)
    {
        UserData.SetFakeCash(UserData.CurrentShowingCash - value);
    }
}
