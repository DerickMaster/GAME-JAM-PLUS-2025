using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CannonController : MonoBehaviour
{
    private Animator animator;
    private readonly int fireHash = Animator.StringToHash("OnFire");

    void Awake() { animator = GetComponent<Animator>(); }
    void Start() { ConstructionManager.Instance.RegisterCannon(this); }
    void OnDestroy() { if (ConstructionManager.Instance != null) ConstructionManager.Instance.UnregisterCannon(this); }

    public void Fire()
    {
        animator.SetTrigger(fireHash);
        // Aqui você pode adicionar um som de tiro e efeitos de partícula.
    }
}