using System;
using System.Collections;
using Cinemachine;
using IPS;
using UnityEngine;


public class PlayerInputCtrl : MonoBehaviour
{
    public enum Side { Left, Right, Ahead }
    [SerializeField] Transform model;
    [SerializeField] Rigidbody body;
    [SerializeField] float moveSpeed = 1.0f;
    [SerializeField] FloatingJoystick joystick;
    [SerializeField] Animator animator;
    [SerializeField] GameObject dust;

    [SerializeField] CinemachineVirtualCamera inRoomCam;
    [SerializeField] CinemachineVirtualCamera outRoomCam;

    [SerializeField] private MapCtrl mapCtrl;
    [SerializeField] private GameObject transport;

    public bool inRoom;
    public Side viewSide;

    private RefillCollector myCollector;
    private float counter;
    private Vector3 direction;
    private float initSpeed;
    private float usingRemainTime;
    private bool moveable;
    public bool Moveable
    {
        get { return moveable; }
        set
        {
            moveable = value;
            if (!moveable)
            {
                direction = Vector3.zero;
                body.velocity = Vector3.zero;
            }
        }
    }
    public bool Running { get; private set; }
    public bool IsMoving => direction != Vector3.zero;

    private void Start()
    {
        this.AddListener<EventDefine.OnStartGame>(OnStartGame);
        myCollector = GetComponent<RefillCollector>();
        counter = 0;
        viewSide = Side.Ahead;
        initSpeed = moveSpeed;
        dust.transform.parent = null;
    }

    private void OnStartGame()
    {
        Running = true;
        Moveable = true;
    }

    private void FixedUpdate()
    {
        if (!Running || !Moveable) return;

        MoveByJoyStick();
    }


    private void MoveByJoyStick()
    {
        if (viewSide == Side.Ahead)
        {
            direction = new Vector3(joystick.Horizontal, 0, joystick.Vertical);
        }
        else if (viewSide == Side.Left)
        {
            Vector3 input = new(joystick.Horizontal, 0, joystick.Vertical);
            direction = Quaternion.Euler(0, -45f, 0) * input;
        }
        else if (viewSide == Side.Right)
        {
            Vector3 input = new(joystick.Horizontal, 0, joystick.Vertical);
            direction = Quaternion.Euler(0, 45f, 0) * input;
        }
        if (!IsMoving)
        {
            if (myCollector.CollectedTotal == 0)
            {
                if (initSpeed == moveSpeed) animator.Play("Idle");
                else animator.Play("StandOnTransport");
            }
            else animator.Play("Grab");
            HideDustVFX();
        }
        else
        {
            if (myCollector.CollectedTotal == 0)
            {
                if (initSpeed == moveSpeed) animator.Play("Running");
                else animator.Play("StandOnTransport");
            }
            else
            {
                if (initSpeed == moveSpeed) animator.Play("GoGrab");
                else animator.Play("Grab");
            }
            counter += Time.fixedDeltaTime;
            if (counter > 0.8f)
            {
                counter = 0;
                ShowDustVFX();
            }
        }

        body.velocity = 100f * moveSpeed * Time.fixedDeltaTime * direction;

        if (direction != Vector3.zero)
        {
            model.localRotation = Quaternion.AngleAxis((Mathf.Atan2(body.velocity.x, body.velocity.z) * Mathf.Rad2Deg), Vector3.up);
            if (transport.activeInHierarchy)
            {
                transport.transform.rotation = Quaternion.AngleAxis((Mathf.Atan2(body.velocity.x, body.velocity.z) * Mathf.Rad2Deg), Vector3.up);
            }
        }
    }
    public void SpeedUp(float speed, float time)
    {
        usingRemainTime += time * 60;
        UICtrl.Instance.Get<IngamePanel>().ShowClock(time * 60);
        initSpeed = moveSpeed;
        moveSpeed = initSpeed * (1 + speed);
        animator.Play("StandOnTransport");
        transport.SetActive(true);
        //Invoke(nameof(ReturnInitSpeed), time * 60);
        StopCoroutine(CountDownUsingVehicle());
        StartCoroutine(CountDownUsingVehicle());
    }
    private void ReturnInitSpeed()
    {
        moveSpeed = initSpeed;
        animator.Play("Running");
        transport.SetActive(false);
    }
    IEnumerator CountDownUsingVehicle()
    {
        while (usingRemainTime > 0)
        {
            yield return new WaitForEndOfFrame();
            usingRemainTime -= Time.deltaTime;
        }
        ReturnInitSpeed();
    }
    private void ShowDustVFX()
    {
        dust.transform.position = model.position;
        dust.SetActive(true);
        Invoke(nameof(HideDustVFX), 0.8f);
    }
    private void HideDustVFX()
    {
        dust.SetActive(false);
    }
    protected void OnTriggerExit(Collider other)
    {
        if (other == null) return;
        if (other.CompareTag("In"))
        {
            inRoom = false;
            viewSide = Side.Ahead;
            outRoomCam.enabled = true;
            inRoomCam.enabled = false;
            this.Dispatch<EventDefine.OnExitRoom>();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;
        if (other.CompareTag("In"))
        {
            inRoom = true;
            Room room = other.transform.parent.parent.GetComponent<Room>();
            if (room != null)
            {
                viewSide = room.LookAtTheLeftSide ? Side.Left : Side.Right;
            }
            else
            {
                Sauna sauna = other.transform.parent.GetComponent<Sauna>();
                if (sauna != null)
                {
                    viewSide = sauna.LookAtTheLeftSide ? Side.Left : Side.Right;
                }
                else
                {
                    viewSide = Side.Right;
                }
            }
            if (viewSide == Side.Left)
            {
                inRoomCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = new Vector3(3.95f, 6.5f, -3.95f);
                inRoomCam.transform.rotation = Quaternion.Euler(50, -45, 0);
            }
            else if (viewSide == Side.Right)
            {
                inRoomCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = new Vector3(-3.95f, 6.5f, -3.95f);
                inRoomCam.transform.rotation = Quaternion.Euler(50, 45, 0);
            }
            inRoomCam.enabled = true;
            outRoomCam.enabled = false;
            this.Dispatch(new EventDefine.OnEnterRoom { leftSideRoom = viewSide == Side.Left });
        }
    }
}
