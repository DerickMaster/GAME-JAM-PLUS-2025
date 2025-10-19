using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Collider))]
public class CollectibleResource : MonoBehaviour
{
    private enum State { Spawning, Moving, Waiting, Sinking }
    private State currentState = State.Spawning;

    [Header("Configuração de Movimento")]
    [SerializeField] private float travelDistance = 15f;
    [SerializeField] private float moveSpeed = 2f;

    [Header("Configuração de Coleta")]
    [SerializeField] private int plasticAmount = 5;
    [SerializeField] private int metalAmount = 5;

    private readonly float waitDuration = 5f;
    private float waitTimer;
    private Vector3 destination;
    private Animator animator;
    private bool hasBeenCollected = false; // Flag para evitar coleta dupla

    private readonly int isActiveHash = Animator.StringToHash("isActive");

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool(isActiveHash, true);
        destination = transform.position + (transform.forward * travelDistance);
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Moving:
                transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, destination) < 0.1f)
                {
                    currentState = State.Waiting;
                    waitTimer = 0f;
                }
                break;

            case State.Waiting:
                waitTimer += Time.deltaTime;
                if (waitTimer >= waitDuration)
                {
                    currentState = State.Sinking;
                    animator.SetBool(isActiveHash, false);
                }
                break;
        }
    }

    // --- FUNÇÕES CHAMADAS PELA ANIMAÇÃO ---
    public void OnAppearAnimationFinished() { currentState = State.Moving; }
    public void OnDisappearAnimationFinished() { Destroy(gameObject); }

    // --- A NOVA FUNÇÃO DE COLETA ---
    // Esta função pública será chamada pelo PlayerBuilder.
    public void Collect()
    {
        // Só pode ser coletado se estiver no estado de espera e não tiver sido coletado ainda.
        if (currentState != State.Waiting || hasBeenCollected) return;

        hasBeenCollected = true; // Impede chamadas múltiplas.
        Debug.Log($"Coletado via 'Use'! +{plasticAmount} Plástico, +{metalAmount} Metal.");
        ResourceManager.Instance.AddResources(plasticAmount, metalAmount);

        // Toca um som de coleta aqui, se desejar.
        Destroy(gameObject);
    }
}