using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public abstract class AInteractable : MonoBehaviour, IInteractable
{
    protected Vector3 halfSize;

    public BoxCollider boxCollider;
    protected BoxCollider BoxCollider
    {
        get
        {
            if (boxCollider != null) return boxCollider;
            boxCollider = GetComponent<BoxCollider>();
            return boxCollider;
        }
    }

    public bool IsInteractEnable => BoxCollider.enabled;

    public virtual void OnInteract(IInteractable other)
    {

    }

    public virtual void EnableInteract()
    {
        BoxCollider.enabled = true;
    }

    public virtual void DisableInteract()
    {
        BoxCollider.enabled = false;
    }

    protected virtual void RemoveSelf()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
