using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimatorController : MonoBehaviour
{
    private Animator animator;

    private readonly int speedHash = Animator.StringToHash("speed");
    private readonly int isInteractingHash = Animator.StringToHash("isInteracting");
    private readonly int onJumpHash = Animator.StringToHash("OnJump");
    private readonly int onCollectHash = Animator.StringToHash("OnCollect"); // Este � o da anima��o de coletar do ch�o que j� t�nhamos
    private readonly int onUseHash = Animator.StringToHash("OnUse");

    // --- NOVA LINHA ---
    private readonly int onCollectingHash = Animator.StringToHash("OnCollecting"); // Este � para coletar o lixo flutuante

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void UpdateMovementParameters(float currentSpeed) { animator.SetFloat(speedHash, currentSpeed); }
    public void TriggerJumpAnimation() { animator.SetTrigger(onJumpHash); }
    public void TriggerCollectAnimation() { animator.SetTrigger(onCollectHash); }
    public void TriggerUseAnimation() { animator.SetTrigger(onUseHash); }
    public void SetInteracting(bool isInteracting) { animator.SetBool(isInteractingHash, isInteracting); }

    // --- NOVA FUN��O ---
    public void TriggerCollectingAnimation()
    {
        animator.SetTrigger(onCollectingHash);
    }
}