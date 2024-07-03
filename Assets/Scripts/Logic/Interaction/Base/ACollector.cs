using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class ACollector : AInteractable, ICollector
{
    protected virtual void OnTriggerEnter(Collider other)
    {
        InteractWith(other);
    }
    protected virtual void OnCollisionEnter(Collision collision)
    {
        InteractWith(collision);
    }

    private void InteractWith(Collider other)
    {
        if (other == null) return;
        var obj = other.GetComponent<IInteractable>();
        if (obj != null)
        {
            obj.OnInteract(this);

            if (obj is ICollectable)
            {
                (obj as ICollectable).OnCollected(this);
            }
        }
    }
    private void InteractWith(Collision other)
    {
        if (other == null) return;
        var obj = other.gameObject.GetComponent<IInteractable>();
        if (obj != null)
        {
            obj.OnInteract(this);

            if (obj is ICollectable)
            {
                (obj as ICollectable).OnCollected(this);
            }
        }
    }
}
