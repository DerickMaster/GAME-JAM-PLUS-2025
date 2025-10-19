using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Required to use Lists

public class Constructible : MonoBehaviour
{
    [Header("Referências Visuais")]
    [SerializeField] private GameObject previewObject;
    [SerializeField] private GameObject constructionObject;
    [SerializeField] private GameObject finalModelObject;
    [SerializeField] private Destructible destructible;

    [Header("Animação")]
    [SerializeField] private Animator constructionBoxAnimator;

    private BuildingData buildingData;
    private float constructionTimer;
    private bool isConstructing = false;

    // --- THIS VARIABLE WAS MISSING ---
    // Stores the list of grid cells this construction occupies.
    private List<GridCell> occupiedCells;

    private readonly int isConstructingHash = Animator.StringToHash("isConstructing");

    // --- THIS FUNCTION SIGNATURE WAS UPDATED ---
    // It now accepts the list of cells from the GridManager.
    public void Initialize(BuildingData data, List<GridCell> cells)
    {
        buildingData = data;
        occupiedCells = cells; // We store the list here.
        isConstructing = true;
        constructionTimer = 0f;

        if (previewObject != null) previewObject.SetActive(false);
        if (constructionObject != null) constructionObject.SetActive(true);

        if (constructionBoxAnimator != null)
        {
            constructionBoxAnimator.SetBool(isConstructingHash, true);
        }
    }

    void Update()
    {
        if (!isConstructing || buildingData == null) return;
        constructionTimer += Time.deltaTime;
        if (constructionTimer >= buildingData.constructionTime)
        {
            StartCoroutine(CompleteConstructionSequence());
        }
    }

    public void SpeedUpConstruction()
    {
        if (!isConstructing) return;
        int metalCost = buildingData.metalCost / 2;
        int plasticCost = buildingData.plasticCost / 2;
        if (ResourceManager.Instance.HasEnoughResources(plasticCost, metalCost))
        {
            ResourceManager.Instance.SpendResources(plasticCost, metalCost);
            StartCoroutine(CompleteConstructionSequence());
        }
        else
        {
            Debug.Log("Recursos insuficientes para acelerar!");
        }
    }

    private IEnumerator CompleteConstructionSequence()
    {
        isConstructing = false;
        if (constructionBoxAnimator != null) constructionBoxAnimator.SetBool(isConstructingHash, false);

        yield return new WaitForSeconds(1f);

        if (constructionObject != null) constructionObject.SetActive(false);
        if (finalModelObject != null) finalModelObject.SetActive(true);

        if (buildingData.isMotor)
        {
            RaftStatusManager.Instance.AddPower(buildingData.powerProvided);
        }

        destructible.Initialize(buildingData, occupiedCells);

        Destroy(this); // O trabalho do Constructible acabou.
    }
}