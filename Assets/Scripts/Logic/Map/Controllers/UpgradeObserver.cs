using IPS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeObserver : MonoBehaviour
{
    [Header("Tutorial")]
    [SerializeField] GameObject instruction;
    [SerializeField] MapCtrl mapCtrl;
    [SerializeField] UpgradeCtrl upgradeCtrl1;
    private bool isTutorial;
    private bool the1st;

    #region Singleton
    private static UpgradeObserver instance;

    public static UpgradeObserver Instance
    {
        get
        {
            if (instance != null) return instance;
            instance = FindObjectOfType<UpgradeObserver>();
            return instance;
        }
    }
    private void Awake()
    {
        if (instance != null && instance.GetInstanceID() != this.GetInstanceID())
        {
            DestroyImmediate(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion
    public void Init()
    {
        the1st = false;
        if (UserData.Inventory.isTutorial)
        {
            isTutorial = true;
            the1st = true;
            this.AddListener<EventDefine.CanUseEvent>(StartInstruction);
        }
        if (upgradeCtrl1.UpgradingRoomIndex < 2)
        {
            this.AddListener<EventDefine.OnUpgradeDone>(CheckOpenMech);
        }
    }
    private void StartInstruction()
    {
        Debug.Log("Can use event dispatched");
        if (!isTutorial || !UserData.Inventory.isTutorial) return;
        if (the1st)
        {
            the1st = false;
            return;
        }
        isTutorial = false;
        UserData.Inventory.SetEndTutorial();
        this.Dispatch<EventDefine.StartUpgradeRoom>();
        Debug.Log("StartUpgrade");
    }
    private void CheckOpenMech()
    {
        if (upgradeCtrl1.UpgradingRoomIndex == 2)
        {
            this.Dispatch<EventDefine.CanCallOrder>();
            this.Dispatch<EventDefine.OpenBoosterOffer>();
        }
    }
}
