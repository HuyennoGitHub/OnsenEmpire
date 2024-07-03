using IPS;
using System.Collections.Generic;
using UnityEngine;

public class GameData : SingletonResourcesScriptable<GameData>
{
    public ObjectData[] defaultUnlocked;
    [SerializeField] ObjectInfo[] objectsData;
    [SerializeField] public List<ObjectInSauna> remainAmountInSauna;
    [SerializeField] LevelUpPoint[] levelSakuraRate;
    [SerializeField] public UpgradingIndex[] upgradingIndex;
    [SerializeField] public ExpandAreaStatus[] expandAreaStatuses;
    private bool loadedData = false;

    public void LoadExpandAreaStatus()
    {
        if (loadedData) return;
        loadedData = true;
        var inventory = UserData.Inventory;
        foreach (var item in inventory.expandAreaStatuses)
        {
            ExpandAreaStatus result = GetExpandAreaStatus(item.Id);
            if (result.IsNull) return;
            SetStatusLockArea(result.Order, item.StatusLock, false);
        }
    }
    public void SetStatusLockArea(uint order, StatusLockArea newStatus, bool save)
    {
        expandAreaStatuses[order].SetAreaStatusLock(newStatus);
        if (save)
        {
            UserData.Inventory.SaveExpandAreaStatus(new ExpandAreaStatus(order, expandAreaStatuses[order].Id,
                                                                        expandAreaStatuses[order].ConditionLevel, newStatus), true);
        }
    }
    public ExpandAreaStatus GetExpandAreaStatus(string id)
    {
        var result = System.Array.Find(expandAreaStatuses, s => s.Id.Equals(id));
        if (result.IsNull)
        {
            Debug.LogError($"Cannot find Area {id}'s Status");
        }
        return result;
    }
    public ExpandAreaStatus GetExpandableArea()
    {
        var result = System.Array.Find(expandAreaStatuses, s => s.IsExpandable && s.StatusLock == StatusLockArea.Showing);
        if (result.IsNull)
        {
            Debug.LogError($"Cannot find any expandable area.");
        }
        return result;
    }
    public LevelUpPoint GetSakuraRateOfLevel(int level)
    {
        LevelUpPoint point = System.Array.Find(levelSakuraRate, i => i.Level == level);
        if (point.Level == level) return point;
        else Debug.LogError($"Cannot find sakura rate of level {level}");
        return point;

    }
    public void SetUpgradingIndexs()
    {
        var inventory = UserData.Inventory;
        upgradingIndex = inventory.upgradingIndexs.ToArray();
    }
    public UpgradingIndex GetUpIndex(uint region)
    {
        return System.Array.Find(upgradingIndex, i => region == i.Region);
    }
    public void GetDataRemainAmountInSauna()
    {
        var inventory = UserData.Inventory;
        if (inventory.remainAmountInSauna.Count != 0)
        {
            foreach (var s in inventory.remainAmountInSauna)
            {
                foreach (var r in remainAmountInSauna)
                {
                    if (s.ID.Equals(r.ID))
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            r.SetRemainAmountInSauna(i, s.GetAmountByIndex(i));
                        }
                        break;
                    }
                }
            }
        }
    }
    public ObjectInSauna GetObjectInSauna(string id)
    {
        ObjectInSauna obj = remainAmountInSauna.Find(i => i.ID.Equals(id));
        if (!obj.IsNull) return obj;
        else Debug.LogError($"object not found id = {id}");
        return obj;
    }
    public ObjectInfo GetObject(string id)
    {
        var obj = System.Array.Find(objectsData, i => i.Id.Equals(id));
        if (obj != null) return obj;
        else Debug.LogError($"object not found id={id}");
        return obj;
    }

    public LevelInfo GetObject(string id, int level)
    {
        var obj = GetObject(id);
        if (obj == null) return null;

        return obj.GetLevel(level);
    }
    public void SaveInventory()
    {
        var inventory = UserData.Inventory;
        foreach (var info in objectsData)
        {
            if (info.GetLevel(1).Unlocked)
            {
                if (info.Type == ObjectType.Sauna)
                {
                    inventory.SaveRemainAmountInSauna(GetObjectInSauna(info.Id), false);
                }
            }
        }
        inventory.Save();
    }
}

