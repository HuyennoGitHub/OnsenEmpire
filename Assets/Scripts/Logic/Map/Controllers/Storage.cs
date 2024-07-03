using IPS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour
{
    public ObjectData Info;
    public GameObject[] hideWalls;
    public UpgradeCtrl upgradeCtrl;
    public Loader[] loaders;

    private List<EventDefine.NeedRefillEvent> needRefills = new();

    private void Start()
    {
        this.AddListener<EventDefine.OnStartGame>(OnStartGame);
        this.AddListener<EventDefine.LoadMissionCompleteEvent>(AssignMission);
        Info.SetLevel(1);
        foreach (var wall in hideWalls)
        {
            wall.SetActive(false);
        }
        var inventory = UserData.Inventory;
        foreach (var item in inventory.unlocked)
        {
            if (item.Type == ObjectType.Loader)
            {
                foreach (var loader in loaders)
                {
                    if (loader.Info.Id.Equals(item.Id))
                    {
                        loader.UnlockLevel(item.Level);
                    }
                }
            }
        }
        loaders = GetComponentsInChildren<Loader>();
        upgradeCtrl.enabled = true;
    }
    public void Unlock()
    {
        GameData.Instance.GetObject(Info.Id, 1).Unlock();
        Info.SetLevel(1);
        UserData.Inventory.SaveItem(new ObjectData(Info.Id, ObjectType.Storage, 1), true);
    }
    private void OnStartGame()
    {
        foreach (var l in loaders)
        {
            StartCoroutine(AssignToLoader(l));
        }
    }
    public void AddNeedRefill(EventDefine.NeedRefillEvent needRefillEvent)
    {
        if (needRefills.Contains(needRefillEvent)) return;
        needRefills.Add(needRefillEvent);
        CheckUpdate();
    }
    private void CheckUpdate()
    {
        int index = 0;
        while (index < needRefills.Count)
        {
            if (!needRefills[index].needRefill.NeedRefill)
            {
                needRefills.RemoveAt(index);
            }
            else ++index;
        }
        //Debug.Log($"needRefills.count = {needRefills.Count}");
    }
    public void AssignMission(EventDefine.LoadMissionCompleteEvent param)
    {
        StartCoroutine(AssignToLoader(param.loader));
    }
    IEnumerator AssignToLoader(Loader loader)
    {
        while (needRefills.Count == 0)
        {
            yield return new WaitForEndOfFrame();
        }
        while (loader.MaxTarget > loader.missions.Count && needRefills.Count > 0)
        {
            loader.missions.Add(needRefills[0]);
            needRefills.RemoveAt(0);
        }
        loader.StartMission();
    }
    public void HireLoader()
    {
        loaders = GetComponentsInChildren<Loader>();
        Loader hiredLoader = null;
        if (loaders.Length == 1)
        {
            hiredLoader = loaders[0];
            GameData.Instance.GetObject(hiredLoader.Info.Id, 1).Unlock();
            hiredLoader.Upgrade();
        }
        else
        {
            foreach (Loader loader in loaders)
            {
                if (!GameData.Instance.GetObject(loader.Info.Id, 1).Unlocked)
                {
                    GameData.Instance.GetObject(loader.Info.Id, 1).Unlock();
                    GameData.Instance.GetObject(loader.Info.Id, 2).Unlock();
                    GameData.Instance.GetObject(loader.Info.Id, 3).Unlock();
                    loader.Upgrade();
                    hiredLoader = loader;
                    break;
                }
            }
        }
        StartCoroutine(AssignToLoader(hiredLoader));
    }
    public void UpgradeLoader()
    {
        GetUpgradeableLoader().Upgrade();
    }
    public Loader GetUpgradeableLoader()
    {
        foreach (Loader loader in loaders)
        {
            if (!GameData.Instance.GetObject(loader.Info.Id, 3).Unlocked) return loader;
        }
        return null;
    }
}
