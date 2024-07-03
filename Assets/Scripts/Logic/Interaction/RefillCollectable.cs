using DG.Tweening;
using System;
using UnityEngine;

public class RefillCollectable : AInteractable, IRefillCollectable
{
    [SerializeField] RefillObjectType type;
    [SerializeField] GameObject refillObjectPrefab;
    [SerializeField] bool isForSpecialOrderThing;
    [SerializeField] AudioClip receiveItemClip;

    public float CDTime = 1;
    public bool CoolingDown { get; private set; } = false;
    public bool IsForSpecialOrderThing => isForSpecialOrderThing;

    public RefillObjectType Type => type;
    public bool Collected { get; private set; }
    public bool Filled { get; private set; }

    Action<IRefillCollectable> onRemoveCallback;

    protected virtual void Start()
    {
        gameObject.tag = RefillCollector.RefillCollectableTag;
    }
    private void PlaySFX()
    {
        SFX.Instance.PlaySound(receiveItemClip);
    }

    public void BeCollect(Transform target, Action<IRefillCollectable> onRemoveCallback, int order, bool playSFX)
    {
        if (Collected) return;
        if (CoolingDown) return;
        if (isForSpecialOrderThing)
        {
            BeCollectForSpecialOrder(target, onRemoveCallback, order, playSFX);
            return;
        }
        Collected = true;
        this.onRemoveCallback = onRemoveCallback;
        var obj = Instantiate(refillObjectPrefab, transform.parent);
        obj.transform.position = transform.position;
        obj.transform.rotation = transform.rotation;
        obj.GetComponent<RefillCollectable>().SetCDState();
        transform.SetParent(target);
        transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
        transform.DOLocalMove(Vector3.zero + Vector3.up * 0.25f * order, .5f);
        transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, 90, transform.localRotation.eulerAngles.z);
        DisableInteract();
        if (playSFX) PlaySFX();

    }
    public void SetCDState()
    {
        if (CoolingDown) return;
        CoolingDown = true;
        Invoke(nameof(SetAvailable), CDTime);
    }
    public void BeCollectForSpecialOrder(Transform target, Action<IRefillCollectable> onRemoveCallback, int order, bool playSFX)
    {
        GetComponent<Sake>().DisableInteract();
        Collected = true;
        this.onRemoveCallback = onRemoveCallback;
        var obj = Instantiate(refillObjectPrefab, transform.parent);
        obj.transform.position = transform.position;
        obj.GetComponent<RefillCollectable>().SetCDState();
        transform.SetParent(target);
        transform.DOLocalMove(Vector3.zero + Vector3.up * 0.25f * order, .5f);
        DisableInteract();
        if (playSFX) PlaySFX();
    }
    public void SetAvailable()
    {
        CoolingDown = false;
    }

    public void BeFill(Transform target, bool playSFX = true)
    {
        if (Filled) return;
        Filled = true;
        onRemoveCallback?.Invoke(this);
        target.gameObject.SetActive(true);
        transform.SetParent(target);
        transform.DOLocalMove(Vector3.zero, .5f);
        if (playSFX) PlaySFX();
        Invoke(nameof(RemoveSelf), .5f);
    }

    public void Organize(int order)
    {
        transform.DOLocalMove(Vector3.zero + Vector3.up * .25f * order, .2f);
    }
}
