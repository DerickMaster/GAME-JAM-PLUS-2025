using UnityEngine;

public class PlayerBuilder : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private PlayerController playerController;

    [Header("Preview Objects")]
    [SerializeField] private GameObject gameplayHighlightPrefab;

    [Header("Preview Materials")]
    [SerializeField] private Material validPlacementMaterial;
    [SerializeField] private Material invalidPlacementMaterial;

    [Header("Preview Settings")]
    [SerializeField] private float previewScaleMultiplier = 1.0f;

    private BuildingData dataToBuild;
    private GridCell currentTargetCell;
    private GameObject previewInstance;
    private MeshRenderer[] previewRenderers;
    private GameObject highlightInstance;
    private GridCell lastHighlightedCell;

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
                // A chamada está correta, a função que faltava.
                UpdatePreviewPositionAndColor();
            }
        }
        else if (playerController.CurrentState == PlayerState.Gameplay)
        {
            UpdateGameplayHighlight();
        }
    }

    // --- ESTA É A FUNÇÃO QUE FALTAVA ---
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
            // Local VÁLIDO
            GridCell startCell = GridManager.Instance.grid[placementCoords.x, placementCoords.y];
            previewInstance.transform.position = startCell.transform.position;
            previewInstance.transform.rotation = Quaternion.identity;
            SetPreviewMaterial(validPlacementMaterial);
        }
        else
        {
            // Local INVÁLIDO
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

        previewRenderers = previewInstance.GetComponentsInChildren<MeshRenderer>();
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
            GridManager.Instance.PlaceBuilding(dataToBuild, placementCoords);
            playerController.SetState(PlayerState.Gameplay);
            if (previewInstance != null) Destroy(previewInstance);
        }
        else
        {
            Debug.Log("Local inválido!");
        }
    }

    private void SetPreviewMaterial(Material mat)
    {
        if (previewRenderers == null) return;
        foreach (var renderer in previewRenderers)
        {
            renderer.material = mat;
        }
    }

    private void OnTriggerEnter(Collider other) { if (other.GetComponent<GridCell>() != null) currentTargetCell = other.GetComponent<GridCell>(); }
    private void OnTriggerExit(Collider other) { if (other.GetComponent<GridCell>() == currentTargetCell) currentTargetCell = null; }
}