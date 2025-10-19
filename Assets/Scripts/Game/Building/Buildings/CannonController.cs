using UnityEngine;

public class CannonController : MonoBehaviour
{
    public Animator animator;
    private readonly int fireHash = Animator.StringToHash("OnFire");

    void Awake()
    {

        // Prova de falha: Se n�o encontrar o Animator, nos avisa com um erro claro.
        if (animator == null)
        {
            Debug.LogError($"[CannonController] N�o conseguiu encontrar um Animator no prefab '{gameObject.name}' ou em seus filhos!", this);
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
        // Checagem de seguran�a para evitar erros se o Animator n�o foi encontrado.
        if (animator == null) return;

        Debug.Log($"<color=orange>[CannonController]</color> RECEBEU ORDEM DE ATIRAR!");
        animator.SetTrigger(fireHash);
        // Adicione som e part�culas aqui
    }
}