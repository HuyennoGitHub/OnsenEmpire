using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ACollectable : AInteractable, ICollectable
{
    [SerializeField] protected GameObject mainObj;
    [SerializeField] protected GameObject collectVfx;

    public virtual bool OnCollected(ICollector collector)
    {
        DisableInteract();
        PlayCollectVfx();
        OnCollectedCallback();
        return true;
    }

    protected virtual void OnCollectedCallback()
    {
        Invoke(nameof(RemoveSelf), 1);
    }

    protected virtual void PlayCollectVfx()
    {
        if (mainObj) mainObj.gameObject.SetActive(false);

        if (collectVfx)
        {
            collectVfx.SetActive(true);
            collectVfx.transform.SetParent(null);
        }
    }

    protected override void RemoveSelf()
    {
        if (collectVfx)
        {
            collectVfx.transform.SetParent(transform);
            collectVfx.SetActive(false);
        }
        base.RemoveSelf();
    }
}
