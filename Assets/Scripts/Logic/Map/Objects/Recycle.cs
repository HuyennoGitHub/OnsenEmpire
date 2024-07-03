using IPS;
using System;
using UnityEngine;

public class Recycle : ARefillable
{
    [SerializeField] Transform fillTarget;
    public Refiller character;
    public GameObject interactArea;
    public Transform FillTarget => fillTarget ?? transform;
    protected override void Start()
    {
        base.Start();
        EnableInteract();
        base.SetNeedRefill();
        this.AddListener<EventDefine.BringingEvent>(SetNeedRefill);
        this.AddListener<EventDefine.NotBringEvent>(SetNotNeedRefill);
    }
    public override void OnCompleted()
    {
        //Filled();
        character.Throw(this);
    }

    public override void OnInteract(IInteractable other)
    {
        base.OnInteract(other);

    }
    public override void SetNeedRefill()
    {
        base.SetNeedRefill();
        SetRemainTime();
        EnableInteract();
        interactArea.SetActive(true);
    }
    public void SetNotNeedRefill()
    {
        //DisableInteract();
        interactArea.SetActive(false);
    }
    private void SetNeedRefill(object sender, EventArgs e)
    {
        SetNeedRefill();
    }
    private void SetNotNeedRefill(object sender, EventArgs e)
    {
        SetNotNeedRefill();
    }
}
