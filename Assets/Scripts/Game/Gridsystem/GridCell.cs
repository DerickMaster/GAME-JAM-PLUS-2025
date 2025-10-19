using UnityEngine;

public class GridCell : MonoBehaviour
{
    [Header("Configuração da Célula")]
    public Vector2Int coordinates;
    [Header("Visualização de Debug")]
    public GameObject occupiedVisualizerPrefab;

    public bool isOccupied { get; private set; } = false;

    public bool IsBroken { get; private set; } = false;
    public GameObject placedBuildingObject { get; private set; }
    public Animator animator;
    private GameObject visualizerInstance;

    private readonly int isBrokenHash = Animator.StringToHash("isBroken");

    void Start()
    {
        isOccupied = false;
        if (visualizerInstance != null) { Destroy(visualizerInstance); }
        if (GridManager.Instance != null) { GridManager.Instance.RegisterCell(this); }
    }

    public void Break()
    {
        if (IsBroken || isOccupied) return; // Não pode quebrar se já estiver quebrada ou ocupada.

        IsBroken = true;
        if (animator != null)
        {
            animator.SetBool(isBrokenHash, true);
        }
        Debug.Log($"<color=red>Célula {coordinates} foi QUEBRADA!</color>");
    }

    public void Repair()
    {
        if (!IsBroken) return; // Só pode consertar se estiver quebrada.

        IsBroken = false;
        if (animator != null)
        {
            animator.SetBool(isBrokenHash, false);
        }
        Debug.Log($"<color=green>Célula {coordinates} foi CONSERTADA!</color>");
    }

    public void Occupy(GameObject building)
    {
        isOccupied = true;
        placedBuildingObject = building;
        if (occupiedVisualizerPrefab != null && visualizerInstance == null)
        {
            Vector3 spawnPosition = transform.position + new Vector3(0, 0.05f, 0);
            visualizerInstance = Instantiate(occupiedVisualizerPrefab, spawnPosition, Quaternion.identity, transform);
        }
    }

    public void Vacate()
    {
        isOccupied = false;
        placedBuildingObject = null;
        if (visualizerInstance != null)
        {
            Destroy(visualizerInstance);
            visualizerInstance = null;
        }
    }
}