[System.Serializable]
public enum ObjectType { Room, Cleaner, Loader, Receptionist, Sauna, Area, Restaurant, Waiter, Storage }

[System.Serializable]
public class ObjectInfo
{
    [SerializeField] string id;
    [SerializeField] ObjectType type;
    [SerializeField] LevelInfo[] levels;

    public string Id => id;

    public ObjectType Type => type;

    public LevelInfo GetLevel(int level)
    {
        var lv = System.Array.Find(levels, i => i.Level == level);
        if (lv != null) return lv;
        else Debug.LogError($"{id} Level not found {level}");
        return lv;
    }
}

[System.Serializable]
public class LevelInfo
{
    [SerializeField] int level;
    [SerializeField] bool unlocked;
    [SerializeField] int unlockCost;
    [SerializeField] GameObject[] prefab3Ds;
    [SerializeField] Sprite[] avatar2D = new Sprite[3];

    public int Level => level;

    public bool Unlocked => unlocked;
    public int UnlockCost => unlockCost;

    public void Unlock()
    {
        unlocked = true;
    }
    public GameObject GetVisual3D(uint option)
    {
        return prefab3Ds[option];
    }
    public Sprite GetAvatar2D(int option)
    {
        return avatar2D[option];
    }
}

[System.Serializable]
public struct ObjectInSauna
{
    [SerializeField] private string idSauna;
    [SerializeField] private int[] remainAmountInSauna;

    public ObjectInSauna(string id)
    {
        this.idSauna = id;
        remainAmountInSauna = new int[3];
        remainAmountInSauna[0] = 0;
        remainAmountInSauna[1] = 0;
        remainAmountInSauna[2] = 0;
    }
    public ObjectInSauna(string id, int param1, int param2, int param3)
    {
        this.idSauna = id;
        remainAmountInSauna = new int[3];
        remainAmountInSauna[0] = param1;
        remainAmountInSauna[1] = param2;
        remainAmountInSauna[2] = param3;
    }
    public bool IsNull => string.IsNullOrEmpty(idSauna);
    public string ID => idSauna;
    public int GetAmountByIndex(int index)
    {
        return remainAmountInSauna[index];
    }
    public void SetRemainAmountInSauna(int index, int value)
    {
        remainAmountInSauna[index] = value;
    }
}
[System.Serializable]
public struct LevelUpPoint
{
    [SerializeField] int level;
    [SerializeField] int point;
    [SerializeField] int reward;

    public int Level => level;
    public int UpPoint => point;
    public int Reward => reward;

}
[System.Serializable]
public struct ExpandAreaStatus
{
    [SerializeField] uint areaOrder;
    [SerializeField] string areaId;
    [SerializeField] int expandableConditionSakuraLevel;
    [SerializeField] StatusLockArea status;

    public uint Order => areaOrder;
    public string Id => areaId;
    public int ConditionLevel => expandableConditionSakuraLevel;
    public bool IsExpandable => expandableConditionSakuraLevel <= UserData.CurrentSakuraLevel;
    public bool IsNull => string.IsNullOrEmpty(Id);
    public StatusLockArea StatusLock => status;
    public void SetAreaStatusLock(StatusLockArea newStatus)
    {
        status = newStatus;
    }
    public ExpandAreaStatus(uint order, string id, int conditionLv, StatusLockArea status)
    {
        areaOrder = order;
        areaId = id;
        expandableConditionSakuraLevel = conditionLv;
        this.status = status;
    }
}
public enum StatusLockArea
{
    WaitingForShowing,
    Showing,
    WaitingForUnlock,
    Unlocked
}