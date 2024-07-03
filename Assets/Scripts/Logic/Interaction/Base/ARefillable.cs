using UnityEngine;
using UnityEngine.UI;

public abstract class ARefillable : AInteractable, IRefillable
{
    [SerializeField] protected float refillDuration;
    [SerializeField] private Transform refillPos;
    public Image countDownCleanTime;
    public GameObject countDownClock;

    public bool Completed { get; private set; } = false;
    public bool IsRefilling { get; private set; } = false;
    public virtual bool NeedRefill { get; private set; } = false;

    public float RemainingTime => remainTime;

    private float remainTime;

    public Vector3 RefillPos => refillPos != null ? refillPos.position : transform.position;

    protected virtual void Start()
    {
        gameObject.tag = ARefiller.RefillableTag;
    }

    public virtual void SetRefillDuration(float duration)
    {
        refillDuration = duration;
    }
    public virtual void SetRemainTime()
    {
        remainTime = refillDuration;
    }

    public virtual void SetNeedRefill()
    {
        if (NeedRefill) return;
        NeedRefill = true;
        Completed = false;
        EnableInteract();
        if (this is RefillByDuration)
        {
            countDownClock.SetActive(true);
            countDownCleanTime.fillAmount = 0;
        }
    }

    public virtual void OnCompleted()
    {
        Completed = true;
        SetCleanStatus();
        if (this is RefillByDuration)
            countDownClock.SetActive(false);
    }

    public virtual void StartCooldown(float duration)
    {
        if (IsRefilling || !NeedRefill) return;
        IsRefilling = true;
        SetRefillDuration(duration);
        remainTime = duration;
    }

    public virtual void EndCooldown()
    {
        if (!IsRefilling) return;
        SetCleanStatus();
    }
    public void SetCleanStatus()
    {
        IsRefilling = false;
        NeedRefill = false;
    }

    public virtual void OnCooldowning(float step)
    {
        if (Completed || !IsRefilling) return;
        if (this is RefillByAmount) return;
        remainTime -= step;
        if (this is RefillByDuration)
        {
            countDownCleanTime.fillAmount = 1 - remainTime / refillDuration;
        }
        if (remainTime <= 0 && !Completed)
        {
            remainTime = 0;
            OnCompleted();
        }
    }
    public virtual void Filled()
    {
        NeedRefill = false;
    }
}
