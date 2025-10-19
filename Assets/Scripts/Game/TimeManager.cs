using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [Tooltip("Dura��o de um dia no jogo, em segundos.")]
    public float secondsPerDay = 120f;

    public int CurrentDay { get; private set; }
    public float DayTimer { get; private set; }

    private void Awake()
    {
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
        CurrentDay = 0;
        DayTimer = 0f;

        // --- L�GICA CORRIGIDA AQUI ---
        // 1. Pega os dados completos do clima para o dia atual.
        WeatherData todayData = WeatherManager.Instance.GetDataForDay(CurrentDay);

        // 2. Garante que os dados existem antes de tentar us�-los.
        if (todayData != null)
        {
            // 3. Usa o nome em portugu�s (displayName_PT) para a mensagem.
            Debug.Log($"O jogo come�ou! Previs�o para o Dia {CurrentDay}: {todayData.displayName_PT}");
        }
    }

    void Update()
    {
        DayTimer += Time.deltaTime;
        if (DayTimer >= secondsPerDay)
        {
            AdvanceDay();
        }
    }

    private void AdvanceDay()
    {
        DayTimer = 0f;
        // N�o incrementamos o dia aqui, deixamos o WeatherManager fazer isso.

        if (WeatherManager.Instance != null)
        {
            WeatherManager.Instance.AdvanceToNextDay();
            // A mensagem de "novo dia" agora � mostrada pelo WeatherUIManager.
        }
        else
        {
            Debug.LogWarning("TimeManager tentou avan�ar o dia, mas n�o encontrou um WeatherManager!");
        }
    }
}