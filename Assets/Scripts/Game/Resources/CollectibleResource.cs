using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Collider))]
public class CollectibleResource : MonoBehaviour
{
    [Header("Configura��o de Movimento")]
    [Tooltip("A dist�ncia total que o recurso vai viajar.")]
    [SerializeField] private float travelDistance = 15f;
    [Tooltip("A velocidade de movimento do recurso.")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Ciclo de Vida")]
    [Tooltip("O tempo total em segundos que o recurso ficar� ativo antes de afundar.")]
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
        animator.SetBool(isActiveHash, true); // Aciona a anima��o "Appearing"

        // Calcula o destino final
        destination = transform.position + (transform.forward * travelDistance);
    }

    void Update()
    {
        // Se o objeto j� est� afundando ou foi coletado, n�o faz mais nada.
        if (isSinking || hasBeenCollected) return;

        // --- L�GICA DE MOVIMENTO CONT�NUO ---
        // Move o objeto em dire��o ao destino, mas para se a dist�ncia for alcan�ada.
        if (Vector3.Distance(transform.position, destination) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
        }

        // --- L�GICA DO TIMER DE VIDA ---
        lifeTimer += Time.deltaTime;
        if (lifeTimer >= lifeTime)
        {
            Sink();
        }
    }

    // A fun��o para afundar.
    private void Sink()
    {
        isSinking = true;
        animator.SetBool(isActiveHash, false); // Aciona a anima��o "Disappear".
    }

    // Esta fun��o DEVE ser chamada por um Animation Event no FINAL da anima��o "Disappear".
    public void OnDisappearAnimationFinished()
    {
        Destroy(gameObject);
    }

    // A fun��o de coleta p�blica, chamada pelo PlayerBuilder.
    public void Collect()
    {
        // S� pode ser coletado se n�o estiver afundando ou j� coletado.
        if (isSinking || hasBeenCollected) return;

        hasBeenCollected = true;
        Debug.Log($"Coletado! +{plasticAmount} Pl�stico, +{metalAmount} Metal.");
        ResourceManager.Instance.AddResources(plasticAmount, metalAmount);

        // Toca um som de coleta aqui, se desejar.
        Destroy(gameObject);
    }
}