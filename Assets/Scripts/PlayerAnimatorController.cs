using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerAnimatorControllerNamespace
{
    public enum PlayerAnimLoopingStatus { Idle, Sprint, Falling, None }

    public class PlayerAnimatorController : MonoBehaviour
    {
        public PlayerAnimLoopingStatus currentPlayerAnimStatus { get; private set; }
        private Animator animator;
        [HideInInspector] public Vector3 deltaPos;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            animator.applyRootMotion = true;
            SetRigged(true);
            DeactiveLanding();
            SetLoopingAnimMode(PlayerAnimLoopingStatus.Idle);
        }

        public void SetLoopingAnimMode(PlayerAnimLoopingStatus mode)
        {
            currentPlayerAnimStatus = mode;
            animator.SetFloat("AnimMode", (int)currentPlayerAnimStatus);
        }

        public void ActiveLanding()
        {
            SetLoopingAnimMode(PlayerAnimLoopingStatus.None);
            animator.SetBool("Landing", true);
        }

        public void DeactiveLanding()
        {
            SetLoopingAnimMode(PlayerAnimLoopingStatus.Sprint);
            animator.SetBool("Landing", false);
        }

        public void SetRigged(bool isRigged)
        {
            animator.SetBool("Rigged", isRigged);
        }

        private void OnAnimatorMove()
        {
            Vector3 velocity = animator.deltaPosition;
            deltaPos = velocity;
        }
    }
}
