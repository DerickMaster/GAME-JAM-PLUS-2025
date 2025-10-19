using UnityEngine;

public class RaftStatusManager : MonoBehaviour
{
    public static RaftStatusManager Instance { get; private set; }

    [Header("Configuração da Balsa")]
    [SerializeField] private int initialRaftWeight = 0;
    [SerializeField] private float overloadTimeToLose = 30f;

    public int CurrentPower { get; private set; }
    public int CurrentWeight { get; private set; }
    public bool IsOverloaded { get; private set; }
    public float OverloadTimer { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Start()
    {
        CurrentWeight = initialRaftWeight;
        CurrentPower = 0;
        UpdateStatus();
    }

    void Update()
    {
        if (IsOverloaded)
        {
            OverloadTimer += Time.deltaTime;
            if (OverloadTimer >= overloadTimeToLose)
            {
                GameOver();
            }
        }
    }

    // --- FUNÇÕES DE ATALHO (CHAMADAS POR OUTROS SCRIPTS) ---

    // Chamada pelo GridManager ao iniciar a construção
    public void AddBuilding(BuildingData data)
    {
        AddWeight(data.weight);
        // A potência só é adicionada pelo Constructible quando a construção termina
    }

    // Chamada pelo Destructible ao desmantelar
    public void RemoveBuilding(BuildingData data)
    {
        RemoveWeight(data.weight);

        // Se o item que está sendo removido era um motor E NÃO estava quebrado,
        // sua potência estava ativa, então a removemos.
        if (data.isMotor)
        {
            Destructible d = GetComponent<Destructible>(); // Verificação para o futuro
            if (d == null || !d.IsBroken)
            {
                RemovePower(data.powerProvided);
            }
        }
    }


    // --- FUNÇÕES GRANULARES (USADAS INTERNAMENTE E POR OUTROS SCRIPTS) ---
    public void AddWeight(int amount)
    {
        CurrentWeight += amount;
        UpdateStatus();
    }

    public void RemoveWeight(int amount)
    {
        CurrentWeight -= amount;
        if (CurrentWeight < initialRaftWeight) CurrentWeight = initialRaftWeight;
        UpdateStatus();
    }

    public void AddPower(int amount)
    {
        CurrentPower += amount;
        UpdateStatus();
    }

    public void RemovePower(int amount)
    {
        CurrentPower -= amount;
        if (CurrentPower < 0) CurrentPower = 0;
        UpdateStatus();
    }

    private void UpdateStatus()
    {
        IsOverloaded = CurrentPower < CurrentWeight;
        if (!IsOverloaded)
        {
            OverloadTimer = 0f;
        }
    }

    private void GameOver()
    {
        Debug.LogError("GAME OVER! A balsa foi engolida pela fera do mar.");
        Time.timeScale = 0f;
    }
}