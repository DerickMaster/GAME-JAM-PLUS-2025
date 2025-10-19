using UnityEngine;

public class PlayerBuilder : MonoBehaviour
{
    [Header("Components")]
    [Tooltip("Arraste o objeto Player que tem o PlayerController aqui.")]
    [SerializeField] private PlayerController playerController;

    [Header("Preview Objects")]
    [Tooltip("O prefab do highlight para o modo gameplay.")]
    [SerializeField] private GameObject gameplayHighlightPrefab;

    [Header("Preview Materials")]
    [Tooltip("O material verde semi-transparente para posições válidas.")]
    [SerializeField] private Material validPlacementMaterial;
    [Tooltip("O material vermelho semi-transparente para posições inválidas.")]
    [SerializeField] private Material invalidPlacementMaterial;

    [Header("Preview Settings")]
    [Tooltip("O multiplicador de escala para o preview corresponder ao tamanho da construção final (geralmente a escala da sua plataforma de grid).")]
    [SerializeField] private float previewScaleMultiplier = 1.0f;

    // Variáveis de estado privadas
    private BuildingData dataToBuild;
    private GridCell currentTargetCell;
    private GameObject previewInstance;
    private Renderer[] previewRenderers;
    private GameObject highlightInstance;
    private GridCell lastHighlightedCell;

    void Start()
    {
        // Cria a instância do highlight no início do jogo para reutilizá-la.
        if (gameplayHighlightPrefab != null)
        {
            highlightInstance = Instantiate(gameplayHighlightPrefab);
            highlightInstance.SetActive(false); // Começa escondido.
        }
    }

    void Update()
    {
        // Gerencia qual visualização deve ser mostrada com base no estado do jogador.
        if (playerController.CurrentState == PlayerState.PlacingObject)
        {
            if (previewInstance != null)
            {
                UpdatePreviewPositionAndColor();
            }
        }
        else if (playerController.CurrentState == PlayerState.Gameplay)
        {
            UpdateGameplayHighlight();
        }
    }

    private void UpdatePreviewPositionAndColor()
    {
        if (currentTargetCell == null)
        {
            previewInstance.SetActive(false);
            return;
        }

        previewInstance.SetActive(true);
        Vector2Int playerCell = playerController.CurrentGridCell;

        if (GridManager.Instance.FindValidPlacement(currentTargetCell.coordinates, dataToBuild.size, playerCell, out Vector2Int placementCoords))
        {
            // Se o local é VÁLIDO:
            GridCell startCell = GridManager.Instance.grid[placementCoords.x, placementCoords.y];
            previewInstance.transform.position = startCell.transform.position;
            previewInstance.transform.rotation = Quaternion.identity;
            SetPreviewMaterial(validPlacementMaterial);
        }
        else
        {
            // Se o local é INVÁLIDO:
            previewInstance.transform.position = currentTargetCell.transform.position;
            previewInstance.transform.rotation = Quaternion.identity;
            SetPreviewMaterial(invalidPlacementMaterial);
        }
    }

    private void UpdateGameplayHighlight()
    {
        if (highlightInstance == null) return;

        // Otimização: só atualiza se a célula alvo mudou.
        if (currentTargetCell != lastHighlightedCell)
        {
            if (currentTargetCell != null) // Se o detector está sobre uma nova célula
            {
                highlightInstance.SetActive(true);
                highlightInstance.transform.position = currentTargetCell.transform.position;
            }
            else // Se o detector saiu de todas as células
            {
                highlightInstance.SetActive(false);
            }
            lastHighlightedCell = currentTargetCell; // Atualiza a "memória".
        }
    }

    public void EnterBuildMode(BuildingData buildingData)
    {
        if (playerController == null) return;

        // Esconde o highlight do modo gameplay para dar lugar ao preview.
        if (highlightInstance != null)
        {
            highlightInstance.SetActive(false);
            lastHighlightedCell = null;
        }

        dataToBuild = buildingData;
        playerController.SetState(PlayerState.PlacingObject);

        if (previewInstance != null) Destroy(previewInstance);

        // --- LÓGICA SIMPLIFICADA ---
        // 1. Cria a instância do prefab (que já deve vir com o "Preview" ativado por padrão).
        previewInstance = Instantiate(dataToBuild.prefab);
        previewInstance.transform.localScale *= previewScaleMultiplier;

        // 2. Encontra os renderers para trocar o material.
        previewRenderers = previewInstance.GetComponentsInChildren<Renderer>();

        // 3. Desativa os colisores do preview para que ele não interfira com nada.
        foreach (Collider col in previewInstance.GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }
    }

    public void CancelBuildMode()
    {
        playerController.SetState(PlayerState.Gameplay);
        if (previewInstance != null) Destroy(previewInstance);
    }

    public void ConfirmBuild()
    {
        if (currentTargetCell == null) return;

        Vector2Int playerCell = playerController.CurrentGridCell;
        if (GridManager.Instance.FindValidPlacement(currentTargetCell.coordinates, dataToBuild.size, playerCell, out Vector2Int placementCoords))
        {
            // Destrói o preview e comanda o GridManager a criar uma instância NOVA e limpa do prefab.
            GridManager.Instance.PlaceBuilding(dataToBuild, placementCoords);
            playerController.SetState(PlayerState.Gameplay);
            if (previewInstance != null) Destroy(previewInstance);
        }
        else
        {
            Debug.Log("Local inválido!");
        }
    }

    // A função de troca de material, que lida com múltiplos materiais.
    private void SetPreviewMaterial(Material mat)
    {
        if (previewRenderers == null) return;

        foreach (var renderer in previewRenderers)
        {
            // Cria um array de materiais com o tamanho da lista de materiais do renderer.
            Material[] materials = new Material[renderer.materials.Length];

            // Preenche cada "slot" do array com o nosso material de preview.
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = mat;
            }

            // Atribui o array de materiais modificado de volta ao renderer.
            renderer.materials = materials;
        }
    }

    // Funções de detecção de célula via Trigger.
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<GridCell>() != null)
            currentTargetCell = other.GetComponent<GridCell>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<GridCell>() == currentTargetCell)
            currentTargetCell = null;
    }

    private Destructible currentDismantleTarget;

    public void StartDismantling()
    {
        if (currentTargetCell == null || !currentTargetCell.isOccupied) return;

        Destructible target = currentTargetCell.placedBuildingObject.GetComponentInChildren<Destructible>();
        if (target != null)
        {
            currentDismantleTarget = target;
            playerController.SetState(PlayerState.Dismantling);
            playerController.GetComponentInChildren<PlayerAnimatorController>().SetInteracting(true); // Liga a animação
            currentDismantleTarget.StartDismantling(); // Avisa o objeto para começar a contar o tempo.
        }
    }

    public void StopDismantling()
    {
        if (currentDismantleTarget != null)
        {
            currentDismantleTarget.StopDismantling(); // Avisa o objeto para parar de contar.
            currentDismantleTarget = null;
        }
        playerController.SetState(PlayerState.Gameplay);
        playerController.GetComponentInChildren<PlayerAnimatorController>().SetInteracting(false); // Desliga a animação
    }
}