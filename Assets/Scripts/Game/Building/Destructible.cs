using UnityEngine;
using System.Collections.Generic;

public class Destructible : MonoBehaviour
{
    public bool IsBroken { get; private set; } = false;

    [Header("Sistema de Quebra")]
    [SerializeField] public GameObject brokenEffectObject;
    [SerializeField] public float breakageCheckInterval = 30f;
    [Range(0f, 1f)]
    [SerializeField] public float breakageChance = 0.1f;

    [Header("Som (Opcional)")]
    [SerializeField] public bool haveSound = false;
    [SerializeField] public GameObject loopableSoundObject;

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

    private bool isInitialized = false;

    private void Awake()
    {
        if (brokenEffectObject != null)
        {
            brokenEffectObject.SetActive(false);
        }
    }

    public void Initialize(BuildingData data, List<GridCell> cells)
    {
        isInitialized = true;
        buildingData = data;
        occupiedCells = cells;
        animator = GetComponentInChildren<Animator>();
        repairPlasticCost = Mathf.CeilToInt(buildingData.plasticCost * 0.2f);
        repairMetalCost = Mathf.CeilToInt(buildingData.metalCost * 0.2f);
        breakageTimer = Random.Range(breakageCheckInterval * 0.5f, breakageCheckInterval * 1.5f);
    }

    void Update()
    {
        // --- A PROTEÇÃO ---
        // Se o script não foi inicializado, ele não faz absolutamente nada.
        if (!isInitialized) return;

        if (isBeingDismantled)
        {
            dismantleProgress += Time.deltaTime;
            if (dismantleProgress >= dismantleTimeToComplete) Dismantle();
            return;
        }

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

        // --- NOSSO DEBUG PRINCIPAL ---
        // Esta linha vai nos dizer tudo o que precisamos saber.
        if (buildingData != null)
        {
            Debug.Log($"[Destructible] Checando se é motor. Nome do item: {buildingData.buildingName}, É um motor? {buildingData.isMotor}");
        }
        else
        {
            Debug.LogError("[Destructible] ERRO CRÍTICO: 'buildingData' é nulo no momento da quebra!");
            return; // Sai da função para evitar mais erros.
        }
        // -----------------------------

        if (animator != null)
        {
            animator.SetBool(isBrokenHash, true);
            animator.speed = 0f;
        }
        if (brokenEffectObject != null)
        {
            brokenEffectObject.SetActive(true);
        }
        if (haveSound && loopableSoundObject)
        {
            loopableSoundObject.SetActive(false);
        }

        if (buildingData.isMotor)
        {
            RaftStatusManager.Instance.RemovePower(buildingData.powerProvided);
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
        IsBroken = false;
        if (animator != null)
        {
            animator.SetBool(isBrokenHash, false);
            animator.speed = 1f;
        }
        if (brokenEffectObject != null)
        {
            brokenEffectObject.SetActive(false);
        }
        if (haveSound && loopableSoundObject != null)
        {
            loopableSoundObject.SetActive(true);
        }
        if (buildingData.isMotor)
        {
            RaftStatusManager.Instance.AddPower(buildingData.powerProvided);
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

        if (RaftStatusManager.Instance != null)
        {
            RaftStatusManager.Instance.RemoveBuilding(buildingData);
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