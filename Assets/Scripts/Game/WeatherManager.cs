using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // Para a lógica de vitória/derrota
using System.Collections;

public class WeatherManager : MonoBehaviour
{
    public static WeatherManager Instance { get; private set; }

    [Header("Configuração da Previsão")]
    [Tooltip("A lista completa de todos os possíveis WeatherData que o jogo pode usar.")]
    [SerializeField] private List<WeatherData> allWeatherData;
    [Tooltip("A sequência completa de climas para todos os 30 dias.")]
    [SerializeField] private WeatherType[] fullForecast = new WeatherType[30];

    [Header("Condição de Vitória")]
    [Tooltip("O dia em que o jogador vence o jogo (ex: 15 para o 15º dia).")]
    [SerializeField] private int winDay = 15;

    [Header("Timers de Eventos (Intervalo em Segundos)")]
    [SerializeField] private float minCrabAttackInterval = 10f;
    [SerializeField] private float maxCrabAttackInterval = 30f;
    [SerializeField] private float minFishAttackInterval = 10f;
    [SerializeField] private float maxFishAttackInterval = 30f;
    [SerializeField] private float minLightningInterval = 5f;
    [SerializeField] private float maxLightningInterval = 15f;

    [Header("Efeitos dos Eventos")]
    [SerializeField] private GameObject raftStrikeEffect;
    [SerializeField] private GameObject rainParticleEffect;
    [SerializeField] private GameObject rainSoundEmitter;

    private const int STORM_WEIGHT_PENALTY = 3;

    public int CurrentDay { get; private set; } = 0;
    public int RadioCount { get; private set; } = 0;

    // Dicionário para acesso rápido aos dados do clima
    private Dictionary<WeatherType, WeatherData> weatherDataMap;

