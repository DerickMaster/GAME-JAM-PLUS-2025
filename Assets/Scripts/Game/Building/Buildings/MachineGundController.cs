using UnityEngine;

public class MachineGunController : MonoBehaviour
{
    public Animator animator;
    private readonly int fireHash = Animator.StringToHash("OnFire");

    void Awake() {}
    void Start() { ConstructionManager.Instance.RegisterMachineGun(this); }
    void OnDestroy() { if (ConstructionManager.Instance != null) ConstructionManager.Instance.UnregisterMachineGun(this); }

    public void Fire()
    {
        animator.SetTrigger(fireHash);
        // Aqui você pode adicionar um som de rajada e efeitos de partícula.
    }
}