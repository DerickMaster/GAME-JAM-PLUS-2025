using UnityEngine;

public class TimeManager : MonoBehaviour
{
    // Singleton para acesso fácil de outros scripts
    public static TimeManager Instance { get; private set; }

    [Tooltip("Duração de um dia no jogo, em segundos. 2 minutos = 120 segundos.")]
    public float secondsPerDay = 120f;

    // Propriedades para saber o dia atual e o tempo corrido no dia
    public int CurrentDay { get; private set; }
    public float DayTimer { get; private set; }

    private void Awake()
    {
        // Configuração do Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // Inicia o jogo no dia 0
        CurrentDay = 0;
        DayTimer = 0f;

        // Exibe a previsão inicial no console
        Debug.Log($"O jogo começou! Previsão para o Dia {CurrentDay}: {WeatherManager.Instance.GetCurrentWeather()}");
    }

    void Update()
    {
        // Acumula o tempo que passou desde o último frame
        DayTimer += Time.deltaTime;

        // Verifica se o tempo do dia acabou
        if (DayTimer >= secondsPerDay)
        {
            AdvanceDay();
        }
    }

    private void AdvanceDay()
    {
        // Reseta o timer do dia
        DayTimer = 0f;
        // Incrementa o contador de dias
        CurrentDay++;

        // Avisa o WeatherManager para avançar para a previsão do próximo dia
        if (WeatherManager.Instance != null)
        {
            WeatherManager.Instance.AdvanceToNextDay();
        }
        else
        {
            Debug.LogWarning("TimeManager tentou avançar o dia, mas não encontrou um WeatherManager!");
        }

        Debug.Log($"O Dia {CurrentDay} começou! A previsão é: {WeatherManager.Instance.GetCurrentWeather()}");
    }
}