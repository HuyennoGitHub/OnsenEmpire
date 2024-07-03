using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using IPS;

public class Reception : MonoBehaviour
{
    public Transform[] waitingPoss;
    public Transform putMoneyPos;
    public Customer specialOrderCustomer;
    public bool isAuto;
    public Image countdown;
    public AudioSource audioSource;
    public GameObject cashPrefab;
    public GameObject sakeTable;

    [SerializeField] private Receptionist[] receptionists;
    [SerializeField] int servedAmount;
    [SerializeField] SortMoneyInContainer sort;
    [SerializeField] BoxCollider interactBoxCollider;
    [SerializeField] GameObject callCustomerArea;
    [SerializeField] private UpgradeCtrl upgradeCtrl;

    List<Customer> waitingCustomers = new List<Customer>();
    List<Room> canUseRoom = new List<Room>();
    int servedCount;
    float taketime;

    private bool isInteracting;
    private bool isOpenSpecialOrder;
    private MapCtrl mapCtrl;
    private Queue<Receptionist> receptionistQueue;


    [Header("Tutorial")]
    public TutorialCamFollow tutorialCam;
    [SerializeField] GameObject instruction;
    private bool is1stCustomer;
    public bool CanCallSpecialCustomer
    {
        get
        {
            return isOpenSpecialOrder;
        }
        private set
        {
            isOpenSpecialOrder = value;
        }
    }
    public bool IsInteractEnable => interactBoxCollider.enabled;

