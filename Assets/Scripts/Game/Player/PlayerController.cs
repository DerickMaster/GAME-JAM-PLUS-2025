using UnityEngine;
using UnityEngine.InputSystem;

// Enum define os possíveis estados do jogador. É público para que outros scripts possam vê-lo.
public enum PlayerState { Gameplay, BuildMenu, PlacingObject }

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // --- Referências (Conectar no Inspector) ---
    [Header("Components")]
    [Tooltip("Arraste o objeto BuildDetector que tem o PlayerBuilder aqui.")]
    [SerializeField] private PlayerBuilder playerBuilder;
    [Tooltip("Arraste o UIManager que tem o BuildUIManager aqui.")]
    [SerializeField] private BuildUIManager buildUIManager;

    // --- Variáveis de Movimento ---
    [Header("Movement")]
    public float moveSpeed = 5.0f;
    public float rotationSpeed = 720f;

    // --- Variáveis de Pulo e Gravidade ---
    [Header("Jumping & Gravity")]
    public float jumpHeight = 1.0f;
    public float gravity = -9.81f;

    // --- Variáveis de Estado ---
    public PlayerState CurrentState { get; private set; } = PlayerState.Gameplay;

    // --- Componentes Internos e Variáveis Privadas ---
    private CharacterController controller;
    private PlayerAnimatorController animatorController;
    private Vector3 playerVelocity;
    private bool isGrounded;
    private Vector2 moveInput;

    // --- Propriedades Públicas ---
    public Vector2Int CurrentGridCell { get; private set; } = new Vector2Int(-1, -1);

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        // Procura "para baixo" na hierarquia para encontrar o script de animação no filho (Wrecker).
        animatorController = GetComponentInChildren<PlayerAnimatorController>();

        // Prova de falha para garantir que a referência da animação foi encontrada.
        if (animatorController == null)
        {
            Debug.LogError("O PlayerController não conseguiu encontrar o PlayerAnimatorController no objeto filho! Verifique se o script está anexado ao objeto Wrecker.", this);
        }
    }

    void Update()
    {
        // Se o estado for BuildMenu, o jogador não pode se mover.
        if (CurrentState == PlayerState.BuildMenu)
        {
            moveInput = Vector2.zero;
            // Mesmo parado, precisamos atualizar o Animator para que ele volte para a animação "Idle".
            if (animatorController != null)
            {
                animatorController.UpdateMovementParameters(0f);
            }
            return; // Impede que a lógica de movimento abaixo seja executada.
        }

        // A lógica de movimento é executada nos estados Gameplay e PlacingObject.
        isGrounded = controller.isGrounded;
        if (isGrounded && playerVelocity.y < 0) { playerVelocity.y = -2f; }

        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(moveDirection), rotationSpeed * Time.deltaTime);
        }

        // Comanda o Animator a cada frame com a velocidade atual.
        if (animatorController != null)
        {
            // Usamos moveInput.magnitude para ter uma resposta instantânea do Animator.
            animatorController.UpdateMovementParameters(moveInput.magnitude);
        }

        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    // Função pública para que outros scripts possam mudar o estado do jogador.
    public void SetState(PlayerState newState)
    {
        CurrentState = newState;
    }

    #region Funções de Input (O Cérebro do Controle)

    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();

        if (CurrentState == PlayerState.BuildMenu)
        {
            buildUIManager.Navigate(input);
        }
        else if (CurrentState == PlayerState.Gameplay || CurrentState == PlayerState.PlacingObject)
        {
            moveInput = input;
        }
    }

    public void OnJump(InputValue value)
    {
        if ((CurrentState == PlayerState.Gameplay || CurrentState == PlayerState.PlacingObject) && isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            if (animatorController != null) animatorController.TriggerJumpAnimation();
        }
    }

    // A tecla 'C' tem função dupla dependendo do estado.
    public void OnOpenBuildMenu(InputValue value)
    {
        switch (CurrentState)
        {
            case PlayerState.Gameplay:
                buildUIManager.OpenBuildMenu();
                break;
            case PlayerState.PlacingObject:
                playerBuilder.ConfirmBuild();
                break;
        }
    }

    // A tecla 'Z' tem função dupla dependendo do estado.
    public void OnCancel(InputValue value)
    {
        switch (CurrentState)
        {
            case PlayerState.BuildMenu:
                buildUIManager.CloseBuildMenu();
                break;
            case PlayerState.PlacingObject:
                playerBuilder.CancelBuildMode();
                break;
        }
    }

    // A tecla 'Enter' só funciona no menu.
    public void OnSubmit(InputValue value)
    {
        if (CurrentState == PlayerState.BuildMenu)
        {
            buildUIManager.SubmitSelection();
        }
    }

    // A tecla 'E' para coletar.
    public void OnCollect(InputValue value)
    {
        if (CurrentState == PlayerState.Gameplay)
        {
            if (animatorController != null) animatorController.TriggerCollectAnimation();
            // Lógica de coleta de recursos virá aqui no futuro.
        }
    }

    #endregion

    #region Detecção de Posição no Grid

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<GridCell>() != null)
            CurrentGridCell = other.GetComponent<GridCell>().coordinates;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<GridCell>() != null && other.GetComponent<GridCell>().coordinates == CurrentGridCell)
            CurrentGridCell = new Vector2Int(-1, -1);
    }

    #endregion
}