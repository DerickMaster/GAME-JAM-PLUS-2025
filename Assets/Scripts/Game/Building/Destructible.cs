using UnityEngine;
using System.Collections.Generic;

public class Destructible : MonoBehaviour
{
    public bool IsBroken { get; private set; } = false;

    // --- MUDANÇA AQUI: Trocamos o prefab por uma referência direta ---
    [Header("Sistema de Quebra")]
    [Tooltip("Arraste o objeto filho que contém o efeito de partícula de quebra aqui.")]
    [SerializeField] private GameObject brokenEffectObject; // Em vez de brokenEffectPrefab

    [Tooltip("O intervalo (em segundos) em que o item pode tentar quebrar.")]
    [SerializeField] private float breakageCheckInterval = 30f;
    [Tooltip("A chance (de 0 a 1) de o item quebrar em cada checagem.")]
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
        // Garante que o efeito de fumaça comece sempre desativado.
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
        IsBroken = true;
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
        IsBroken = false;
        if (animator != null)
        {
            animator.SetBool(isBrokenHash, false);
            animator.speed = 1f;
        }

        // --- MUDANÇA AQUI: Desativa o objeto em vez de destruir ---
        if (brokenEffectObject != null)
        {
            brokenEffectObject.SetActive(false);
        }
        Debug.Log($"<color=green>ITEM CONSERTADO!</color> {gameObject.name} foi consertado.");
    }

    public void StartDismantling()
    {
        isBeingDismantled = true;
        // Se estiver quebrado, esconde a fumaça.
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
        // Se parou de desmantelar e ainda está quebrado, mostra a fumaça de novo.
        if (IsBroken && brokenEffectObject != null)
        {
            brokenEffectObject.SetActive(true);
            if (animator != null) animator.speed = 0f;
        }
    }

    private void Dismantle()
    {
        // ... (lógica de devolução de recursos, etc., não muda) ...
        Destroy(gameObject);
    }
}