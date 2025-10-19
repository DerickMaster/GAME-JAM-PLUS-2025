using UnityEngine;
using System.Collections.Generic;

public class WeatherManager : MonoBehaviour
{
    public static WeatherManager Instance { get; private set; }

    [Header("Configuração da Previsão")]
    [Tooltip("A lista completa de todos os possíveis WeatherData que o jogo pode usar.")]
    [SerializeField] private List<WeatherData> allWeatherData;

    // --- MUDANÇA PRINCIPAL AQUI ---
    [Tooltip("A sequência completa de climas para todos os 30 dias.")]
    [SerializeField] private WeatherType[] fullForecast = new WeatherType[30];

    // 'CurrentDay' agora é um ponteiro para a posição atual na lista de 30 dias.
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

    // --- Funções para os Rádios (não mudam) ---
    public void RegisterRadio() { RadioCount++; }
    public void UnregisterRadio() { RadioCount--; }

    // --- FUNÇÃO CORRIGIDA ---
    // 'dayIndex' agora é um "deslocamento". 0 = hoje, 1 = amanhã, 2 = depois de amanhã, etc.
    public WeatherData GetDataForDay(int dayIndex)
    {
        // Calcula o dia real na previsão de 30 dias.
        int actualDay = CurrentDay + dayIndex;

        // Checagem de segurança para garantir que não estamos tentando ler além do fim da previsão.
        if (actualDay < 0 || actualDay >= fullForecast.Length) return null;

        WeatherType type = fullForecast[actualDay];
        return weatherDataMap.ContainsKey(type) ? weatherDataMap[type] : null;
    }

    // A função para avançar o dia agora simplesmente "pula uma casa" no ponteiro.
    public void AdvanceToNextDay()
    {
        CurrentDay++; // Amanhã se torna o novo "hoje".

        if (CurrentDay >= fullForecast.Length)
        {
            Debug.Log("Fim da previsão de 30 dias.");
            // Aqui podemos adicionar lógica para o fim do jogo ou para gerar mais dias aleatoriamente.
            return;
        }

        // Pega os dados do NOVO dia atual para o anúncio.
        WeatherData todayData = GetDataForDay(0); // '0' sempre significa "hoje".
        if (todayData != null)
        {
            WeatherUIManager.Instance.ShowDayAnnouncement(todayData);
            Debug.Log($"Novo dia começou! Hoje ({CurrentDay}) é: {todayData.displayName_PT}");
        }
    }
}