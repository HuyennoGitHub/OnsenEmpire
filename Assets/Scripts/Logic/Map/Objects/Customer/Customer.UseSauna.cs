using System.Collections;
using UnityEngine;

public partial class Customer
{
    public SaunaSlot usingSaunaSlot;
    public bool payedForSauna;

    public void GoToSauna(Vector3 watingSlot, Transform lookAtTarget)
    {
        MoveTo(watingSlot);
        isWaiting = true;
        WaitSaunaSlot(lookAtTarget);
    }
    public void WaitSaunaSlot(Transform lookAt)
    {
        StartCoroutine(OnWaitingSaunaSlot(lookAt));
    }
    IEnumerator OnWaitingSaunaSlot(Transform lookAt)
    {
        actionDoing = ActionDoing.GoingToSauna;
        while (isWaiting && !CheckReachDestination(destination))
        {
            yield return new WaitForEndOfFrame();
        }
        actionDoing = ActionDoing.Waiting;
        transform.LookAt(lookAt);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }
    public virtual void MoveToSauna(SaunaSlot saunaSlot, Transform lookAtCenter)
    {
        actionDoing = ActionDoing.GoingToSauna;
        usingSaunaSlot = saunaSlot;
        saunaSlot.Using = true;
        MoveTo(saunaSlot.UsePos);
        StartCoroutine(OnUsingSauna(lookAtCenter));
    }
    IEnumerator OnUsingSauna(Transform lookAt)
    {
        while (!CheckReachDestination(usingSaunaSlot.UsePos))
        {
            yield return new WaitForEndOfFrame();
        }
        actionDoing = ActionDoing.UsingService;
        isMoving = false;
        animator.Play("Idle");
        transform.LookAt(lookAt);
        //transform.rotation = Quaternion.Euler(0, 180, 0);
        animator.Play("StandSit");
        //model.localPosition = new Vector3(0, 0.124f, -0.405f);
        Invoke(nameof(GoHome), 5f);
        Invoke(nameof(ReturnModelBack), 5f);
    }
    private void ReturnModelBack()
    {
        model.localPosition = Vector3.zero;
        usingSaunaSlot.Using = false;
    }
}
