using IPS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RefillCollector : AInteractable, IRefillCollector
{
    [SerializeField] private int maxTarget;
    [SerializeField] Animator animator;
    [SerializeField] Transform collectPoint;

    public const string RefillCollectableTag = "RefillCollectable";

    public int MaxTarget
    {
        get { return maxTarget; }
        set { maxTarget = value; }
    }
    public int CollectedTotal => collecteds.Count;
    private List<IRefillCollectable> collecteds = new();

    public IRefillCollectable GetObject(RefillObjectType type)
    {
        if (type == RefillObjectType.FoodBoth)
        {
            for (int i = collecteds.Count - 1; i >= 0; i--)
            {
                if (collecteds[i].Type == RefillObjectType.Vegetable || collecteds[i].Type == RefillObjectType.Meat)
                {
                    return collecteds[i];
                }
            }
        }
        else if (type == RefillObjectType.None)
        {
            return null;
        }
        for (int i = collecteds.Count - 1; i >= 0; i--)
        {
            if (collecteds[i].Type == type) return collecteds[i];
        }
        return null;
    }
    public IRefillCollectable GetLastObject()
    {
        if (collecteds.Count == 0)
        {
            var arr = collectPoint.GetComponentsInChildren<IRefillCollectable>();
            if (arr.Length != 0)
            {
                foreach (var item in arr)
                {
                    collecteds.Add(item);
                }
            }
            if (collecteds.Count == 0) return null;
        }
        return collecteds[collecteds.Count - 1];
    }

    public void StartCollect(IRefillCollectable target)
    {
        if (target == null || CollectedTotal >= maxTarget) return;
        if (target.CoolingDown) return;
        var obj = target as RefillCollectable;
        if (obj == null) return;
        obj.SetCDState();
        PlayCollectAnim();
        return;
    }

    public void OnRemove(IRefillCollectable target)
    {
        if (target is IInteractable)
        {
            (target as AInteractable).DisableInteract();
        }
        collecteds.Remove(target);
        if (CompareTag("Player"))
        {
            if (CollectedTotal == 0)
            {
                this.Dispatch<EventDefine.NotBringEvent>();
            }
            else
            {
                this.Dispatch<EventDefine.BringingEvent>();
            }
        }
    }
    public IEnumerator OnCollecting(IRefillCollectable target)
    {
        if (target.IsForSpecialOrderThing && FindObjectByTypeInCollecteds(target.Type)) yield break;
        if (target == null || CollectedTotal >= maxTarget)
        {
            EndCollect(target);
            yield break;
        }
        if (collecteds.Contains(target)) yield break;
        if (target.CoolingDown) yield break;
        if (CompareTag("Player"))
        {
            target.BeCollect(collectPoint, OnRemove, CollectedTotal, true);
        }
        else
        {
            target.BeCollect(collectPoint, OnRemove, CollectedTotal, false);
        }
        yield return new WaitForSeconds(.5f);

        collecteds.Add(target);

        if (CompareTag("Player"))
        {
            SFX.Instance.Haptic();
            if (CollectedTotal == 1)
            {
                this.Dispatch<EventDefine.BringingEvent>();
            }
        }
        yield break;
    }
    public bool FindObjectByTypeInCollecteds(RefillObjectType type)
    {
        foreach (var obj in collecteds)
        {
            if (obj.Type == type) return true;
        }
        return false;
    }
    public IRefillCollectable GetObjectFromCollectedsByType(RefillObjectType type)
    {
        foreach (var obj in collecteds)
        {
            if (obj.Type == type)
            {
                return obj;
            }
        }
        return null;
    }
    public void EndCollect(IRefillCollectable target)
    {

    }
    public void ReOrganize()
    {
        for (int i = 0; i < collecteds.Count; i++)
        {
            collecteds[i].Organize(i);
        }
    }

    private void PlayCollectAnim()
    {
        if (animator == null) return;
        animator.Play("Grab");
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other == null || !other.CompareTag(RefillCollectableTag)) return;
        var target = other.GetComponent<IRefillCollectable>();
        if (target != null)
        {
            StartCollect(target);
        }
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        if (other == null || !other.CompareTag(RefillCollectableTag)) return;
        var target = other.GetComponent<IRefillCollectable>();
        if (target != null)
        {
            StartCoroutine(OnCollecting(target));
        }
    }

    protected virtual void OnCollisionEnter(Collision other)
    {
        if (other == null || !other.gameObject.CompareTag(RefillCollectableTag)) return;
        var target = other.gameObject.GetComponent<IRefillCollectable>();
        if (target != null)
        {
            StartCollect(target);
        }
    }
}
