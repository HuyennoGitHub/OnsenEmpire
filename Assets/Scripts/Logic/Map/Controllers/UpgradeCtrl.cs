using IPS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class UpgradeCtrl : MonoBehaviour
{
    [SerializeField] uint region;
    [SerializeField] GameObject[] upgradeRoomOrder;
    [SerializeField] GameObject[] upgradeOtherOrder;

    [Header("Tutorial")]
    [SerializeField] GameObject instruction;


    private int upgradingRoomIndex;
    public int UpgradingRoomIndex
    {
        get { return upgradingRoomIndex; }
        set
        {
            upgradingRoomIndex = value;
            UserData.Inventory.SaveRoomIndex(region, value);
        }
    }

    private bool isUpgradeFlowStarted;
    private void Start()
    {
        this.AddListener<EventDefine.OnStartGame>(OnStartGame);
        if (region == 0)
        {
            CheckOtherUpgradeable();
            return;
        }
        area = GetComponent<Area>();
        foreach (var item in upgradeRoomOrder)
        {
            item.SetActive(false);
        }
        foreach (var item in upgradeOtherOrder)
        {
            item.SetActive(false);
        }
        var inventory = UserData.Inventory;
        GameData.Instance.SetUpgradingIndexs();
        SetUpgradeIndex(GameData.Instance.GetUpIndex(region).Room);
        if (region == 1)
        {
            UpgradeObserver.Instance.Init();
        }
        else
        {
            if (upgradingRoomIndex == -1)
            {
                StartUpgradeRoomOrder();
            }
        }
        CheckOtherUpgradeable();
    }
    private void OnEnable()
    {
        this.AddListener<EventDefine.OnUpLevel>(CheckOtherUpgradeable);
        this.AddListener<EventDefine.OnUpgradeDone>(CheckOtherUpgradeable);
        CheckOtherUpgradeable();
    }
    private void OnStartGame()
    {
        if (region == 1)
        {
            this.AddListener<EventDefine.StartUpgradeRoom>(StartUpgradeRoomOrder);
        }
    }
    public void SetUpgradeIndex(int roomIndex)
    {
        upgradingRoomIndex = roomIndex;
        if (UpgradingRoomIndex == 0)
        {
            ActiveInstruction(true);
        }
        if (roomIndex != -1 && roomIndex < upgradeRoomOrder.Length)
        {
            ShowBox(true, roomIndex);
        }
        if (region == 1 && UpgradingRoomIndex > 1)
        {
            this.Dispatch<EventDefine.CanCallOrder>();
            this.Dispatch<EventDefine.OpenBoosterOffer>();
        }
    }
    public void StartUpgradeRoomOrder()
    {
        if (UpgradingRoomIndex > 0 || isUpgradeFlowStarted) return;
        isUpgradeFlowStarted = true;
        ActiveInstruction(true);
        UpgradingRoomIndex = 0;
        ShowBox(true, 0);
    }
    public void ShowNextUpgradeRoomBox()
    {
        ++UpgradingRoomIndex;
        ActiveInstruction(false);
        if (region == 1)
        {
            this.Dispatch<EventDefine.OnUpgradeRoomDone>();
        }
        ShowBox(true, UpgradingRoomIndex);
    }
    private void ShowBox(bool isRoom, int index)
    {
        if (isRoom && index < upgradeRoomOrder.Length)
        {
            upgradeRoomOrder[index].SetActive(true);
        }
    }
    private void ActiveInstruction(bool on)
    {
        if (instruction == null) return;
        if (on)
        {
            instruction.SetActive(on);
        }
        else
        {
            if (instruction.activeInHierarchy) instruction.SetActive(false);
        }
    }
    private void CheckOtherUpgradeable()
    {
        foreach (var upgrade in upgradeOtherOrder)
        {
            AUpgrade up = upgrade.GetComponent<AUpgrade>();
            if (!up.Completed && up.CheckCondition())
            {
                upgrade.SetActive(true);
            }
        }
    }
}