    // --- Flags de Estado (controlam o Update) ---
    private bool isHeavyDay = false;
    private bool isCuttersDay = false;
    private bool isStormDay = false;
    private float crabAttackTimer;
    private float fishAttackTimer;
    private float lightningStrikeTimer;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        // Preenche o dicionário para acesso rápido
        weatherDataMap = new Dictionary<WeatherType, WeatherData>();
        foreach (var data in allWeatherData)
        {
            weatherDataMap[data.type] = data;
        }
    }

    void Start()
    {
        // Garante que todos os efeitos comecem desligados
        if (rainParticleEffect != null) rainParticleEffect.SetActive(false);
        if (rainSoundEmitter != null) rainSoundEmitter.SetActive(false);
        if (raftStrikeEffect != null) raftStrikeEffect.SetActive(false);

        // Aplica os efeitos do primeiro dia
        StartDayEffects(GetDataForDay(0));
    }

    void Update()
    {
        // O Update agora é muito limpo. Ele apenas gerencia os timers dos eventos que estão ATIVOS.

        if (isHeavyDay)
        {
            crabAttackTimer -= Time.deltaTime;
            if (crabAttackTimer <= 0)
            {
                if (CrabController.Instance != null && !CrabController.Instance.gameObject.activeSelf)
                {
                    CrabController.Instance.StartAttackSequence();
                }
                ResetCrabTimer();
            }
        }

        if (isCuttersDay)
        {
            fishAttackTimer -= Time.deltaTime;
            if (fishAttackTimer <= 0)
            {
                if (FishSwarmController.Instance != null && !FishSwarmController.Instance.gameObject.activeSelf)
                {
                    FishSwarmController.Instance.StartAttackSequence();
                }
                ResetFishTimer();
            }
        }

        if (isStormDay)
        {
            lightningStrikeTimer -= Time.deltaTime;
            if (lightningStrikeTimer <= 0)
            {
                TriggerLightningStrike();
                ResetLightningTimer();
            }
        }
    }

    // --- FUNÇÃO DE AVANÇO DE DIA (AGORA É O "GERENTE") ---
    public void AdvanceToNextDay()
    {
        // 1. Pega os dados do dia que está terminando e encerra seus efeitos.
        WeatherData endedDayData = GetDataForDay(0);
        if (endedDayData != null)
        {
            EndDayEffects(endedDayData);
        }

        // 2. Avança o contador do dia.
        CurrentDay++;

        // 3. Checa as condições de vitória ou fim da previsão.
        if (CurrentDay >= winDay)
        {
            WinGame();
            return;
        }
        if (CurrentDay >= fullForecast.Length)
        {
            Debug.Log("Fim da previsão de 30 dias.");
            return;
        }

        // 4. Pega os dados do novo dia e inicia seus efeitos.
        WeatherData newDayData = GetDataForDay(0);
        if (newDayData != null)
        {
            StartDayEffects(newDayData);
        }
    }

    // --- NOVA FUNÇÃO (BOA PRÁTICA) ---
    // Limpa todos os efeitos do dia que acabou.
    private void EndDayEffects(WeatherData endedDayData)
    {
        Debug.Log($"[WeatherManager] Encerrando o dia {CurrentDay}: {endedDayData.displayName_PT}");

        // Desliga as flags de evento
        isHeavyDay = false;
        isCuttersDay = false;
        isStormDay = false;

        // Reverte os efeitos da Tempestade (se o dia que acabou era uma)
        if (endedDayData.type == WeatherType.Storm)
        {
            RaftStatusManager.Instance.RemoveWeight(STORM_WEIGHT_PENALTY);
            LightingManager.Instance.SetNormalLighting();
            if (rainParticleEffect != null) rainParticleEffect.SetActive(false);
            if (rainSoundEmitter != null) rainSoundEmitter.SetActive(false);
        }
    }

    // --- NOVA FUNÇÃO (BOA PRÁTICA) ---
    // Configura e ativa todos os efeitos do novo dia.
    private void StartDayEffects(WeatherData todayData)
    {
        if (todayData == null) return;

        Debug.Log($"[WeatherManager] Iniciando o dia {CurrentDay}: {todayData.displayName_PT}");

        // Anuncia o novo dia (só não no primeiro frame do jogo)
        if (Time.time > 0)
        {
            WeatherUIManager.Instance.ShowDayAnnouncement(todayData);
        }

        // Ativa os efeitos com base no tipo de clima
        switch (todayData.type)
        {
            case WeatherType.Storm:
                isStormDay = true;
                ResetLightningTimer();
                LightingManager.Instance.SetStormLighting();
                RaftStatusManager.Instance.AddWeight(STORM_WEIGHT_PENALTY);
                if (rainParticleEffect != null) rainParticleEffect.SetActive(true);
                if (rainSoundEmitter != null) rainSoundEmitter.SetActive(true);
                break;

            case WeatherType.Heavy:
                isHeavyDay = true;
                ResetCrabTimer();
                LightingManager.Instance.SetNormalLighting(); // Garante que a luz esteja normal
                break;

            case WeatherType.Cutters:
                isCuttersDay = true;
                ResetFishTimer();
                LightingManager.Instance.SetNormalLighting(); // Garante que a luz esteja normal
                break;

            case WeatherType.Calm:
            default:
                // Garante que a iluminação volte ao normal em dias calmos.
                LightingManager.Instance.SetNormalLighting();
                break;
        }
    }

    private void WinGame()
    {
        Debug.Log($"<color=yellow>VITÓRIA!</color> O jogador sobreviveu até o dia {CurrentDay}.");
        Time.timeScale = 1f;
        SceneManager.LoadScene("WinScreen");
    }

    // --- Funções de busca e timers (sem mudanças) ---
    public WeatherData GetDataForDay(int dayIndex)
    {
        int actualDay = CurrentDay + dayIndex;
        if (actualDay < 0 || actualDay >= fullForecast.Length) return null;
        WeatherType type = fullForecast[actualDay];
        return weatherDataMap.ContainsKey(type) ? weatherDataMap[type] : null;
    }

    public void RegisterRadio() { RadioCount++; }
    public void UnregisterRadio() { RadioCount--; }

    private void TriggerLightningStrike()
    {
        LightningRodController availableRod = ConstructionManager.Instance.GetFirstAvailableLightningRod();
        if (availableRod != null)
        {
            availableRod.AbsorbStrike();
        }
        else
        {
            StartCoroutine(RaftStrikeSequence());
        }
    }

    private IEnumerator RaftStrikeSequence()
    {
        if (raftStrikeEffect == null) yield break;
        raftStrikeEffect.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        GridManager.Instance.BreakRandomSlots(2);
        raftStrikeEffect.SetActive(false);
    }

    private void ResetCrabTimer() { crabAttackTimer = Random.Range(minCrabAttackInterval, maxCrabAttackInterval); }
    private void ResetFishTimer() { fishAttackTimer = Random.Range(minFishAttackInterval, maxFishAttackInterval); }
    private void ResetLightningTimer() { lightningStrikeTimer = Random.Range(minLightningInterval, maxLightningInterval); }
}