using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CollectorController : MonoBehaviour
{
    // --- NOVA VARIÁVEL ---
    // Guarda a referência do nosso BoxCollider para usá-lo.
    private BoxCollider triggerArea;
    [SerializeField] Animator animator;

    // --- NOVO: Método Awake() ---
    // Usamos Awake() para garantir que a referência do collider seja pega antes do Start().
    private void Awake()
    {
        triggerArea = GetComponent<BoxCollider>();
    }

    // --- NOVO: Método Start() ---
    // Start() é chamado uma vez, no primeiro frame em que o objeto está ativo.
    private void Start()
    {
        // Chama uma nova função para checar a área imediatamente.
        CheckForInitialResources();
    }

    // A função OnTriggerEnter continua a mesma, para pegar novos recursos que chegam.
    private void OnTriggerEnter(Collider other)
    {
        CollectibleResource resource = other.GetComponent<CollectibleResource>();
        if (resource != null)
        {
            animator.SetTrigger("OnPìckUp");
            resource.SetPullTarget(transform);
        }
    }

    // --- NOVA FUNÇÃO ---
    // Esta função escaneia a área do BoxCollider em busca de recursos que já estão lá.
    private void CheckForInitialResources()
    {
        if (triggerArea == null) return;

        // 1. Calcula o centro e o tamanho da caixa de colisão no espaço do mundo.
        Vector3 boxCenter = transform.position + triggerArea.center;
        Vector3 halfExtents = Vector3.Scale(triggerArea.size, transform.lossyScale) / 2;

        // 2. Usa a física do Unity para encontrar TODOS os colliders dentro dessa caixa.
        Collider[] collidersInside = Physics.OverlapBox(boxCenter, halfExtents, transform.rotation);

        // 3. Itera por cada collider que foi encontrado.
        foreach (Collider col in collidersInside)
        {
            // Tenta pegar o script de recurso em cada um.
            CollectibleResource resource = col.GetComponent<CollectibleResource>();
            if (resource != null)
            {
                // Se for um recurso, manda ele ser puxado.
                Debug.Log($"<color=cyan>[Collector]</color> Recurso '{col.name}' detectado na inicialização. Puxando...");
                resource.SetPullTarget(transform);
            }
        }
    }
}