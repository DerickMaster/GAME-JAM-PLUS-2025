using UnityEngine;
using UnityEngine.SceneManagement; // << ADICIONADO: Necessário para gerenciar cenas

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

    public void AddBuilding(BuildingData data)
    {
        AddWeight(data.weight);
    }

    public void RemoveBuilding(BuildingData data)
    {
        RemoveWeight(data.weight);

        if (data.isMotor)
        {
            Destructible d = GetComponent<Destructible>();
            if (d == null || !d.IsBroken)
            {
                RemovePower(data.powerProvided);
            }
        }
    }

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

    // --- FUNÇÃO MODIFICADA ---
    private void GameOver()
    {
        Debug.LogError("GAME OVER! A balsa foi engolida pela fera do mar.");

        // Garante que o tempo do jogo volte ao normal antes de carregar a nova cena.
        Time.timeScale = 1f;

        // Carrega a cena de Game Over.
        SceneManager.LoadScene("LoseScreen");
    }
}