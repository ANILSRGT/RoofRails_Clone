using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    public enum PlayerAnimLoopingStatus { Sprint, FallingIdle, HangIdle }
    private PlayerAnimLoopingStatus currentPlayerAnimStatus;
    private Animator animator;
    [HideInInspector] public Vector3 deltaPos;

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.applyRootMotion = true;
        SetLoopingAnimMode(PlayerAnimLoopingStatus.Sprint);
    }

    public void SetLoopingAnimMode(PlayerAnimLoopingStatus mode)
    {
        currentPlayerAnimStatus = mode;
        animator.SetFloat("AnimMode", (int)currentPlayerAnimStatus);
    }

    private void OnAnimatorMove()
    {
        Vector3 velocity = animator.deltaPosition;
        deltaPos = velocity;
    }
}