    private void Awake()
    {
        this.AddListener<EventDefine.CanUseEvent>(CallCustomer, false);
        this.AddListener<EventDefine.CanCallOrder>(OpenSpecialCustomerMech, false);

    }
    private void Start()
    {
        receptionists = FindObjectsOfType<Receptionist>(true);
        var inventory = UserData.Inventory;
        foreach (var item in inventory.unlocked)
        {
            foreach (var receptionist in receptionists)
            {
                if (receptionist.Info.Id.Equals(item.Id))
                {
                    receptionist.gameObject.SetActive(true);
                    GameData.Instance.GetObject(receptionist.Info.Id, 1).Unlock();
                    receptionist.Info.SetLevel(1);
                    break;
                }
            }
        }
        receptionists = FindObjectsOfType<Receptionist>();
        receptionistQueue = new Queue<Receptionist>();
        if (receptionists.Length == 0)
        {
            this.AddListener<EventDefine.OnStartGame>(OnStartGame, false);
        }
        else
        {
            foreach (var receptionist in receptionists)
            {
                receptionistQueue.Enqueue(receptionist);
            }
            isAuto = true;
        }
        if (isAuto)
        {
            AutoCall();
        }
        else
        {
            this.AddListener<EventDefine.OnHavingCustomer>(OnHavingCustomerComing, false);
        }
        upgradeCtrl.enabled = true;
        sakeTable.SetActive(true);
    }
    public void OnStartGame()
    {
        servedCount = 0;
        mapCtrl = FindObjectOfType<MapCtrl>();
        is1stCustomer = true;
        for (int i = 0; i < waitingPoss.Length; i++)
        {
            this.Dispatch(new EventDefine.OnHavingEmptySlot { pos = waitingPoss[i].position, receptionist = null });
        }
    }
    private void OnHavingCustomerComing(EventDefine.OnHavingCustomer param)
    {
        if (param.receptionist != null) return;
        waitingCustomers.Add(param.customer);
    }
    private void ReArrangeWaitingQueue()
    {
        for (int i = 0; i < waitingCustomers.Count; i++)
        {
            waitingCustomers[i].MoveTo(waitingPoss[i].position);
        }
    }
    public void StartCall()
    {
        if (isInteracting || isAuto) return;
        isInteracting = true;
    }
    public void OnCalling()
    {
        if (!isInteracting || isAuto) return;
        if (taketime < 1)
        {
            taketime += Time.fixedDeltaTime;
            countdown.fillAmount = taketime;
        }
        if (taketime >= 1)
        {
            if (SFX.Instance.SoundEnable) audioSource.Play();
            SFX.Instance.Vibrate();
            Customer customer = waitingCustomers[0];
            waitingCustomers.RemoveAt(0);
            if (mapCtrl.IsTutorialCleanroom && is1stCustomer)
            {
                if (instruction != null) instruction.SetActive(false);
                is1stCustomer = false;
                tutorialCam.followCustomer = customer;
                tutorialCam.transform.position = Camera.main.transform.position;
                tutorialCam.GetComponent<Camera>().enabled = true;
                tutorialCam.transform.DOMove(customer.transform.position + new Vector3(0, 6.5f, -4), .2f).SetEase(Ease.Linear).OnComplete(() => { tutorialCam.SetOffset(); });
            }
            Room checkInRoom = canUseRoom[0];
            canUseRoom.RemoveAt(0);
            if ((specialOrderCustomer == null || specialOrderCustomer.IsServed) && CanCallSpecialCustomer) servedCount++;
            if (servedCount == servedAmount)
            {
                customer.SetSpecialOrderCustomer();
                specialOrderCustomer = customer;
                servedCount = 0;
            }
            customer.MoveToRoom(checkInRoom);
            checkInRoom.usedCustomer = customer;
            PayToCheckIn(checkInRoom);
            this.Dispatch(new EventDefine.OnHavingEmptySlot { pos = Vector3.zero });
            ReArrangeWaitingQueue();
            EndCall();
        }
    }
    public void EndCall()
    {
        if (!isInteracting) return;
        isInteracting = false;
        if (canUseRoom.Count == 0)
        {
            interactBoxCollider.enabled = false;
        }
        if (taketime >= 1f)
        {
            taketime = 0;
            countdown.fillAmount = 0;
            if (canUseRoom.Count > 0)
            {
                StartCall();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        StartCall();
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        EndCall();
    }
    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        OnCalling();
    }
    public void CallCustomer(EventDefine.CanUseEvent param)
    {
        if (mapCtrl == null) mapCtrl = FindObjectOfType<MapCtrl>();
        if (canUseRoom.Contains(param.room)) return;
        canUseRoom.Add(param.room);
        if (!isAuto)
        {
            interactBoxCollider.enabled = true;
            if (mapCtrl.IsTutorialCleanroom && is1stCustomer && instruction != null)
            {
                instruction.SetActive(true);
            }
        }
    }
    public void OpenSpecialCustomerMech()
    {
        CanCallSpecialCustomer = true;
    }
    public void PayToCheckIn(Room room)
    {
        var cash = Instantiate(cashPrefab, putMoneyPos);
        var money = cash.GetComponent<CashObject>();
        money.SetValue(room.RoomCost);
        cash.transform.position = waitingPoss[0].position;
        sort.Sort(cash, money);
    }
    public void AutoCall()
    {
        callCustomerArea.SetActive(false);
        interactBoxCollider.enabled = false;
        isAuto = true;
        while (waitingCustomers.Count > 0)
        {
            Customer customer = waitingCustomers[0];
            waitingCustomers.Remove(customer);
            Destroy(customer.gameObject);
        }
        StartCoroutine(ReceptionManage());
    }
    public void HireMoreReceptionist()
    {
        receptionists = FindObjectsOfType<Receptionist>();
        receptionistQueue.Clear();
        foreach (var recep in receptionists)
        {
            recep.Unlock();
            receptionistQueue.Enqueue(recep);
        }
        if (receptionists.Length == 1)
        {
            tutorialCam.MoveToStaffUnlockView(receptionists[0].transform);
        }
        if (!isAuto)
        {
            AutoCall();
        }
    }
    IEnumerator ReceptionManage()
    {
        while (true)
        {
            while (canUseRoom.Count == 0)
            {
                yield return new WaitForEndOfFrame();
            }
            Receptionist recep = receptionistQueue.Dequeue();
            Room room = canUseRoom[0];
            if ((specialOrderCustomer == null || specialOrderCustomer.IsServed) && CanCallSpecialCustomer) servedCount++;
            if (servedCount == servedAmount)
            {
                recep.AddRoomToList(room, true);
                servedCount = 0;
            }
            else recep.AddRoomToList(room, false);
            canUseRoom.Remove(room);
            receptionistQueue.Enqueue(recep);
            yield return new WaitForEndOfFrame();
        }
    }
    public void DeleteRoomForVIP(Room room)
    {
        if (canUseRoom.Contains(room))
        {
            canUseRoom.Remove(room);
        }
        else
        {
            foreach (var recep in receptionists)
            {
                if (recep.RemoveRoomToList(room)) break;
            }
        }
    }
}
