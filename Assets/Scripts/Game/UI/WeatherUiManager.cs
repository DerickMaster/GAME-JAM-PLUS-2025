using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class WeatherUIManager : MonoBehaviour
{
    public static WeatherUIManager Instance { get; private set; }

    [Header("UI da Previsão (Forecast)")]
    [Tooltip("Os 6 elementos de UI para os dias. Arraste-os na ordem correta.")]
    [SerializeField] private ForecastDayUI[] forecastDays = new ForecastDayUI[6];

    [Header("UI do Anúncio do Dia")]
    [Tooltip("O painel que contém a mensagem de anúncio.")]
    [SerializeField] private GameObject announcementPanel;
    [Tooltip("O texto para o título do anúncio (ex: 'Dia da Tempestade').")]
    [SerializeField] private TMP_Text announcementTitleText;
    [Tooltip("O texto para a dica do anúncio (ex: 'Construa um para-raios!').")]
    [SerializeField] private TMP_Text announcementHintText;
    [Tooltip("Quanto tempo (em segundos) o anúncio fica na tela.")]
    [SerializeField] private float announcementDuration = 4f;

    void Awake()
    {
        // Configuração do padrão Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        // ESTE É O NOSSO TESTE. Se esta mensagem não aparecer, o script nunca foi ativado.
        Debug.Log("<color=lime>[WeatherUIManager]</color> SCRIPT INICIADO E ATIVO!");

        if (announcementPanel != null)
            announcementPanel.SetActive(false);

        // Chamada inicial
        UpdateForecastUI();
    }

    void Update()
    {
        // Esta função garante que a UI esteja sempre sincronizada com o número de rádios.
        UpdateForecastUI();
    }

    public void UpdateForecastUI()
    {
        if (WeatherManager.Instance == null) return;

        // Calcula quantos dias o jogador pode ver (1 por padrão + 1 para cada rádio).
        int daysVisible = 1 + WeatherManager.Instance.RadioCount;

        for (int i = 0; i < forecastDays.Length; i++)
        {
            // Checagem de segurança para evitar erros se a UI não estiver configurada no Inspector.
            if (forecastDays[i] == null || forecastDays[i].dayText == null || forecastDays[i].iconImage == null)
            {
                Debug.LogError($"[WeatherUI] ERRO: O 'Forecast Day' no índice {i} não está configurado corretamente no Inspector!");
                continue; // Pula para o próximo dia para evitar mais erros.
            }

            if (i < daysVisible)
            {
                // Mostra a informação do dia.
                WeatherData data = WeatherManager.Instance.GetDataForDay(i);
                if (data != null)
                {
                    forecastDays[i].SetData(data.icon, data.displayName_PT);
                }
            }
            else
            {
                // Esconde a informação do dia.
                forecastDays[i].SetHidden();
            }
        }
    }

    // Chamada pelo WeatherManager quando um novo dia começa.
    public void ShowDayAnnouncement(WeatherData data)
    {
        StartCoroutine(AnnouncementSequence(data));
    }

    // Corrotina para mostrar o painel de anúncio por um tempo e depois escondê-lo.
    private IEnumerator AnnouncementSequence(WeatherData data)
    {
        if (announcementPanel == null) yield break;

        announcementTitleText.text = "Dia do " + data.displayName_PT;
        announcementHintText.text = data.hintText_PT;
        announcementPanel.SetActive(true);

        yield return new WaitForSeconds(announcementDuration);

        announcementPanel.SetActive(false);
    }
}

// Classe auxiliar para organizar as referências da UI de cada dia do forecast.
[System.Serializable]
public class ForecastDayUI
{
    [Tooltip("A imagem para o ícone do clima.")]
    public Image iconImage;
    [Tooltip("O texto que mostrará o nome do clima OU '????'.")]
    public TMP_Text dayText;

    // Função para mostrar os dados do clima.
    public void SetData(Sprite icon, string name)
    {
        // Ativa o ícone e define a imagem.
        iconImage.gameObject.SetActive(true);
        iconImage.sprite = icon;

        // Define o texto com o nome do clima.
        dayText.text = name;
    }

    // Função para esconder os dados do clima.
    public void SetHidden()
    {

        // Apenas muda o texto para "????", sem desativar o objeto de texto.
        dayText.text = "????";
    }
}