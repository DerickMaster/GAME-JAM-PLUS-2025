using UnityEngine;
using System.Collections.Generic;

public class Destructible : MonoBehaviour
{
    public bool IsBroken { get; private set; } = false;

    [Header("Sistema de Quebra")]
    [SerializeField] private GameObject brokenEffectObject;
    [SerializeField] private float breakageCheckInterval = 30f;
    [Range(0f, 1f)]
    [SerializeField] private float breakageChance = 0.1f;

    private BuildingData buildingData;
    private List<GridCell> occupiedCells;
    private float dismantleProgress = 0f;
    private bool isBeingDismantled = false;
    private Animator animator;
    private float breakageTimer;
    private int repairPlasticCost;
    private int repairMetalCost;
    private readonly int isBrokenHash = Animator.StringToHash("isBroken");
    private readonly float dismantleTimeToComplete = 2.0f;

    private void Awake()
    {
        if (brokenEffectObject != null)
        {
            brokenEffectObject.SetActive(false);
        }
    }

    public void Initialize(BuildingData data, List<GridCell> cells)
    {
        buildingData = data;
        occupiedCells = cells;
        animator = GetComponent<Animator>();
        repairPlasticCost = Mathf.CeilToInt(buildingData.plasticCost * 0.2f);
        repairMetalCost = Mathf.CeilToInt(buildingData.metalCost * 0.2f);
        breakageTimer = Random.Range(breakageCheckInterval * 0.5f, breakageCheckInterval * 1.5f);
    }

    void Update()
    {
        if (isBeingDismantled)
        {
            dismantleProgress += Time.deltaTime;
            if (dismantleProgress >= dismantleTimeToComplete) Dismantle();
            return;
        }

        // Agora usamos a propriedade pública 'IsBroken'
        if (!IsBroken)
        {
            breakageTimer -= Time.deltaTime;
            if (breakageTimer <= 0)
            {
                AttemptToBreak();
                breakageTimer = breakageCheckInterval;
            }
        }
    }

    private void AttemptToBreak()
    {
        if (Random.value <= breakageChance)
        {
            Break();
        }
    }

    private void Break()
    {
        IsBroken = true; // Usamos a propriedade para definir o valor
        if (animator != null)
        {
            animator.SetBool(isBrokenHash, true);
            animator.speed = 0f;
        }
        if (brokenEffectObject != null)
        {
            brokenEffectObject.SetActive(true);
        }
    }

    public bool TryRepair()
    {
        if (!IsBroken) return false;
        if (ResourceManager.Instance.HasEnoughResources(repairMetalCost, repairPlasticCost))
        {
            ResourceManager.Instance.SpendResources(repairPlasticCost, repairMetalCost);
            Repair();
            return true;
        }
        else
        {
            Debug.Log("Recursos insuficientes para consertar!");
            return false;
        }
    }

    private void Repair()
    {
        IsBroken = false; // Usamos a propriedade para definir o valor
        if (animator != null)
        {
            animator.SetBool(isBrokenHash, false);
            animator.speed = 1f;
        }
        if (brokenEffectObject != null)
        {
            brokenEffectObject.SetActive(false);
        }
        Debug.Log($"<color=green>ITEM CONSERTADO!</color> {gameObject.name} foi consertado.");
    }

    public void StartDismantling()
    {
        isBeingDismantled = true;
        if (IsBroken && brokenEffectObject != null)
        {
            brokenEffectObject.SetActive(false);
        }
        if (animator != null) animator.speed = 1f;
    }

    public void StopDismantling()
    {
        isBeingDismantled = false;
        dismantleProgress = 0f;
        if (IsBroken && brokenEffectObject != null)
        {
            brokenEffectObject.SetActive(true);
            if (animator != null) animator.speed = 0f;
        }
    }

    private void Dismantle()
    {
        int metalRefund = buildingData.metalCost / 2;
        int plasticRefund = buildingData.plasticCost / 2;
        ResourceManager.Instance.AddResources(plasticRefund, metalRefund);
        if (WeightManager.Instance != null)
        {
            WeightManager.Instance.RemoveWeight(buildingData.weight);
        }
        if (occupiedCells != null)
        {
            foreach (GridCell cell in occupiedCells)
            {
                cell.Vacate();
            }
        }
        Destroy(gameObject);
    }
}