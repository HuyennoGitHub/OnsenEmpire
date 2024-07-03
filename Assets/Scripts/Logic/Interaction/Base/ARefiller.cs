using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ARefiller : AInteractable, IRefiller
{
    [SerializeField] float refillSpeed = 1;
    public float RefillSpeed => refillSpeed;
    public bool IsRefilling { get; private set; }

    IRefillable target;
    protected AudioSource source;

    public const string RefillableTag = "Refillable";

    public void SetRefillSpeed(float speed)
    {
        this.refillSpeed = speed;
    }

    public virtual void StartRefill(IRefillable other)
    {
        if (other == null || IsRefilling) return;

        IsRefilling = true;
        float doTaskTime = 1f;
        if (other is RefillByDuration)
        {
            var refillByDuration = other as RefillByDuration;
            if (CompareTag("Player"))
            {
                refillByDuration.PlayCleaningSound();
            }
        }
        var cleaner = transform.GetComponent<Cleaner>();
        if (cleaner != null)
        {
            doTaskTime = cleaner.doTaskTime;
        }
        other.StartCooldown(doTaskTime);
    }

    public virtual void EndRefill(IRefillable other)
    {
        if (other == null || !IsRefilling) return;
        IsRefilling = false;
        if (other is RefillByDuration)
        {
            (other as RefillByDuration).StopSound();
        }
        target = null;
    }

    public virtual void OnFilling(IRefillable other)
    {
        if (other == null || !IsRefilling) return;
        other.OnCooldowning(RefillSpeed * Time.fixedDeltaTime);

        if (!other.NeedRefill)
        {
            EndRefill(target);
            if (other is RefillByDuration)
            {
                var refillByDuration = other as RefillByDuration;
                refillByDuration.StopSound();
                if (CompareTag("Player"))
                {
                    refillByDuration.PlayCleanedSound();
                    SFX.Instance.Vibrate();
                }
            }
        }
    }

    public override void OnInteract(IInteractable other)
    {
        base.OnInteract(other);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other == null || target != null || !other.CompareTag(RefillableTag)) return;
        target = other.GetComponent<IRefillable>();
        if (target is RefillByAmount && other is BoxCollider)
        {
            target = null;
            return;
        }
        if (target is ShokuTable && other is CapsuleCollider && target.NeedRefill)
        {
            StartRefill(target);
            return;
        }
        if (target != null && target.NeedRefill)
        {
            StartRefill(target);
        }
        else
        {
            target = null;
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other == null || target == null || !other.CompareTag(RefillableTag)) return;
        EndRefill(target);
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        if (other == null || target == null || !other.CompareTag(RefillableTag)) return;
        OnFilling(target);
    }

    protected virtual void OnCollisionEnter(Collision other)
    {
        if (other == null || target != null || !other.gameObject.CompareTag(RefillableTag)) return;
        target = other.gameObject.GetComponent<IRefillable>();
        if (target is RefillByAmount && other.collider is BoxCollider)
        {
            target = null;
            return;
        }
        if (target != null && target.NeedRefill)
        {
            StartRefill(target);
        }
        else
        {
            target = null;
        }
    }

    protected virtual void OnCollisionExit(Collision other)
    {
        if (other == null || target == null || !other.gameObject.CompareTag(RefillableTag)) return;
        EndRefill(target);
    }

    protected virtual void OnCollisionStay(Collision other)
    {
        if (other == null || target == null || !other.gameObject.CompareTag(RefillableTag)) return;
        OnFilling(target);
    }
}
