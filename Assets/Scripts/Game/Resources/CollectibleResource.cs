using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Collider))]
public class CollectibleResource : MonoBehaviour
{
    [Header("Configuração de Movimento")]
    [Tooltip("A distância total que o recurso vai viajar.")]
    [SerializeField] private float travelDistance = 15f;
    [Tooltip("A velocidade de movimento do recurso.")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Ciclo de Vida")]
    [Tooltip("O tempo total em segundos que o recurso ficará ativo antes de afundar.")]
    [SerializeField] private float lifeTime = 12f; // Ex: 7.5s de movimento + 4.5s "parado" no final

    [Header("Valores do Recurso")]
    [SerializeField] private int plasticAmount = 0;
    [SerializeField] private int metalAmount = 0;

    private Vector3 destination;
    private Animator animator;
    private bool hasBeenCollected = false;
    private bool isSinking = false;
    private float lifeTimer = 0f;

    private readonly int isActiveHash = Animator.StringToHash("isActive");

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool(isActiveHash, true); // Aciona a animação "Appearing"

        // Calcula o destino final
        destination = transform.position + (transform.forward * travelDistance);
    }

    void Update()
    {
        // Se o objeto já está afundando ou foi coletado, não faz mais nada.
        if (isSinking || hasBeenCollected) return;

        // --- LÓGICA DE MOVIMENTO CONTÍNUO ---
        // Move o objeto em direção ao destino, mas para se a distância for alcançada.
        if (Vector3.Distance(transform.position, destination) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
        }

        // --- LÓGICA DO TIMER DE VIDA ---
        lifeTimer += Time.deltaTime;
        if (lifeTimer >= lifeTime)
        {
            Sink();
        }
    }

    // A função para afundar.
    private void Sink()
    {
        isSinking = true;
        animator.SetBool(isActiveHash, false); // Aciona a animação "Disappear".
    }

    // Esta função DEVE ser chamada por um Animation Event no FINAL da animação "Disappear".
    public void OnDisappearAnimationFinished()
    {
        Destroy(gameObject);
    }

    // A função de coleta pública, chamada pelo PlayerBuilder.
    public void Collect()
    {
        // Só pode ser coletado se não estiver afundando ou já coletado.
        if (isSinking || hasBeenCollected) return;

        hasBeenCollected = true;
        Debug.Log($"Coletado! +{plasticAmount} Plástico, +{metalAmount} Metal.");
        ResourceManager.Instance.AddResources(plasticAmount, metalAmount);

        // Toca um som de coleta aqui, se desejar.
        Destroy(gameObject);
    }
}