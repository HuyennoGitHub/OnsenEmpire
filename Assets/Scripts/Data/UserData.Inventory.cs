using System.Collections.Generic;
using UnityEngine;

public partial class UserData
{
    public static Inventory Inventory
    {
        get
        {
            string data = GetString(InventoryKey);
            Debug.Log(data);
            if (string.IsNullOrEmpty(data))
            {
                // Set default object for first install
                SetCash(50);
                InitShowingCash();
                var inventory = new Inventory();
                inventory.isTutorial = true;
                foreach (var o in GameData.Instance.defaultUnlocked)
                {
                    inventory.SaveItem(new ObjectData(o.Id, o.Type, o.Level), false);
                }
                foreach (var i in GameData.Instance.upgradingIndex)
                {
                    inventory.SaveUpgradingIndex(new UpgradingIndex(i.Region, i.Room), false);
                }
                foreach (var i in GameData.Instance.remainAmountInSauna)
                {
                    inventory.SaveRemainAmountInSauna(new ObjectInSauna(i.ID, i.GetAmountByIndex(0), i.GetAmountByIndex(1), i.GetAmountByIndex(2)), false);
                }
                foreach (var area in GameData.Instance.expandAreaStatuses)
                {
                    inventory.SaveExpandAreaStatus(new ExpandAreaStatus(area.Order, area.Id, area.ConditionLevel, StatusLockArea.Showing), false);
                }
                inventory.Save();
                data = GetString(InventoryKey);
            }
            return JsonUtility.FromJson<Inventory>(data);
        }
    }
}
[System.Serializable]
public class Inventory
{
    [SerializeField] public List<ObjectData> unlocked = new List<ObjectData>();
    [SerializeField] public List<ObjectInSauna> remainAmountInSauna = new List<ObjectInSauna>();
    [SerializeField] public List<UpgradingIndex> upgradingIndexs = new List<UpgradingIndex>();
    [SerializeField] public bool isTutorial;
    [SerializeField] public List<ExpandAreaStatus> expandAreaStatuses = new List<ExpandAreaStatus>();

    public int FindTheMostExpensiveUnlocked()
    {
        int max = 0;
        foreach (var item in unlocked)
        {
            int cost = GameData.Instance.GetObject(item.Id, item.Level).UnlockCost;
            if (cost > max) max = cost;
        }
        return max;
    }
    public void SetEndTutorial()
    {
        isTutorial = false;
        Save();
    }
    private bool CheckContainInList(ObjectInSauna item)
    {
        foreach (var obj in remainAmountInSauna)
        {
            if (obj.ID.Equals(item.ID)) return true;
        }
        return false;
    }
    public void SaveUpgradingIndex(UpgradingIndex index, bool save)
    {
        if (upgradingIndexs.Contains(index)) return;
        if (CheckContainInList(index))
        {
            var exist = upgradingIndexs.Find(i => i.Region == index.Region);
            upgradingIndexs.Remove(exist);
        }
        upgradingIndexs.Add(index);
        if (save) Save();
    }
    private bool CheckContainInList(UpgradingIndex item)
    {
        foreach (var obj in upgradingIndexs)
        {
            if (obj.Region == item.Region) return true;
        }
        return false;
    }
    public void SaveItem(ObjectData item, bool save)
    {
        if (item.IsNull) return;
        //if (unlocked.Contains(item)) return;
        if (CheckContainInList(item))
        {
            var exist = unlocked.Find(i => i.Id.Equals(item.Id));
            unlocked.Remove(exist);
        }
        unlocked.Add(item);

        if (save) Save();
    }
    private bool CheckContainInList(ObjectData item)
    {
        foreach (var obj in unlocked)
        {
            if (obj.Id.Equals(item.Id)) return true;
        }
        return false;
    }
    public void SaveRemainAmountInSauna(ObjectInSauna item, bool save)
    {
        if (remainAmountInSauna.Contains(item)) return;
        if (CheckContainInList(item))
        {
            var exist = remainAmountInSauna.Find(i => i.ID.Equals(item.ID));
            remainAmountInSauna.Remove(exist);
        }
        remainAmountInSauna.Add(item);
        if (save) Save();
    }
    public void SaveRoomIndex(uint region, int index)
    {
        UpgradingIndex found = upgradingIndexs.Find(i => i.Region == region);
        SaveUpgradingIndex(new UpgradingIndex(region, index), true);
    }
    public void SaveExpandAreaStatus(ExpandAreaStatus item, bool save)
    {
        if (expandAreaStatuses.Contains(item)) return;
        var exist = expandAreaStatuses.Find(i => i.Id.Equals(item.Id));
        expandAreaStatuses.Remove(exist);
        expandAreaStatuses.Add(item);
        if (save) Save();
    }
    public void Save()
    {
        if (unlocked.Count == 0)
        {
            Debug.LogError("Empty data inventory!");
            return;
        }

        string data = JsonUtility.ToJson(this);
        if (string.IsNullOrEmpty(data))
        {
            Debug.LogError("Somethings wrong!");
            return;
        }
        UserData.SetString(UserData.InventoryKey, data);
    }
}
[System.Serializable]
public struct ObjectData
{
    [SerializeField] string id;
    [SerializeField] ObjectType type;
    [SerializeField] int lv; // 0: locked.
    [SerializeField] uint visualChosen;

    public string Id => id;
    public ObjectType Type => type;
    public int Level => lv;
    public uint Visual => visualChosen;

    public bool IsNull => string.IsNullOrEmpty(id);

    public ObjectData(string id, ObjectType type, int lv, uint visual = 1)
    {
        this.id = id;
        this.type = type;
        this.lv = lv;
        visualChosen = visual;
    }

    public void SetLevel(int newLevel)
    {
        lv = newLevel;
    }
    public void SetVisual(uint newVisual)
    {
        visualChosen = newVisual;
    }

    public bool Unlocked => Level > 0;
}
[System.Serializable]
public struct UpgradingIndex
{
    [SerializeField] uint region;
    [SerializeField] int roomIndex;

    public uint Region => region;
    public int Room
    {
        get { return roomIndex; }
        set { roomIndex = value; }
    }
    public UpgradingIndex(uint region, int roomIndex)
    {
        this.region = region;
        this.roomIndex = roomIndex;
    }
}
