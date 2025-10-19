using UnityEngine;
using System.Collections.Generic; // Required to use Lists

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
                if (grid[checkY, checkX] == null) { return false; }
                if (grid[checkY, checkX].isOccupied) return false;
                if (currentCheckCell == playerCell) return false;
            }
        }
        return true;
    }

    public void PlaceBuilding(BuildingData buildingData, Vector2Int startCoords)
    {
        if (RaftStatusManager.Instance != null)
        {
            RaftStatusManager.Instance.AddWeight(buildingData.weight);
        }

        GridCell startCell = grid[startCoords.x, startCoords.y];
        GameObject newBuildingObject = Instantiate(buildingData.prefab, startCell.transform);
        newBuildingObject.transform.localPosition = Vector3.zero;

        // Pega a lista de células que serão ocupadas.
        List<GridCell> occupiedCells = GetCellsForArea(startCoords, buildingData.size);

        // Pega o script Constructible e o inicializa, passando os dados E a lista de células.
        Constructible constructible = newBuildingObject.GetComponent<Constructible>();
        if (constructible != null)
        {
            constructible.Initialize(buildingData, occupiedCells);
        }

        // Ocupa as células.
        foreach (GridCell cell in occupiedCells)
        {
            cell.Occupy(newBuildingObject);
        }
    }

    // --- THIS IS THE MISSING FUNCTION ---
    // A helper function to get a list of all cells for a given area.
    public List<GridCell> GetCellsForArea(Vector2Int startCoords, Vector2Int size)
    {
        List<GridCell> cells = new List<GridCell>();
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                cells.Add(grid[startCoords.x + y, startCoords.y + x]);
            }
        }
        return cells;
    }
}