using IPS;
using UnityEngine;

public class RefillByDuration : ARefillable
{
    public GameObject sparkle;
    public AudioClip cleanedClip;
    public AudioClip cleaningClip;
    public Animator animator;
    MapCtrl mapCtrl;
    public GameObject attachObject;
    [Header("Tutorial")]
    public GameObject instruction;

    protected override void Start()
    {
        base.Start();
        InitVisual();
        if (instruction != null)
        {
            mapCtrl = FindObjectOfType<MapCtrl>();
        }
    }
    public void InitVisual()
    {
        var usable = GetComponent<Usable>();
        if (usable == null) return;
        if (usable.Status == Usable.ObjectStatus.Dirty)
        {
            SetNeedRefill();
        }
    }
    public override void SetNeedRefill()
    {
        base.SetNeedRefill();
        if (mapCtrl != null && mapCtrl.IsTutorialCleanroom && instruction != null)
        {
            instruction.SetActive(true);
        }
        animator.Play("Dirty");
        var usable = GetComponent<Usable>();
        if (usable != null)
        {
            usable.SetDirty();
        }
        if (attachObject != null)
        {
            attachObject.SetActive(true);
        }
    }
    public override void OnCooldowning(float step)
    {
        if (Completed || !IsRefilling) return;
        base.OnCooldowning(step);
    }
    public override void OnCompleted()
    {
        base.OnCompleted();
        EndCooldown();
        if (mapCtrl != null && mapCtrl.IsTutorialCleanroom && instruction != null)
        {
            instruction.SetActive(false);
        }
        ShowSparkleVFX();
        SetClean();
        this.Dispatch(new EventDefine.CleanEvent { inRoom = this.gameObject.transform.parent.GetComponent<Room>() });
    }
    public void SetClean()
    {
        base.OnCompleted();
        SetCleanStatus();
        DisableInteract();
        animator.Play("Clean");
        var usable = GetComponent<Usable>();
        if (usable != null)
        {
            usable.Cleaned();
        }
        if (attachObject != null)
        {
            attachObject.SetActive(false);
        }
    }
    public override void OnInteract(IInteractable other)
    {
        if (other is not ARefiller) return;
        base.OnInteract(other);
    }
    private void ShowSparkleVFX()
    {
        sparkle.SetActive(true);
        Invoke(nameof(HideSparkleVFX), 1f);
    }
    private void HideSparkleVFX()
    {
        sparkle.SetActive(false);
    }
    public void PlayCleaningSound()
    {
        SFX.Instance.PlaySound(cleaningClip, true);
    }
    public void PlayCleanedSound()
    {
        SFX.Instance.PlaySound(cleanedClip);
    }
    public void StopSound()
    {
        SFX.Instance.StopAudio();
    }
}
