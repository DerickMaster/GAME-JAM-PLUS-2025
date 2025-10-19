using UnityEngine;
using System.Collections.Generic; // Necess�rio para usar Listas

public class WeatherManager : MonoBehaviour
{
    public static WeatherManager Instance { get; private set; }

    [Header("Configura��o da Previs�o")]
    [Tooltip("A lista completa de todos os poss�veis WeatherData que o jogo pode usar.")]
    [SerializeField] private List<WeatherData> allWeatherData;
    [Tooltip("A sequ�ncia de climas para os pr�ximos dias.")]
    public WeatherType[] forecast = new WeatherType[6];

    public int CurrentDay { get; private set; } = 0;
    public int RadioCount { get; private set; } = 0; // Contador de r�dios

    private Dictionary<WeatherType, WeatherData> weatherDataMap;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        // Cria um "dicion�rio" para acesso r�pido aos dados do clima.
        weatherDataMap = new Dictionary<WeatherType, WeatherData>();
        foreach (var data in allWeatherData)
        {
            weatherDataMap[data.type] = data;
        }
    }

    // --- Fun��es para os R�dios ---
    public void RegisterRadio()
    {
        RadioCount++;
        Debug.Log($"R�dio constru�do! Total de r�dios: {RadioCount}");
    }

    public void UnregisterRadio()
    {
        RadioCount--;
        Debug.Log($"R�dio destru�do! Total de r�dios: {RadioCount}");
    }

    // --- Fun��es para a UI e o Jogo ---
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
            // O que fazer quando a previs�o acaba? Por enquanto, vamos parar.
            Debug.Log("Fim da previs�o.");
            return;
        }

        WeatherData todayData = GetDataForDay(CurrentDay);
        if (todayData != null)
        {
            // AQUI chamaremos a fun��o da UI para mostrar a mensagem.
            WeatherUIManager.Instance.ShowDayAnnouncement(todayData);
            Debug.Log($"Novo dia come�ou! Hoje �: {todayData.displayName_PT}");
        }
    }
}