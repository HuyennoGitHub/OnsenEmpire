using IPS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Service { Sauna, Restaurant, None }
public partial class MapCtrl : MonoBehaviour
{
    List<Service> servableServices = new List<Service>();

    //public TutorialCamFollow tutCam;
    public bool IsTutorialCleanroom => UserData.Inventory.isTutorial;

    public List<EventDefine.NeedRefillEvent> needRefills = new();
    [ContextMenu("BindObject")]
    private void BindObject()
    {
        if (areas == null || areas.Length == 0) areas = FindObjectsOfType<Area>(true);
        restaurant = FindObjectOfType<Restaurant>(true);
    }
    private void Awake()
    {
        this.AddListener<EventDefine.NeedRefillEvent>(RefillResponse);
        GameData.Instance.LoadExpandAreaStatus();
    }
    public void AddServableService(Service service)
    {
        if (servableServices.Contains(service)) return;
        servableServices.Add(service);
        if (service == Service.Sauna && !storage.gameObject.activeInHierarchy)
        {
            storage.gameObject.SetActive(true);
        }
    }
    public void RefillResponse(EventDefine.NeedRefillEvent param)
    {
        storage.AddNeedRefill(param);
    }
}
