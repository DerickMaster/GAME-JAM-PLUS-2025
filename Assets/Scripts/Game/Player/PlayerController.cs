using UnityEngine;
using UnityEngine.InputSystem;

// Enum define os possíveis estados do jogador. É público para que outros scripts possam vê-lo.
public enum PlayerState { Gameplay, BuildMenu, PlacingObject, Dismantling }

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
        animatorController = GetComponentInChildren<PlayerAnimatorController>();

        if (animatorController == null)
        {
            Debug.LogError("O PlayerController não conseguiu encontrar o PlayerAnimatorController no objeto filho! Verifique se o script está anexado ao objeto Wrecker.", this);
        }
    }

    void Update()
    {
        // O jogador fica parado tanto no menu quanto ao desmantelar.
        if (CurrentState == PlayerState.BuildMenu || CurrentState == PlayerState.Dismantling)
        {
            moveInput = Vector2.zero;
            if (animatorController != null)
            {
                animatorController.UpdateMovementParameters(0f);
            }
            return;
        }

        isGrounded = controller.isGrounded;
        if (isGrounded && playerVelocity.y < 0) { playerVelocity.y = -2f; }

        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(moveDirection), rotationSpeed * Time.deltaTime);
        }

        if (animatorController != null)
        {
            animatorController.UpdateMovementParameters(moveInput.magnitude);
        }

        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    public void SetState(PlayerState newState)
    {
        CurrentState = newState;
    }

    #region Funções de Input (O Cérebro do Controle)

    public void OnMove(InputValue value)
    {
        if (CurrentState == PlayerState.Dismantling)
        {
            playerBuilder.StopDismantling();
            return;
        }

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

    public void OnOpenBuildMenu(InputValue value) // Tecla 'C'
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

    public void OnDismantle(InputValue value) // Tecla 'Z'
    {
        if (CurrentState == PlayerState.Gameplay && value.isPressed)
        {
            playerBuilder.StartDismantling();
        }
        else if (CurrentState == PlayerState.Dismantling && !value.isPressed)
        {
            playerBuilder.StopDismantling();
        }
    }

    public void OnUse(InputValue value) // Tecla 'X'
    {
        if (CurrentState == PlayerState.Gameplay)
        {
            // A variável agora é do tipo InteractionType.
            InteractionType interactionType = playerBuilder.TryUse();

            if (animatorController != null)
            {
                // Usamos um switch para lidar com os resultados do enum.
                switch (interactionType)
                {
                    case InteractionType.CollectResource:
                        animatorController.TriggerCollectAnimation(); // A animação correta para coletar
                        break;

                    case InteractionType.SpeedUpConstruction:
                        animatorController.TriggerUseAnimation(); // A animação correta para acelerar
                        break;

                    // Se o tipo for 'None', não fazemos nada.
                    case InteractionType.None:
                    default:
                        break;
                }
            }
        }
    }

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

    public void OnSubmit(InputValue value) // Tecla 'Enter'
    {
        if (CurrentState == PlayerState.BuildMenu)
        {
            buildUIManager.SubmitSelection();
        }
    }

    public void OnCollect(InputValue value) // Tecla 'E'
    {
        if (CurrentState == PlayerState.Gameplay)
        {
            if (animatorController != null) animatorController.TriggerCollectAnimation();
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