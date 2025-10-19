using UnityEngine;

public class CannonController : MonoBehaviour
{
    public Animator animator;
    private readonly int fireHash = Animator.StringToHash("OnFire");

    void Awake()
    {

        // Prova de falha: Se não encontrar o Animator, nos avisa com um erro claro.
        if (animator == null)
        {
            Debug.LogError($"[CannonController] Não conseguiu encontrar um Animator no prefab '{gameObject.name}' ou em seus filhos!", this);
        }
    }

    void Start()
    {
        if (ConstructionManager.Instance != null)
        {
            ConstructionManager.Instance.RegisterCannon(this);
        }
    }

    void OnDestroy()
    {
        if (ConstructionManager.Instance != null)
            ConstructionManager.Instance.UnregisterCannon(this);
    }

    public void Fire()
    {
        // Checagem de segurança para evitar erros se o Animator não foi encontrado.
        if (animator == null) return;

        Debug.Log($"<color=orange>[CannonController]</color> RECEBEU ORDEM DE ATIRAR!");
        animator.SetTrigger(fireHash);
        // Adicione som e partículas aqui
    }
}