using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ZombieAnimationController : MonoBehaviour
{
    private enum ZombieState
    {
        Idle,
        Move,
        Attack,
        Die
    }
    
    private static readonly int IsIdle = Animator.StringToHash("IsIdle");
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
    private static readonly int Hit = Animator.StringToHash("HitReaction");
    private static readonly int Die = Animator.StringToHash("Die");
    
    private Animator animator;
    
    private ZombieState currentState = ZombieState.Idle;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    // Set move speed in animator
    public void SetMoveSpeed(float speed)
    {
        animator.speed = speed;
    }
    
    private void SetState(ZombieState state)
    {
        ResetAllStates();
        switch (state)
        {
            case ZombieState.Idle:
                animator.SetBool(IsIdle, true);
                break;
            case ZombieState.Move:
                animator.SetBool(IsMoving, true);
                break;
            case ZombieState.Attack:
                animator.SetBool(IsAttacking, true);
                break;
            case ZombieState.Die:
                animator.SetTrigger(Die);
                break;
        }
    }
    
    public void SetIdle()
    {
        if (currentState == ZombieState.Idle) return;
        currentState = ZombieState.Idle;
        SetState(ZombieState.Idle);
    }
    
    public void SetMoving()
    {
        if (currentState == ZombieState.Move) return;
        currentState = ZombieState.Move;
        SetState(ZombieState.Move);
    }
    
    public void SetAttacking()
    {
        if (currentState == ZombieState.Attack) return;
        currentState = ZombieState.Attack;
        SetState(ZombieState.Attack);
    }
    
    public void SetHitReaction()
    {
        animator.SetTrigger(Hit);
    }
    
    public void SetDying()
    {
        if (currentState == ZombieState.Die) return;
        currentState = ZombieState.Die;
        SetState(ZombieState.Die);
    }

    // Resets all bool parameters to ensure only one state is active
    private void ResetAllStates()
    {
        animator.SetBool(IsIdle, false);
        animator.SetBool(IsMoving, false);
        animator.SetBool(IsAttacking, false);
    }
}