using UnityEngine;
using System.Collections.Generic;

public class WeatherManager : MonoBehaviour
{
    public static WeatherManager Instance { get; private set; }

    [Header("Configuração da Previsão")]
    [Tooltip("A lista completa de todos os possíveis WeatherData que o jogo pode usar.")]
    [SerializeField] private List<WeatherData> allWeatherData;
    [Tooltip("A sequência completa de climas para todos os 30 dias.")]
    [SerializeField] private WeatherType[] fullForecast = new WeatherType[30];

    [Header("Evento 'Pesado'")]
    [SerializeField] private float minCrabAttackInterval = 10f;
    [SerializeField] private float maxCrabAttackInterval = 30f;
    private float crabAttackTimer;
    private bool isHeavyDay = false;

    // --- NOVA CONSTANTE PARA A TEMPESTADE ---
    private const int STORM_WEIGHT_PENALTY = 6;

    public int CurrentDay { get; private set; } = 0;
    public int RadioCount { get; private set; } = 0;

    private Dictionary<WeatherType, WeatherData> weatherDataMap;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        weatherDataMap = new Dictionary<WeatherType, WeatherData>();
        foreach (var data in allWeatherData)
        {
            weatherDataMap[data.type] = data;
        }
    }

    void Update()
    {
        if (isHeavyDay)
        {
            crabAttackTimer -= Time.deltaTime;
            if (crabAttackTimer <= 0)
            {
                // Só ataca se o caranguejo não estiver já ativo.
                if (CrabController.Instance != null && !CrabController.Instance.gameObject.activeSelf)
                {
                    CrabController.Instance.StartAttackSequence();
                }
                ResetCrabTimer();
            }
        }

    }

    private void ResetCrabTimer()
    {
        crabAttackTimer = Random.Range(minCrabAttackInterval, maxCrabAttackInterval);
    }

    // --- NOVA LÓGICA NO START ---
    void Start()
    {
        // Verifica o clima do primeiro dia para aplicar efeitos imediatamente.
        WeatherData todayData = GetDataForDay(0);
        if (todayData != null && todayData.type == WeatherType.Storm)
        {
            Debug.Log($"<color=blue>[WeatherManager]</color> O jogo começou em uma tempestade! Adicionando peso extra.");
            RaftStatusManager.Instance.AddWeight(STORM_WEIGHT_PENALTY);
        }
    }

    public void RegisterRadio() { RadioCount++; }
    public void UnregisterRadio() { RadioCount--; }

    public WeatherData GetDataForDay(int dayIndex)
    {
        int actualDay = CurrentDay + dayIndex;
        if (actualDay < 0 || actualDay >= fullForecast.Length) return null;

        WeatherType type = fullForecast[actualDay];
        return weatherDataMap.ContainsKey(type) ? weatherDataMap[type] : null;
    }

    public void AdvanceToNextDay()
    {
        // --- LÓGICA ATUALIZADA AQUI ---

        // 1. Verifica o clima do dia que está TERMINANDO.
        WeatherData endedDayData = GetDataForDay(0);
        if (endedDayData != null && endedDayData.type == WeatherType.Storm)
        {
            // Se era uma tempestade, remove o peso extra.
            Debug.Log($"<color=blue>[WeatherManager]</color> A tempestade acabou! Removendo peso extra.");
            RaftStatusManager.Instance.RemoveWeight(STORM_WEIGHT_PENALTY);
        }

        // 2. Avança para o próximo dia.
        CurrentDay++;

        if (CurrentDay >= fullForecast.Length)
        {
            Debug.Log("Fim da previsão de 30 dias.");
            return;
        }

        // 3. Pega os dados e anuncia o NOVO dia.
        WeatherData newDayData = GetDataForDay(0);
        if (newDayData != null)
        {
            WeatherUIManager.Instance.ShowDayAnnouncement(newDayData);

            if (newDayData.type == WeatherType.Heavy)
            {
                isHeavyDay = true; // Liga o evento
                ResetCrabTimer();
            }

            // 4. Verifica se o NOVO dia é uma tempestade.
            if (newDayData.type == WeatherType.Storm)
            {
                // Se for, adiciona o peso extra.
                Debug.Log($"<color=blue>[WeatherManager]</color> Uma nova tempestade começou! Adicionando peso extra.");
                RaftStatusManager.Instance.AddWeight(STORM_WEIGHT_PENALTY);
            }
        }
    }
}