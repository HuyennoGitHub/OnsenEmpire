using System;
using UnityEngine;

public interface IInteractable
{
    bool IsInteractEnable { get; }
    void OnInteract(IInteractable other);
    void EnableInteract();
    void DisableInteract();
}

public interface ICollectable
{
    bool OnCollected(ICollector collector);
}

public interface ICollector
{

}
public interface IMoneyCollector
{
    void CollectMoney(int value);
    void SpendMoney(int value);
}

public interface IUseable
{
    bool CanUse { get; }
    Vector3 UsePos { get; }
}

public interface IRefillCollector
{
    public int CollectedTotal { get; }
    void StartCollect(IRefillCollectable target);
    void EndCollect(IRefillCollectable target);
}

public enum RefillObjectType { Sake, Towel, Vegetable, Meat, None, FoodBoth, ToiletPaper }

public interface IRefillCollectable
{
    public RefillObjectType Type { get; }

    public bool Collected { get; }
    public bool Filled { get; }
    public bool CoolingDown { get; }
    public bool IsForSpecialOrderThing { get; }
    public void Organize(int order);
    void BeCollect(Transform target, Action<IRefillCollectable> onRemoveCallback, int order, bool playSFX);
    void BeFill(Transform target, bool playSFX = true);
}

public interface IRefillable
{
    Vector3 RefillPos { get; }
    float RemainingTime { get; }
    bool NeedRefill { get; }
    bool IsRefilling { get; }
    void SetRefillDuration(float remainingTime);
    void StartCooldown(float duration);
    void EndCooldown();
    void OnCooldowning(float step);
    void OnCompleted();
}

public interface IRefiller
{
    float RefillSpeed { get; }
    void SetRefillSpeed(float speed);
    void StartRefill(IRefillable target);
    void EndRefill(IRefillable target);
    void OnFilling(IRefillable target);
}

public interface IMoveable
{
    float MoveSpeed { get; }
    void SetMoveSpeed(float speed);
    void MoveTo(Vector3 target);
}
public interface IRoom
{
    bool CanUse { get; }
    bool Using { get; }
    Vector3 UsePos { get; }
    ServiceType ServiceOfType { get; }
}

public enum ServiceType { Onsen, Sauna, Restaurant}
