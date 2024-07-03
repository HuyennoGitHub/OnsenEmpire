using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Usable : AInteractable, IUseable
{
    [SerializeField] Transform usePos;
    [SerializeField] BoxCollider useableCollider;
    public GameObject tapWater;

    public ObjectStatus Status { get; private set; } = ObjectStatus.Clean;

    public enum ObjectStatus
    {
        Dirty,
        Clean
    }
    public override void OnInteract(IInteractable other)
    {

    }
    public void Cleaned()
    {
        Status = ObjectStatus.Clean;
        useableCollider.enabled = true;
        tapWater.SetActive(true);
    }
    public void SetDirty()
    {
        Status = ObjectStatus.Dirty;
        tapWater.SetActive(false);
    }

    public bool CanUse => Status != ObjectStatus.Dirty;

    public Vector3 UsePos => usePos != null ? usePos.transform.position : transform.position;
}
