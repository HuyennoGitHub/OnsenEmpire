using IPS;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class ExpandAreaCtrl : MonoBehaviour
{
    public GameObject[] lockedAreaCanvas;
    public GameObject[] unlockedAreaCanvas;
    public TextMeshProUGUI[] conditionLevelText;

    private void Start()
    {
        this.AddListener<EventDefine.OnUpLevel>(Unlockable);
        this.AddListener<EventDefine.OnExpandedArea>(HideAreaCanvas);
        foreach (var area in GameData.Instance.expandAreaStatuses)
        {
            if (area.StatusLock == StatusLockArea.WaitingForUnlock)
            {
                lockedAreaCanvas[area.Order].SetActive(false);
                unlockedAreaCanvas[area.Order].SetActive(true);
            }
            else if (area.StatusLock == StatusLockArea.Unlocked)
            {
                lockedAreaCanvas[area.Order].SetActive(false);
                unlockedAreaCanvas[area.Order].SetActive(false);
            }
            else
            {
                lockedAreaCanvas[area.Order].SetActive(true);
                unlockedAreaCanvas[area.Order].SetActive(false);
            }
            conditionLevelText[area.Order].text = area.ConditionLevel.ToString();
        }
    }
    private void Unlockable()
    {
        ExpandAreaStatus result = GameData.Instance.GetExpandableArea();
        if (result.IsNull) return;
        uint order = result.Order;
        lockedAreaCanvas[order].SetActive(false);
        unlockedAreaCanvas[order].SetActive(true);
        GameData.Instance.SetStatusLockArea(order, StatusLockArea.WaitingForUnlock, true);
    }
    private void HideAreaCanvas(EventDefine.OnExpandedArea param)
    {
        ExpandAreaStatus result = GameData.Instance.GetExpandAreaStatus(param.areaId);
        if (result.IsNull) return;
        uint order = result.Order;
        lockedAreaCanvas[order].SetActive(false);
        unlockedAreaCanvas[order].SetActive(false);
        GameData.Instance.SetStatusLockArea(order, StatusLockArea.Unlocked, true);
    }
}
