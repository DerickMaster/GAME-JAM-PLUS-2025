using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Collider))]
public class CollectibleResource : MonoBehaviour
{
    [Header("Configuração de Movimento")]
    [SerializeField] private float travelDistance = 15f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float pullSpeedMultiplier = 3f; // Velocidade com que é puxado pelo ímã

    [Header("Ciclo de Vida")]
    [SerializeField] private float lifeTime = 12f;

    [Header("Valores do Recurso")]
    [SerializeField] private int plasticAmount = 0;
    [SerializeField] private int metalAmount = 0;

    // --- NOVAS VARIÁVEIS DE CONTROLE ---
    private bool isBeingPulled = false;
    private Transform pullTarget;

    private Vector3 destination;
    private Animator animator;
    private bool hasBeenCollected = false;
    private bool isSinking = false;
    private float lifeTimer = 0f;

    private readonly int isActiveHash = Animator.StringToHash("isActive");

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool(isActiveHash, true);
        destination = transform.position + (transform.forward * travelDistance);
    }

    void Update()
    {
        if (isSinking || hasBeenCollected) return;

        // --- NOVA LÓGICA DE PRIORIDADE ---
        // Se estiver sendo puxado, essa lógica tem prioridade sobre todo o resto.
        if (isBeingPulled)
        {
            // Move o recurso em direção ao Collector, com velocidade aumentada.
            transform.position = Vector3.MoveTowards(transform.position, pullTarget.position, moveSpeed * pullSpeedMultiplier * Time.deltaTime);

            // Se chegou perto o suficiente do Collector, coleta.
            if (Vector3.Distance(transform.position, pullTarget.position) < 0.5f)
            {
                Collect();
            }
            return; // Impede que a lógica de ciclo de vida abaixo seja executada.
        }

        // --- LÓGICA DO CICLO DE VIDA NORMAL ---
        if (Vector3.Distance(transform.position, destination) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
        }

        lifeTimer += Time.deltaTime;
        if (lifeTimer >= lifeTime)
        {
            Sink();
        }
    }
    public void SetPullTarget(Transform target)
    {
        // Só pode ser capturado se não estiver já afundando ou sendo puxado.
        if (isSinking || isBeingPulled) return;

        isBeingPulled = true;
        pullTarget = target;
    }

    private void Sink()
    {
        isSinking = true;
        animator.SetBool(isActiveHash, false);
    }

    public void OnDisappearAnimationFinished()
    {
        Destroy(gameObject);
    }

    public void Collect()
    {
        if (hasBeenCollected) return;
        hasBeenCollected = true;
        ResourceManager.Instance.AddResources(plasticAmount, metalAmount);
        Destroy(gameObject);
    }
}