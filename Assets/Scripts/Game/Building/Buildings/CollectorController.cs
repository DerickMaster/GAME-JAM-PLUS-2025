using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CollectorController : MonoBehaviour
{
    // --- NOVA VARI�VEL ---
    // Guarda a refer�ncia do nosso BoxCollider para us�-lo.
    private BoxCollider triggerArea;
    [SerializeField] Animator animator;

    // --- NOVO: M�todo Awake() ---
    // Usamos Awake() para garantir que a refer�ncia do collider seja pega antes do Start().
    private void Awake()
    {
        triggerArea = GetComponent<BoxCollider>();
    }

    // --- NOVO: M�todo Start() ---
    // Start() � chamado uma vez, no primeiro frame em que o objeto est� ativo.
    private void Start()
    {
        // Chama uma nova fun��o para checar a �rea imediatamente.
        CheckForInitialResources();
    }

    // A fun��o OnTriggerEnter continua a mesma, para pegar novos recursos que chegam.
    private void OnTriggerEnter(Collider other)
    {
        CollectibleResource resource = other.GetComponent<CollectibleResource>();
        if (resource != null)
        {
            animator.SetTrigger("OnP�ckUp");
            resource.SetPullTarget(transform);
        }
    }

    // --- NOVA FUN��O ---
    // Esta fun��o escaneia a �rea do BoxCollider em busca de recursos que j� est�o l�.
    private void CheckForInitialResources()
    {
        if (triggerArea == null) return;

        // 1. Calcula o centro e o tamanho da caixa de colis�o no espa�o do mundo.
        Vector3 boxCenter = transform.position + triggerArea.center;
        Vector3 halfExtents = Vector3.Scale(triggerArea.size, transform.lossyScale) / 2;

        // 2. Usa a f�sica do Unity para encontrar TODOS os colliders dentro dessa caixa.
        Collider[] collidersInside = Physics.OverlapBox(boxCenter, halfExtents, transform.rotation);

        // 3. Itera por cada collider que foi encontrado.
        foreach (Collider col in collidersInside)
        {
            // Tenta pegar o script de recurso em cada um.
            CollectibleResource resource = col.GetComponent<CollectibleResource>();
            if (resource != null)
            {
                // Se for um recurso, manda ele ser puxado.
                Debug.Log($"<color=cyan>[Collector]</color> Recurso '{col.name}' detectado na inicializa��o. Puxando...");
                resource.SetPullTarget(transform);
            }
        }
    }
}