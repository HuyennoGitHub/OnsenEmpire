using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ACharacter : MonoBehaviour, IMoveable
{
    [SerializeField] float moveSpeed = 1;
    [SerializeField] Animator animator;

    public float MoveSpeed => moveSpeed;

    protected virtual void Start()
    {
        PlayIdleAnim();
    }

    public void SetMoveSpeed(float speed)
    {
        this.moveSpeed = speed;
    }

    public virtual void MoveTo(Vector3 target)
    {
        target.y = transform.position.y;
        PlayWalkingAnim();
    }

    protected virtual void PlayWalkingAnim()
    {
        if (animator == null) return;
        animator.Play("Walking", 0);
    }

    protected virtual void PlayIdleAnim()
    {
        if (animator == null) return;
        animator.Play("Idle", 0);
    }
}
