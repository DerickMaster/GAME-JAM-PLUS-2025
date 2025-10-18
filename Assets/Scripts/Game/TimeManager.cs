using UnityEngine;

public class TimeManager : MonoBehaviour
{
    // Singleton para acesso f�cil de outros scripts
    public static TimeManager Instance { get; private set; }

    [Tooltip("Dura��o de um dia no jogo, em segundos. 2 minutos = 120 segundos.")]
    public float secondsPerDay = 120f;

    // Propriedades para saber o dia atual e o tempo corrido no dia
    public int CurrentDay { get; private set; }
    public float DayTimer { get; private set; }

    private void Awake()
    {
        // Configura��o do Singleton
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

        // Exibe a previs�o inicial no console
        Debug.Log($"O jogo come�ou! Previs�o para o Dia {CurrentDay}: {WeatherManager.Instance.GetCurrentWeather()}");
    }

    void Update()
    {
        // Acumula o tempo que passou desde o �ltimo frame
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

        // Avisa o WeatherManager para avan�ar para a previs�o do pr�ximo dia
        if (WeatherManager.Instance != null)
        {
            WeatherManager.Instance.AdvanceToNextDay();
        }
        else
        {
            Debug.LogWarning("TimeManager tentou avan�ar o dia, mas n�o encontrou um WeatherManager!");
        }

        Debug.Log($"O Dia {CurrentDay} come�ou! A previs�o �: {WeatherManager.Instance.GetCurrentWeather()}");
    }
}