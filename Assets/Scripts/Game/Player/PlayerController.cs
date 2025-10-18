using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState { Gameplay, BuildMenu, PlacingObject }

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5.0f;
    public float rotationSpeed = 720f;
    [Header("Jumping & Gravity")]
    public float jumpHeight = 1.0f;
    public float gravity = -9.81f;

    [Header("Components")]
    [SerializeField] private PlayerBuilder playerBuilder;
    [SerializeField] private BuildUIManager buildUIManager;

    public PlayerState CurrentState { get; private set; } = PlayerState.Gameplay;

    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool isGrounded;
    private Vector2 moveInput;

    public Vector2Int CurrentGridCell { get; private set; } = new Vector2Int(-1, -1);

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // --- A MUDANÇA MAIS IMPORTANTE ---
        // Agora, só paramos o movimento se o estado for EXATAMENTE BuildMenu.
        if (CurrentState == PlayerState.BuildMenu)
        {
            moveInput = Vector2.zero;
            return;
        }

        // Se o estado for Gameplay ou PlacingObject, este código será executado.
        isGrounded = controller.isGrounded;
        if (isGrounded && playerVelocity.y < 0) { playerVelocity.y = -2f; }
        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);
        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(moveDirection), rotationSpeed * Time.deltaTime);
        }
        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    public void SetState(PlayerState newState)
    {
        CurrentState = newState;
    }

    public void OnMove(InputValue value)
    {
        // Se estivermos no menu, o input vira navegação.
        if (CurrentState == PlayerState.BuildMenu)
        {
            buildUIManager.Navigate(value.Get<Vector2>());
        }
        // --- MUDANÇA AQUI ---
        // Se estivermos no Gameplay OU no modo de posicionar, o input vira movimento.
        else if (CurrentState == PlayerState.Gameplay || CurrentState == PlayerState.PlacingObject)
        {
            moveInput = value.Get<Vector2>();
        }
    }

    public void OnJump(InputValue value)
    {
        // --- MUDANÇA AQUI ---
        // Permite pular durante o Gameplay e o modo de posicionamento.
        if ((CurrentState == PlayerState.Gameplay || CurrentState == PlayerState.PlacingObject) && isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    public void OnOpenBuildMenu(InputValue value)
    {
        // A lógica do 'C' continua a mesma, alternando entre os contextos.
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

    public void OnSubmit(InputValue value)
    {
        if (CurrentState == PlayerState.BuildMenu)
        {
            buildUIManager.SubmitSelection();
        }
    }

    private void OnTriggerStay(Collider other) { if (other.GetComponent<GridCell>() != null) CurrentGridCell = other.GetComponent<GridCell>().coordinates; }
    private void OnTriggerExit(Collider other) { if (other.GetComponent<GridCell>() != null && other.GetComponent<GridCell>().coordinates == CurrentGridCell) CurrentGridCell = new Vector2Int(-1, -1); }
}