using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class Bot : MonoBehaviour, IMoveable
{
    [SerializeField] float moveSpeed = 1;
    [SerializeField] protected Animator animator;
    [SerializeField] protected NavMeshAgent agent;

    public float MoveSpeed => moveSpeed;

    protected Vector3 destination;

    protected virtual void Start()
    {
        PlayIdleAnim();
    }

    public void SetMoveSpeed(float speed)
    {
        this.moveSpeed = speed;
    }
    public float CalculateDistance(Vector3 pos1, Vector3 pos2)
    {
        float xDiff = pos1.x - pos2.x;
        float zDiff = pos1.z - pos2.z;
        return Mathf.Sqrt(xDiff * xDiff + zDiff * zDiff);
    }
    public virtual void MoveTo(Vector3 target)
    {
        target.y = transform.position.y;
        //target.y = 0;
        destination = target;
        //PlayWalkingAnim();
        agent.speed = moveSpeed;
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

    private void FixedUpdate()
    {
        if (agent.speed > 0)
        {
            agent.SetDestination(destination);
        }
    }
}
