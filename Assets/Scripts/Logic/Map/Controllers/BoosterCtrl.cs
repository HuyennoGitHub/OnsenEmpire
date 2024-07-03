using IPS;
using System;
using UnityEngine;

public class BoosterCtrl : MonoBehaviour
{
    [SerializeField] BoosterCollectable boosterMoneyCollectable;
    [SerializeField] BoosterCollectable boosterTransportCollectable;
    [SerializeField] float transportBoosterDis;
    [SerializeField] float moneyBoosterDis;
    [SerializeField] int bonusSpeedPercentage;
    [SerializeField] float coolDownTimeS;
    [SerializeField] float showTimeS;

    public GameObject area3;
    private bool openedBoosterOffer;
    private Transform player;
    private Vector3 transportBoosterPos;
    private Vector3 moneyBoosterPos;
    private void Awake()
    {
        player = FindObjectOfType<PlayerInputCtrl>().transform;
        this.AddListener<EventDefine.OpenBoosterOffer>(OpenBoosterMech);
    }
    private void Start()
    {
        this.AddListener<EventDefine.OrderToCallShowBoosterOffer>(Order);
    }
    private void OpenBoosterMech()
    {
        openedBoosterOffer = true;
        ShowOffer(BoosterType.Both);
    }
    private void Order(EventDefine.OrderToCallShowBoosterOffer param)
    {
        if (param.type == BoosterType.Transport)
        {
            Invoke(nameof(ShowOfferTransport), coolDownTimeS);
        }
        else if (param.type == BoosterType.Money)
        {
            Invoke(nameof(ShowOfferMoney), coolDownTimeS);
        }
    }
    private void ShowOfferTransport()
    {
        ShowOffer(BoosterType.Transport);
    }
    private void ShowOfferMoney()
    {
        ShowOffer(BoosterType.Money);
    }
    private void ShowOffer(BoosterType type)
    {
        if (!openedBoosterOffer) return;
        if (type == BoosterType.Money)
        {
            ShowMoneyOffer();
        }
        else if (type == BoosterType.Transport)
        {
            ShowTransportOffer();
        }
        else
        {
            ShowMoneyOffer();
            ShowTransportOffer();
        }
    }
    private void ShowMoneyOffer()
    {
        GameObject booster = boosterMoneyCollectable.gameObject;
        booster.transform.position = FindPositionToShowBooster(moneyBoosterDis, true);
        moneyBoosterPos = booster.transform.position;
        boosterMoneyCollectable.SetValue(UserData.Inventory.FindTheMostExpensiveUnlocked(), showTimeS);
        booster.SetActive(true);
    }
    public void ShowTransportOffer()
    {
        GameObject booster = boosterTransportCollectable.gameObject;
        booster.transform.position = FindPositionToShowBooster(transportBoosterDis, false);
        transportBoosterPos = booster.transform.position;
        boosterTransportCollectable.SetValue(bonusSpeedPercentage, showTimeS);
        booster.SetActive(true);
    }
    private Vector3 FindPositionToShowBooster(float dis, bool isMoneyBooster)
    {
        Vector3 pos = Vector3.zero;
        pos.x = 0.4f;
        if (player.position.z <= 37 && player.position.z >= 0) pos.z = player.position.z;
        else if (player.position.z > 37) pos.z = 37;
        else pos.z = 0;
        if (isMoneyBooster)
        {
            if (Mathf.Abs(pos.z - transportBoosterPos.z) < 4)
            {
                pos.z = transportBoosterPos.z + 4;
                if (pos.z > 37) pos.z -= 8;
            }
        } else
        {
            if (Mathf.Abs(pos.z - moneyBoosterPos.z) < 4)
            {
                pos.z = moneyBoosterPos.z + 4;
                if (pos.z > 37) pos.z -= 8;
            }
        }
        return pos;
    }
}
