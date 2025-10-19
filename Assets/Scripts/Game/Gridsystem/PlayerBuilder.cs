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
    [Tooltip("O multiplicador de escala para o preview corresponder ao tamanho da construção final.")]
    [SerializeField] private float previewScaleMultiplier = 1.0f;

    private const int CELL_REPAIR_METAL_COST = 20;
    private const int CELL_REPAIR_PLASTIC_COST = 20;

    // Variáveis de estado privadas
    private BuildingData dataToBuild;
    private GridCell currentTargetCell;
    private GameObject previewInstance;
    private Renderer[] previewRenderers;
    private GameObject highlightInstance;
    private GridCell lastHighlightedCell;
    private Destructible currentDismantleTarget;
    private CollectibleResource currentTargetResource;


    void Start()
    {
        if (gameplayHighlightPrefab != null)
        {
            highlightInstance = Instantiate(gameplayHighlightPrefab);
            highlightInstance.SetActive(false);
        }
    }

    void Update()
    {
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
            GridCell startCell = GridManager.Instance.grid[placementCoords.x, placementCoords.y];
            previewInstance.transform.position = startCell.transform.position;
            previewInstance.transform.rotation = Quaternion.identity;
            SetPreviewMaterial(validPlacementMaterial);
        }
        else
        {
            previewInstance.transform.position = currentTargetCell.transform.position;
            previewInstance.transform.rotation = Quaternion.identity;
            SetPreviewMaterial(invalidPlacementMaterial);
        }
    }

    private void UpdateGameplayHighlight()
    {
        if (highlightInstance == null) return;
        if (currentTargetCell != lastHighlightedCell)
        {
            if (currentTargetCell != null)
            {
                highlightInstance.SetActive(true);
                highlightInstance.transform.position = currentTargetCell.transform.position;
            }
            else
            {
                highlightInstance.SetActive(false);
            }
            lastHighlightedCell = currentTargetCell;
        }
    }

    public void EnterBuildMode(BuildingData buildingData)
    {
        if (playerController == null) return;
        if (highlightInstance != null)
        {
            highlightInstance.SetActive(false);
            lastHighlightedCell = null;
        }

        dataToBuild = buildingData;
        playerController.SetState(PlayerState.PlacingObject);

        if (previewInstance != null) Destroy(previewInstance);
        previewInstance = Instantiate(dataToBuild.prefab);
        previewInstance.transform.localScale *= previewScaleMultiplier;

        previewRenderers = previewInstance.GetComponentsInChildren<Renderer>();
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
        if (currentTargetCell == null || dataToBuild == null) return;

        if (!ResourceManager.Instance.HasEnoughResources(dataToBuild.metalCost, dataToBuild.plasticCost))
        {
            Debug.Log("Recursos insuficientes!");
            return;
        }

        Vector2Int playerCell = playerController.CurrentGridCell;
        if (GridManager.Instance.FindValidPlacement(currentTargetCell.coordinates, dataToBuild.size, playerCell, out Vector2Int placementCoords))
        {
            ResourceManager.Instance.SpendResources(dataToBuild.plasticCost, dataToBuild.metalCost);
            GridManager.Instance.PlaceBuilding(dataToBuild, placementCoords);
            playerController.SetState(PlayerState.Gameplay);
            if (previewInstance != null) Destroy(previewInstance);
        }
        else
        {
            Debug.Log("Local inválido!");
        }
    }


    public InteractionType TryUse()
    {
        // PRIORIDADE MÁXIMA: Consertar uma célula do grid.
        if (currentTargetCell != null && currentTargetCell.IsBroken)
        {
            if (ResourceManager.Instance.HasEnoughResources(CELL_REPAIR_METAL_COST, CELL_REPAIR_PLASTIC_COST))
            {
                ResourceManager.Instance.SpendResources(CELL_REPAIR_PLASTIC_COST, CELL_REPAIR_METAL_COST);
                currentTargetCell.Repair();
                return InteractionType.RepairObject; // Reutilizamos o mesmo tipo de interação.
            }
            else
            {
                Debug.Log("Recursos insuficientes para consertar a célula!");
                return InteractionType.None;
            }
        }

        // Prioridade 2: Coletar um recurso.
        if (currentTargetResource != null)
        {
            currentTargetResource.Collect();
            currentTargetResource = null;
            return InteractionType.CollectResource;
        }

        // Prioridade 3: Interagir com uma construção (acelerar OU consertar).
        if (currentTargetCell != null && currentTargetCell.isOccupied)
        {
            Destructible destructibleTarget = currentTargetCell.placedBuildingObject.GetComponentInChildren<Destructible>();

            if (destructibleTarget != null)
            {
                if (destructibleTarget.IsBroken)
                {
                    if (destructibleTarget.TryRepair())
                    {
                        return InteractionType.RepairObject;
                    }
                    else
                    {
                        return InteractionType.None;
                    }
                }
                else
                {
                    Constructible constructible = currentTargetCell.placedBuildingObject.GetComponent<Constructible>();
                    if (constructible != null)
                    {
                        constructible.SpeedUpConstruction();
                        return InteractionType.SpeedUpConstruction;
                    }
                }
            }
        }

        return InteractionType.None;
    }

    public void StartDismantling()
    {
        if (currentTargetCell == null || !currentTargetCell.isOccupied) return;

        Destructible target = currentTargetCell.placedBuildingObject.GetComponentInChildren<Destructible>();
        if (target != null)
        {
            currentDismantleTarget = target;
            playerController.SetState(PlayerState.Dismantling);
            playerController.GetComponentInChildren<PlayerAnimatorController>().SetInteracting(true);
            currentDismantleTarget.StartDismantling();
        }
    }

    public void StopDismantling()
    {
        if (currentDismantleTarget != null)
        {
            currentDismantleTarget.StopDismantling();
            currentDismantleTarget = null;
        }
        playerController.SetState(PlayerState.Gameplay);
        playerController.GetComponentInChildren<PlayerAnimatorController>().SetInteracting(false);
    }

    private void SetPreviewMaterial(Material mat)
    {
        if (previewRenderers == null) return;
        foreach (var renderer in previewRenderers)
        {
            Material[] materials = new Material[renderer.materials.Length];
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = mat;
            }
            renderer.materials = materials;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Checa se é uma célula do grid.
        GridCell cell = other.GetComponent<GridCell>();
        if (cell != null)
            currentTargetCell = cell;

        // Checa se é um recurso coletável.
        CollectibleResource resource = other.GetComponent<CollectibleResource>();
        if (resource != null)
            currentTargetResource = resource;
    }

    private void OnTriggerExit(Collider other)
    {
        // Checa se está saindo de uma célula do grid.
        if (other.GetComponent<GridCell>() == currentTargetCell)
            currentTargetCell = null;

        // Checa se está saindo de um recurso coletável.
        if (other.GetComponent<CollectibleResource>() == currentTargetResource)
            currentTargetResource = null;
    }
}