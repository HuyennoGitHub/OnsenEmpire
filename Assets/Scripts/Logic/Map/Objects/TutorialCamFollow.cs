using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCamFollow : MonoBehaviour
{
    public Customer followCustomer;
    public PlayerInputCtrl player;
    private Camera mainCam;
    private Vector3 offset;
    private Vector3 upgradeAreaOffset = new(14.65f, 8.975f, -27.67f);
    public bool follow = false;
    private Vector3 bathroomView = new(-8.7f, 14.07f, -7.38f);
    private Vector3 saunaView = new(-2.36f, 9, 7.07f);
    private Vector3 storageView = new(8.3f, 9, -11.5f);
    private Vector3 staffViewOffset = new(0.33f, 9, -5);
    private Vector3 bathroomViewOffset = new(0.01f, 14.07f, -8.59f);
    private List<Transform> waitingToShow = new();
    private Vector3 playerOffset = new(0, 9, -4.88f);
    private bool isBusy;
    private int isLookingLeft;
    private Camera thisCam;
    private bool isMoving;
    private Transform target;
    private Vector3 oldPos;
    private void Awake()
    {
        mainCam = Camera.main;
        thisCam = GetComponent<Camera>();
    }
    private void Start()
    {
        if (player == null)
        {
            player = FindObjectOfType<PlayerInputCtrl>();
        }
        isBusy = false;
        StartCoroutine(ManageSecondaryCamera());
    }
    public void AddToShowList(Transform obj)
    {
        if (waitingToShow.Contains(obj)) return;
        waitingToShow.Add(obj);
    }
    IEnumerator ManageSecondaryCamera()
    {
        while (true)
        {
            while (waitingToShow.Count == 0 || isBusy)
            {
                yield return new WaitForEndOfFrame();
            }
            isBusy = true;
            Transform view = waitingToShow[0];
            waitingToShow.RemoveAt(0);
            MoveToUpgradeView(view);
            while (isBusy)
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }
    public void SetOffset()
    {
        offset = transform.position - followCustomer.transform.position;
        follow = true;
    }
    private void FixedUpdate()
    {
        if (followCustomer != null && follow)
        {
            player.Moveable = false;
            transform.position = followCustomer.transform.position + offset;
            if (followCustomer.transform.position.x < -8.7f)
            {
                follow = false;
                Invoke(nameof(ShowBathroomView), 5.2f);
            }
        }
    }
    private void ShowBathroomView()
    {
        followCustomer = null;
        transform.DOMove(bathroomView, 2f).SetEase(Ease.Linear).OnComplete(() => {
            Invoke(nameof(MoveToMainCam), 2.5f);
        });
    }
    private void MoveToMainCam()
    {
        transform.DORotateQuaternion(mainCam.transform.rotation, .5f).SetEase(Ease.Linear);
        transform.DOMove(mainCam.transform.position, .5f).SetEase(Ease.Linear).OnComplete(() => {
            FreeCam(true);
            player.Moveable = true;
        });
    }

    private void MoveToUpgradeView(Transform lookAt)
    {
        if (!thisCam.enabled)
        {
            transform.position = mainCam.transform.position;
            transform.rotation = mainCam.transform.rotation;
            thisCam.enabled = true;
        }
        player.Moveable = false;
        transform.DORotate(new Vector3(50, 0, 0), 1.5f).SetEase(Ease.Linear);
        transform.DOMove(lookAt.gameObject.GetComponent<RectTransform>().anchoredPosition3D + upgradeAreaOffset, 1.5f)
            .SetEase(Ease.Linear).OnComplete(() => {
                StartCoroutine(WaitingShowView(1));
            });
    }
    IEnumerator WaitingShowView(float waitingTime)
    {
        yield return new WaitForSeconds(waitingTime);
        ChangeView();
    }
    public void ChangeView()
    {
        if (waitingToShow.Count > 0)
        {
            FreeCam2();
        }
        else
        {
            MoveToMainCam();
        }
    }
    public void MoveToStaffUnlockView(Transform lookAt)
    {
        isBusy = true;
        thisCam.enabled = true;
        player.Moveable = false;

        transform.DOMove(lookAt.position + staffViewOffset, 1f).From(mainCam.transform.position)
            .SetEase(Ease.Linear).OnComplete(() => {
                StartCoroutine(WaitingShowView(1.5f));
            });
    }
    public void MoveToToiletView()
    {
        player.Moveable = false;
        isBusy = true;
        transform.position = mainCam.transform.position;
        thisCam.enabled = true;
        transform.DORotate(new Vector3(50, -45, 0), .5f);
        transform.DOMove(saunaView, 1f).SetEase(Ease.Linear).OnComplete(() => {
            Invoke(nameof(MoveToStorageView), 2f);
        });
    }
    private void MoveToStorageView()
    {
        transform.DORotate(new Vector3(50, 90, 0), .5f);
        transform.DOMove(storageView, 1f).SetEase(Ease.Linear).OnComplete(() => {
            Invoke(nameof(FreeCam2), 2f);
        });
    }

    private void FreeCam2()
    {
        isBusy = false;
    }
    public void FreeCam(bool returnMainCam)
    {
        isBusy = false;
        if (returnMainCam)
        {
            thisCam.enabled = false;
            follow = false;
        }
    }
    public void MoveToShowRoom(Room room)
    {
        isBusy = true;
        player.Moveable = false;
        transform.position = mainCam.transform.position;
        thisCam.enabled = true;
        transform.DORotate(new Vector3(50, 0, 0), .5f);
        transform.DOMove(room.center.position + bathroomViewOffset, 0.2f).SetEase(Ease.Linear).OnComplete(() => { });
    }
    public void MoveToShowStaff(Transform staff)
    {
        isBusy = true;
        player.Moveable = false;
        transform.position = mainCam.transform.position;
        thisCam.enabled = true;
        transform.DORotate(new Vector3(50, 0, 0), .5f);
        transform.DOMove(staff.position + staffViewOffset, 0.2f).SetEase(Ease.Linear).OnComplete(() => { });
    }
}
