using UnityEngine;
using System.Collections.Generic;

public class WeatherManager : MonoBehaviour
{
    public static WeatherManager Instance { get; private set; }

    [Header("Configura��o da Previs�o")]
    [Tooltip("A lista completa de todos os poss�veis WeatherData que o jogo pode usar.")]
    [SerializeField] private List<WeatherData> allWeatherData;

    // --- MUDAN�A PRINCIPAL AQUI ---
    [Tooltip("A sequ�ncia completa de climas para todos os 30 dias.")]
    [SerializeField] private WeatherType[] fullForecast = new WeatherType[30];

    // 'CurrentDay' agora � um ponteiro para a posi��o atual na lista de 30 dias.
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

    // --- Fun��es para os R�dios (n�o mudam) ---
    public void RegisterRadio() { RadioCount++; }
    public void UnregisterRadio() { RadioCount--; }

    // --- FUN��O CORRIGIDA ---
    // 'dayIndex' agora � um "deslocamento". 0 = hoje, 1 = amanh�, 2 = depois de amanh�, etc.
    public WeatherData GetDataForDay(int dayIndex)
    {
        // Calcula o dia real na previs�o de 30 dias.
        int actualDay = CurrentDay + dayIndex;

        // Checagem de seguran�a para garantir que n�o estamos tentando ler al�m do fim da previs�o.
        if (actualDay < 0 || actualDay >= fullForecast.Length) return null;

        WeatherType type = fullForecast[actualDay];
        return weatherDataMap.ContainsKey(type) ? weatherDataMap[type] : null;
    }

    // A fun��o para avan�ar o dia agora simplesmente "pula uma casa" no ponteiro.
    public void AdvanceToNextDay()
    {
        CurrentDay++; // Amanh� se torna o novo "hoje".

        if (CurrentDay >= fullForecast.Length)
        {
            Debug.Log("Fim da previs�o de 30 dias.");
            // Aqui podemos adicionar l�gica para o fim do jogo ou para gerar mais dias aleatoriamente.
            return;
        }

        // Pega os dados do NOVO dia atual para o an�ncio.
        WeatherData todayData = GetDataForDay(0); // '0' sempre significa "hoje".
        if (todayData != null)
        {
            WeatherUIManager.Instance.ShowDayAnnouncement(todayData);
            Debug.Log($"Novo dia come�ou! Hoje ({CurrentDay}) �: {todayData.displayName_PT}");
        }
    }
}