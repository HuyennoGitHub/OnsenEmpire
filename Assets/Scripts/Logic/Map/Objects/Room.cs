using IPS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour, IRoom
{
    [SerializeField] Transform visualGraphic;
    [SerializeField] public RefillByDuration[] refillableObjects;
    [SerializeField] Usable[] useableObjects;
    [SerializeField] bool lookLeft;

    //public GameObject fog;
    public ObjectData Info;
    public GameObject logDirty;
    public Transform center;
    public GameObject[] hideWalls;
    [SerializeField] RoomData roomData;
    //[SerializeField] uint regionBelong;
    public Area area;
    public GameObject fogVFX;
    public Customer usedCustomer;
    public int RoomCost => roomData.checkin * (Info.Level * 2 - 1);
    public int RoomTip => roomData.tip * (Info.Level * 2 - 1);
    public int RoomOrder => roomData.orderFee * (Info.Level * 2 - 1);
    public bool LookAtTheLeftSide => lookLeft;
    //public uint Region => regionBelong;

    [ContextMenu("BindObject")]
    private void BindObject()
    {
        if (refillableObjects == null || refillableObjects.Length == 0)
        {
            refillableObjects = transform.GetComponentsInChildren<RefillByDuration>();
        }

        if (useableObjects == null || useableObjects.Length == 0)
        {
            useableObjects = transform.GetComponentsInChildren<Usable>();
        }
    }

    private void Start()
    {
        foreach (var wall in hideWalls)
        {
            wall.SetActive(false);
        }
        usedCustomer = null;
    }
    private void Awake()
    {
        BindObject();
        this.AddListener<EventDefine.CleanEvent>(CheckCanUse);
    }
    public RefillByDuration RefillTarget => refillableObjects != null ? Array.Find(refillableObjects, i => i.NeedRefill) : null;

    public Usable UseTarget => useableObjects != null ? Array.Find(useableObjects, i => i.CanUse) : null;

    public bool CanUse => RefillTarget == null && !Using;

    public Vector3 UsePos
    {
        get
        {
            var target = UseTarget;
            if (target != null)
            {
                return target.UsePos;
            }
            return transform.position;
        }
    }

    public ServiceType ServiceOfType => ServiceType.Onsen;

    public bool Using { get; set; } = false;

    private void CheckCanUse(EventDefine.CleanEvent param)
    {
        if (param.inRoom != this) return;
        CheckCanUseRoom();
    }
    public void CheckCanUseRoom()
    {
        if (CanUse)
        {
            HideLogDirty();
            this.Dispatch(new EventDefine.CanUseEvent { room = this });
        }
    }
    public void SetClean()
    {
        foreach (var obj in refillableObjects)
        {
            obj.SetClean();
        }
        if (!Using) this.Dispatch(new EventDefine.CanUseEvent { room = this });
        logDirty.SetActive(false);
    }
    public void SetDirty()
    {
        foreach (var obj in refillableObjects)
        {
            obj.SetNeedRefill();
        }
        this.Dispatch(new EventDefine.NeedCleanEvent { needCleanRoom = this, areaId = area.Info.Id });
        logDirty.SetActive(true);
        usedCustomer = null;
    }
    void HideLogDirty()
    {
        logDirty.SetActive(false);
    }

    public bool IsUnlocked => !Info.IsNull && Info.Unlocked;

    public void UnlockLevel(int lv = 1, uint visualOption = 1, bool save = true)
    {
        for (int i = 1; i <= lv; i++)
        {
            GameData.Instance.GetObject(Info.Id, i).Unlock();
        }
        Info.SetLevel(lv);
        LoadVisual(visualOption);
        if (save) SaveInventory();
    }
    public void SaveInventory()
    {
        UserData.Inventory.SaveItem(new ObjectData(Info.Id, ObjectType.Room, Info.Level), true);
    }

    private void LoadVisual(uint option = 1)
    {
        var obj = GameData.Instance.GetObject(Info.Id, Info.Level);
        if (obj == null)
        {
            Debug.LogError($"Object id={Info.Id} does not exist in GameData");
            return;
        }

        // Destroy current visual level
        if (visualGraphic.childCount > 0)
        {
            for (int i = visualGraphic.childCount - 1; i >= 0; --i)
            {
                Destroy(visualGraphic.GetChild(i).gameObject);
            }

        }

        // Spawn new level prefab
        var visual = Instantiate(obj.GetVisual3D(option - 1), visualGraphic.parent);
        Destroy(visualGraphic.gameObject);
        visualGraphic = visual.transform;
        Transform visualChild = visualGraphic;
        for (int i = 0; i < refillableObjects.Length; ++i)
        {
            refillableObjects[i].animator = visualChild.GetChild(visualChild.childCount - 4 + i).GetComponent<Animator>();
        }
        for (int i = 0; i < useableObjects.Length; ++i)
        {
            useableObjects[i].tapWater = visualChild.GetChild(visualChild.childCount - 5 + i).gameObject;
        }
        foreach (var refillableObject in refillableObjects)
        {
            refillableObject.InitVisual();
        }
    }
    public void Upgrade(uint optionVisual)
    {
        SetClean();
        //currentLevel++;
        Info.SetLevel(Info.Level + 1);
        GameData.Instance.GetObject(Info.Id, Info.Level).Unlock();
        Info.SetVisual(optionVisual);
        UserData.Inventory.SaveItem(new ObjectData(Info.Id, ObjectType.Room, Info.Level, optionVisual), true);

        LoadVisual(optionVisual);
    }
    public void TurnOnFogVFX()
    {
        fogVFX.SetActive(true);
        Invoke(nameof(TurnOffFogVFX), 5f);
    }
    private void TurnOffFogVFX()
    {
        fogVFX.SetActive(false);
    }
}
