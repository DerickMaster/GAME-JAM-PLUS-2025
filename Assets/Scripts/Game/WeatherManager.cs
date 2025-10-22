using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

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

    [Header("Evento 'Cortadores'")]
    [SerializeField] private float minFishAttackInterval = 10f;
    [SerializeField] private float maxFishAttackInterval = 30f;
    private float fishAttackTimer;
    private bool isCuttersDay = false;

    [Header("Evento 'Tempestade'")]
    [SerializeField] private float minLightningInterval = 5f;
    [SerializeField] private float maxLightningInterval = 15f;
    [SerializeField] private GameObject raftStrikeEffect; // O efeito de raio que atinge a balsa
    private float lightningStrikeTimer;

    [SerializeField] private int winDay = 15;

    [Header("Efeitos de Tempestade")]
    [Tooltip("O objeto que contém o sistema de partículas da chuva.")]
    [SerializeField] private GameObject rainParticleEffect;
    [Tooltip("O objeto que contém o FMOD Emitter para o som da chuva.")]
    [SerializeField] private GameObject rainSoundEmitter;
    private const int STORM_WEIGHT_PENALTY = 3;

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

        if (GetDataForDay(0)?.type == WeatherType.Storm)
        {
            lightningStrikeTimer -= Time.deltaTime;
            if (lightningStrikeTimer <= 0)
            {
                TriggerLightningStrike();
                ResetLightningTimer();
            }
        }
    }

    private void TriggerLightningStrike()
    {
        // 1. Pergunta se existe um para-raios funcional.
        LightningRodController availableRod = ConstructionManager.Instance.GetFirstAvailableLightningRod();

        if (availableRod != null)
        {
            // SUCESSO: O jogador tem defesa.
            Debug.Log("[WeatherManager] Para-raios encontrado! O raio será absorvido.");
            availableRod.AbsorbStrike();
        }
        else
        {
            // FALHA: O jogador está indefeso.
            Debug.LogWarning("[WeatherManager] Nenhum para-raios funcional! A balsa será atingida.");
            StartCoroutine(RaftStrikeSequence());
        }
    }

    private IEnumerator RaftStrikeSequence()
    {
        if (raftStrikeEffect == null) yield break;

        raftStrikeEffect.SetActive(true);
        // Aqui você pode adicionar um som de raio atingindo a balsa.
        yield return new WaitForSeconds(1.5f); // Duração do efeito visual.

        GridManager.Instance.BreakRandomSlots(2); // Quebra 2 slots.

        raftStrikeEffect.SetActive(false);
    }

    private void ResetLightningTimer()
    {
        lightningStrikeTimer = Random.Range(minLightningInterval, maxLightningInterval);
    }

    private void ResetCrabTimer()
    {
        crabAttackTimer = Random.Range(minCrabAttackInterval, maxCrabAttackInterval);
    }

    private void ResetFishTimer()
    {
        fishAttackTimer = Random.Range(minFishAttackInterval, maxFishAttackInterval);
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

        if (rainParticleEffect != null) rainParticleEffect.SetActive(false);
        if (rainSoundEmitter != null) rainSoundEmitter.SetActive(false);

        if (raftStrikeEffect != null) raftStrikeEffect.SetActive(false); // Garante que comece desligado.
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

    private void ApplyWeatherEffectsForCurrentDay()
    {
        WeatherData todayData = GetDataForDay(0);
        if (todayData == null) return;

        // Anuncia o novo dia (se não for o primeiro frame do jogo)
        if (Time.time > 0) // Time.time é 0 no primeiro frame
        {
            WeatherUIManager.Instance.ShowDayAnnouncement(todayData);
        }

        bool isStormy = todayData.type == WeatherType.Storm;

        if (isStormy)
        {
            LightingManager.Instance.SetStormLighting();
            RaftStatusManager.Instance.AddWeight(STORM_WEIGHT_PENALTY);
        }
        else
        {
            LightingManager.Instance.SetNormalLighting();
        }


        if (rainParticleEffect != null) rainParticleEffect.SetActive(isStormy);
        if (rainSoundEmitter != null) rainSoundEmitter.SetActive(isStormy);

        // --- LÓGICA DA TEMPESTADE ---
        if (todayData.type == WeatherType.Storm)
        {
            ResetLightningTimer();
            LightingManager.Instance.SetStormLighting();
            RaftStatusManager.Instance.AddWeight(STORM_WEIGHT_PENALTY);
        }

        else
        {
            // Garante que a iluminação volte ao normal se não for tempestade.
            LightingManager.Instance.SetNormalLighting();
        }

        // --- LÓGICA DO PESADO ---
        if (todayData.type == WeatherType.Heavy)
        {
            isHeavyDay = true;
            ResetCrabTimer();
        }

        // --- LÓGICA DOS CORTADORES ---
        if (todayData.type == WeatherType.Cutters)
        {
            isCuttersDay = true;
            ResetFishTimer();
        }
    }

    public void AdvanceToNextDay()
    {

        if (CurrentDay >= winDay)
        {
            WinGame();
            return; // Impede que o resto da função seja executado.
        }

        WeatherData endedDayData = GetDataForDay(0);
        if (endedDayData != null && endedDayData.type == WeatherType.Storm)
        {
            if (endedDayData.type == WeatherType.Heavy) isHeavyDay = false;
            if (endedDayData.type == WeatherType.Cutters) isCuttersDay = false;
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

            else if (newDayData.type == WeatherType.Cutters) // Liga o evento
            {
                isCuttersDay = true;
                ResetFishTimer();
            }

            ApplyWeatherEffectsForCurrentDay();
        }
    }

    private void WinGame()
    {
        Debug.Log($"<color=yellow>VITÓRIA!</color> O jogador sobreviveu até o dia {CurrentDay}.");
        Time.timeScale = 1f; // Garante que o tempo não esteja pausado.
        SceneManager.LoadScene("WinScreen");
    }
}