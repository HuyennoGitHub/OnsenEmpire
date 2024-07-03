using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IPS;

public class Receptionist : MonoBehaviour
{
    List<Customer> waitingCustomers = new();
    public Transform[] waitingPoss;
    public GameObject cashPrefab;
    public Transform putMoneyPos;
    public Animator animator;
    public ObjectData Info;
    List<Room> canUseRoom = new();
    [SerializeField] SortMoneyInContainer sort;
    public Image countdown;
    float taketime;
    public AudioSource audioSource;
    private Room specialRoom;
    private Reception reception;

    private void Start()
    {
        this.AddListener<EventDefine.OnHavingCustomer>(OnHavingCustomerComing);
        reception = FindObjectOfType<Reception>();
        for (int i = 0; i < waitingPoss.Length; i++)
        {
            this.Dispatch(new EventDefine.OnHavingEmptySlot { pos = waitingPoss[i].position, receptionist = this });
        }
        StartCoroutine(ManageReception());
    }
    public void Unlock()
    {
        Info.SetLevel(1);
        GameData.Instance.GetObject(Info.Id, 1).Unlock();
        UserData.Inventory.SaveItem(new ObjectData(Info.Id, ObjectType.Receptionist, Info.Level), true);
    }
    IEnumerator ManageReception()
    {
        while (true)
        {
            countdown.fillAmount = 0;
            while (canUseRoom.Count == 0) yield return new WaitForEndOfFrame();
            taketime = 0;
            while (taketime < 1.5f)
            {
                taketime += Time.deltaTime;
                countdown.fillAmount = taketime / 1.5f;
                yield return new WaitForEndOfFrame();
            }
            //animator.Play("Receive");
            if (canUseRoom.Count == 0) continue;
            if (SFX.Instance.SoundEnable) audioSource.Play();
            Customer customer = waitingCustomers[0];
            waitingCustomers.RemoveAt(0);
            Room checkInRoom = canUseRoom[0];
            if (checkInRoom == specialRoom)
            {
                customer.SetSpecialOrderCustomer();
                specialRoom = null;
                reception.specialOrderCustomer = customer;
            }
            canUseRoom.RemoveAt(0);
            customer.MoveToRoom(checkInRoom);
            checkInRoom.usedCustomer = customer;
            PayToCheckIn(checkInRoom);
            this.Dispatch(new EventDefine.OnHavingEmptySlot { pos = Vector3.zero, receptionist = this }); ;
            ReArrangeWaitingQueue();
        }
    }
    private void ReArrangeWaitingQueue()
    {
        for (int i = 0; i < waitingCustomers.Count; i++)
        {
            waitingCustomers[i].MoveTo(waitingPoss[i].position);
        }
    }
    private void OnHavingCustomerComing(EventDefine.OnHavingCustomer param)
    {
        if (param.receptionist == this)
            waitingCustomers.Add(param.customer);
    }

    public void PayToCheckIn(Room room)
    {
        var cash = Instantiate(cashPrefab, putMoneyPos);
        var money = cash.GetComponent<CashObject>();
        money.SetValue(room.RoomCost);
        cash.transform.position = waitingPoss[0].position;
        sort.Sort(cash, money);
    }
    public void AddRoomToList(Room room, bool isSpecialCustomer)
    {
        if (canUseRoom.Contains(room)) return;
        canUseRoom.Add(room);
        if (isSpecialCustomer)
        {
            specialRoom = room;
        }
    }
    public bool RemoveRoomToList(Room room)
    {
        if (!canUseRoom.Contains(room)) return false;
        canUseRoom.Remove(room);
        return true;
    }
}
