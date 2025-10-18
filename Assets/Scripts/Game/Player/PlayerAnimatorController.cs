using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimatorController : MonoBehaviour
{
    private Animator animator;

    // Hashes são uma forma otimizada de se referir aos parâmetros do Animator.
    private readonly int speedHash = Animator.StringToHash("speed");
    private readonly int isInteractingHash = Animator.StringToHash("isInteracting");
    private readonly int onJumpHash = Animator.StringToHash("OnJump");
    private readonly int onCollectHash = Animator.StringToHash("OnCollect");

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // --- FUNÇÕES PÚBLICAS QUE RECEBEM ORDENS ---

    // O Cérebro (PlayerController) vai chamar esta função a cada frame.
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