using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimatorController : MonoBehaviour
{
    private Animator animator;

    // Hashes for the Animator parameters
    private readonly int speedHash = Animator.StringToHash("speed");
    private readonly int isInteractingHash = Animator.StringToHash("isInteracting");
    private readonly int onJumpHash = Animator.StringToHash("OnJump");
    private readonly int onCollectHash = Animator.StringToHash("OnCollect");
    // --- THIS IS THE MISSING HASH ---
    private readonly int onUseHash = Animator.StringToHash("OnUse");

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // --- Public functions that receive commands ---

    public void UpdateMovementParameters(float currentSpeed)
    {
        animator.SetFloat(speedHash, currentSpeed);
    }

    public void TriggerJumpAnimation()
    {
        animator.SetTrigger(onJumpHash);
    }

    public void TriggerCollectAnimation()
    {
        animator.SetTrigger(onCollectHash);
    }

    // --- THIS IS THE MISSING FUNCTION ---
    public void TriggerUseAnimation()
    {
        animator.SetTrigger(onUseHash);
    }

    public void SetInteracting(bool isInteracting)
    {
        animator.SetBool(isInteractingHash, isInteracting);
    }
}