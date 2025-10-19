using UnityEngine;
using System.Collections.Generic; // Necessário para usar Listas

public class WeatherManager : MonoBehaviour
{
    public static WeatherManager Instance { get; private set; }

    [Header("Configuração da Previsão")]
    [Tooltip("A lista completa de todos os possíveis WeatherData que o jogo pode usar.")]
    [SerializeField] private List<WeatherData> allWeatherData;
    [Tooltip("A sequência de climas para os próximos dias.")]
    public WeatherType[] forecast = new WeatherType[6];

    public int CurrentDay { get; private set; } = 0;
    public int RadioCount { get; private set; } = 0; // Contador de rádios

    private Dictionary<WeatherType, WeatherData> weatherDataMap;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        // Cria um "dicionário" para acesso rápido aos dados do clima.
        weatherDataMap = new Dictionary<WeatherType, WeatherData>();
        foreach (var data in allWeatherData)
        {
            weatherDataMap[data.type] = data;
        }
    }

    // --- Funções para os Rádios ---
    public void RegisterRadio()
    {
        RadioCount++;
        Debug.Log($"Rádio construído! Total de rádios: {RadioCount}");
    }

    public void UnregisterRadio()
    {
        RadioCount--;
        Debug.Log($"Rádio destruído! Total de rádios: {RadioCount}");
    }

    // --- Funções para a UI e o Jogo ---
    public WeatherData GetDataForDay(int dayIndex)
    {
        if (dayIndex < 0 || dayIndex >= forecast.Length) return null;

        WeatherType type = forecast[dayIndex];
        return weatherDataMap.ContainsKey(type) ? weatherDataMap[type] : null;
    }

    public void AdvanceToNextDay()
    {
        CurrentDay++;
        if (CurrentDay >= forecast.Length)
        {
            // O que fazer quando a previsão acaba? Por enquanto, vamos parar.
            Debug.Log("Fim da previsão.");
            return;
        }

        WeatherData todayData = GetDataForDay(CurrentDay);
        if (todayData != null)
        {
            // AQUI chamaremos a função da UI para mostrar a mensagem.
            WeatherUIManager.Instance.ShowDayAnnouncement(todayData);
            Debug.Log($"Novo dia começou! Hoje é: {todayData.displayName_PT}");
        }
    }
}