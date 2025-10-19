using UnityEngine;
using System.Collections.Generic;

public class Destructible : MonoBehaviour
{
    private BuildingData buildingData;
    private List<GridCell> occupiedCells;
    private float dismantleProgress = 0f;
    private bool isBeingDismantled = false;

    // O GridManager vai chamar esta fun��o para passar os dados do item.
    public void Initialize(BuildingData data, List<GridCell> cells)
    {
        buildingData = data;
        occupiedCells = cells;
    }

    void Update()
    {
        // Se a flag de desmantelamento estiver ativa, o progresso aumenta.
        if (isBeingDismantled)
        {
            dismantleProgress += Time.deltaTime;

            if (dismantleProgress >= buildingData.constructionTime) // Usando constructionTime como tempo para desmontar
            {
                Dismantle();
            }
        }
    }

    // O PlayerBuilder vai chamar estas fun��es.
    public void StartDismantling()
    {
        isBeingDismantled = true;
    }

    public void StopDismantling()
    {
        isBeingDismantled = false;
        dismantleProgress = 0f; // Reseta o progresso se o jogador parar.
    }

    private void Dismantle()
    {
        // 1. Devolve uma parte dos recursos para o jogador.
        int metalRefund = buildingData.metalCost / 2; // 50% de reembolso
        int plasticRefund = buildingData.plasticCost / 2;
        ResourceManager.Instance.AddResources(plasticRefund, metalRefund);
        Debug.Log($"Desmantelado! Recursos recuperados: {plasticRefund} Pl�stico, {metalRefund} Metal.");

        // 2. Remove o peso do WeightManager.
        WeightManager.Instance.RemoveWeight(buildingData.weight);

        // 3. Libera as c�lulas do grid.
        foreach (GridCell cell in occupiedCells)
        {
            cell.Vacate();
        }

        // 4. Destr�i o objeto de constru��o.
        Destroy(gameObject);
    }
}