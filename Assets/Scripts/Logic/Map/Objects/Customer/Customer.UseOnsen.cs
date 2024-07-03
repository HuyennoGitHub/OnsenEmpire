using IPS;
using System.Collections;
using UnityEngine;

public partial class Customer
{
    [SerializeField] GameObject alcoholGO;
    public Room usingRoom;
    public bool haveSpecialOrder;
    private bool isSpecialOrderServed;
    public RefillObjectType orderType;
    public BoxCollider servedCollider;
    public GameObject orderImgGO;

    public void SetSpecialOrderCustomer()
    {
        haveSpecialOrder = true;
        isSpecialOrderServed = false;
        orderType = RefillObjectType.Sake;
        servedCollider.enabled = true;
    }
    public virtual void MoveToRoom(Room room)
    {
        isWaiting = false;
        animator.Play("Walking");
        actionDoing = ActionDoing.GoingToOnsen;
        isMoving = true;
        usingRoom = room;
        room.Using = true;
        if (haveSpecialOrder)
        {
            MoveTo(room.center.position);
            StartCoroutine(WaitingForServing(room));
        }
        else MoveToChangeClothes(room);
    }
    private void MoveToChangeClothes(Room room)
    {
        actionDoing = ActionDoing.GoingToChangeClothes;
        MoveTo(room.center.position);
        room.usedCustomer = this;
        DisableInteract();
        StartCoroutine(OnGoingToChangeClothes());
    }
    public void MoveToOnsenUsePos()
    {
        MoveTo(usingRoom.UsePos);
        actionDoing = ActionDoing.GoingToOnsen;
    }
    IEnumerator OnGoingToChangeClothes()
    {
        while (!CheckReachDestination(destination))
        {
            yield return new WaitForEndOfFrame();
        }
        ChangeClothes();
        MoveToOnsenUsePos();
    }
    IEnumerator WaitingForServing(Room room)
    {
        while (!CheckReachDestination(room.center.position))
        {
            yield return new WaitForEndOfFrame();
        }
        actionDoing = ActionDoing.CallingOrder;
        isMoving = false;
        transform.rotation = Quaternion.Euler(0, 180, 0);
        animator.Play("Call");
        orderImgGO.SetActive(true);
        this.Dispatch<EventDefine.OrderingSake>();
        while (!isSpecialOrderServed)
        {
            yield return new WaitForEndOfFrame();
        }
        actionDoing = ActionDoing.UsingService;
        this.Dispatch(new EventDefine.RewardSakura { sakura = 1 });
        alcoholGO.SetActive(true);
        orderImgGO.SetActive(false);
        animator.Play("Drink");
        servedCollider.enabled = false;
        yield return new WaitForSeconds(5.3f);
        alcoholGO.SetActive(false);
        MoveTo(room.UsePos);
        isMoving = true;
    }
    public virtual void MoveOutOfRoom()
    {
        animator.Play("SitStand");
        usingRoom.SetDirty();
        usingRoom.Using = false;
        DropCash(usingRoom.RoomTip);
        //GoHome();
        FindNextService();
    }
    IEnumerator WaitForReachingOnsen()
    {
        while (!CheckReachDestination(usingRoom.UsePos))
        {
            yield return new WaitForEndOfFrame();
        }
        //Invoke(nameof(FindNextService), 5f);
        animator.Play("StandSit");
        usingRoom.TurnOnFogVFX();
        actionDoing = ActionDoing.UsingService;
        transform.LookAt(usingRoom.center);
        //transform.rotation = Quaternion.Euler(0, -180, 0);
    }
}
