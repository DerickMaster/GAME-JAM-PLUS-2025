using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimatorController : MonoBehaviour
{
    private Animator animator;

    // Hashes s�o uma forma otimizada de se referir aos par�metros do Animator.
    private readonly int speedHash = Animator.StringToHash("speed");
    private readonly int isInteractingHash = Animator.StringToHash("isInteracting");
    private readonly int onJumpHash = Animator.StringToHash("OnJump");
    private readonly int onCollectHash = Animator.StringToHash("OnCollect");

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // --- FUN��ES P�BLICAS QUE RECEBEM ORDENS ---

    // O C�rebro (PlayerController) vai chamar esta fun��o a cada frame.
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

    public void SetInteracting(bool isInteracting)
    {
        animator.SetBool(isInteractingHash, isInteracting);
    }
}