using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }
    public GridCell[,] grid = new GridCell[4, 4];

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void RegisterCell(GridCell cell)
    {
        if (cell.coordinates.x < 4 && cell.coordinates.y < 4)
        {
            grid[cell.coordinates.x, cell.coordinates.y] = cell;
        }
    }

    public bool FindValidPlacement(Vector2Int targetCell, Vector2Int buildingSize, Vector2Int playerCell, out Vector2Int foundPlacement)
    {
        // Esta lógica de encontrar o melhor lugar continua a mesma e funciona bem.
        for (int yOffset = 0; yOffset < buildingSize.y; yOffset++)
        {
            for (int xOffset = 0; xOffset < buildingSize.x; xOffset++)
            {
                Vector2Int potentialStart = new Vector2Int(targetCell.x - yOffset, targetCell.y - xOffset);

                if (IsAreaValid(potentialStart, buildingSize, playerCell))
                {
                    foundPlacement = potentialStart;
                    return true;
                }
            }
        }

        foundPlacement = Vector2Int.zero;
        return false;
    }

    private bool IsAreaValid(Vector2Int startCoords, Vector2Int size, Vector2Int playerCell)
    {
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                int checkY = startCoords.x + y;
                int checkX = startCoords.y + x;
                Vector2Int currentCheckCell = new Vector2Int(checkY, checkX);

                if (checkY < 0 || checkY >= 4 || checkX < 0 || checkX >= 4) return false;
                if (grid[checkY, checkX] == null) { Debug.LogError($"Célula [{checkY},{checkX}] não registrada!"); return false; }
                if (grid[checkY, checkX].isOccupied) return false;
                if (currentCheckCell == playerCell) return false;
            }
        }
        return true;
    }

    // --- MUDANÇA DE ARQUITETURA AQUI ---
    public void PlaceBuilding(BuildingData buildingData, Vector2Int startCoords)
    {
        // 1. Pega a referência do GameObject da célula inicial (que será o pai).
        GridCell startCell = grid[startCoords.x, startCoords.y];

        // 2. Instancia o prefab E O TORNA FILHO da startCell.
        // O Unity automaticamente posiciona o filho no centro do pai.
        GameObject newBuilding = Instantiate(buildingData.prefab, startCell.transform);

        // Opcional, mas garante alinhamento perfeito se o prefab tiver alguma posição residual.
        newBuilding.transform.localPosition = Vector3.zero;

        // 3. Ocupa as células logicamente, incluindo a célula pai.
        for (int y = 0; y < buildingData.size.y; y++)
        {
            for (int x = 0; x < buildingData.size.x; x++)
            {
                grid[startCoords.x + y, startCoords.y + x].Occupy(newBuilding);
            }
        }

        Debug.Log($"Construído {buildingData.buildingName} como filho de {startCell.gameObject.name}");
    }
}