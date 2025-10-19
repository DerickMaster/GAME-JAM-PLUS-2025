using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class WeatherUIManager : MonoBehaviour
{
    public static WeatherUIManager Instance { get; private set; }

    [Header("UI da Previs�o (Forecast)")]
    [Tooltip("Os 6 elementos de UI para os dias. Arraste-os na ordem correta.")]
    [SerializeField] private ForecastDayUI[] forecastDays = new ForecastDayUI[6];

    [Header("UI do An�ncio do Dia")]
    [Tooltip("O painel que cont�m a mensagem de an�ncio.")]
    [SerializeField] private GameObject announcementPanel;
    [Tooltip("O texto para o t�tulo do an�ncio (ex: 'Dia da Tempestade').")]
    [SerializeField] private TMP_Text announcementTitleText;
    [Tooltip("O texto para a dica do an�ncio (ex: 'Construa um para-raios!').")]
    [SerializeField] private TMP_Text announcementHintText;
    [Tooltip("Quanto tempo (em segundos) o an�ncio fica na tela.")]
    [SerializeField] private float announcementDuration = 4f;

    void Awake()
    {
        // Configura��o do padr�o Singleton
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
        // ESTE � O NOSSO TESTE. Se esta mensagem n�o aparecer, o script nunca foi ativado.
        Debug.Log("<color=lime>[WeatherUIManager]</color> SCRIPT INICIADO E ATIVO!");

        if (announcementPanel != null)
            announcementPanel.SetActive(false);

        // Chamada inicial
        UpdateForecastUI();
    }

    void Update()
    {
        // Esta fun��o garante que a UI esteja sempre sincronizada com o n�mero de r�dios.
        UpdateForecastUI();
    }

    public void UpdateForecastUI()
    {
        if (WeatherManager.Instance == null) return;

        // Calcula quantos dias o jogador pode ver (1 por padr�o + 1 para cada r�dio).
        int daysVisible = 1 + WeatherManager.Instance.RadioCount;

        for (int i = 0; i < forecastDays.Length; i++)
        {
            // Checagem de seguran�a para evitar erros se a UI n�o estiver configurada no Inspector.
            if (forecastDays[i] == null || forecastDays[i].dayText == null || forecastDays[i].iconImage == null)
            {
                Debug.LogError($"[WeatherUI] ERRO: O 'Forecast Day' no �ndice {i} n�o est� configurado corretamente no Inspector!");
                continue; // Pula para o pr�ximo dia para evitar mais erros.
            }

            if (i < daysVisible)
            {
                // Mostra a informa��o do dia.
                WeatherData data = WeatherManager.Instance.GetDataForDay(i);
                if (data != null)
                {
                    forecastDays[i].SetData(data.icon, data.displayName_PT);
                }
            }
            else
            {
                // Esconde a informa��o do dia.
                forecastDays[i].SetHidden();
            }
        }
    }

    // Chamada pelo WeatherManager quando um novo dia come�a.
    public void ShowDayAnnouncement(WeatherData data)
    {
        StartCoroutine(AnnouncementSequence(data));
    }

    // Corrotina para mostrar o painel de an�ncio por um tempo e depois escond�-lo.
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

// Classe auxiliar para organizar as refer�ncias da UI de cada dia do forecast.
[System.Serializable]
public class ForecastDayUI
{
    [Tooltip("A imagem para o �cone do clima.")]
    public Image iconImage;
    [Tooltip("O texto que mostrar� o nome do clima OU '????'.")]
    public TMP_Text dayText;

    // Fun��o para mostrar os dados do clima.
    public void SetData(Sprite icon, string name)
    {
        // Ativa o �cone e define a imagem.
        iconImage.gameObject.SetActive(true);
        iconImage.sprite = icon;

        // Define o texto com o nome do clima.
        dayText.text = name;
    }

    // Fun��o para esconder os dados do clima.
    public void SetHidden()
    {

        // Apenas muda o texto para "????", sem desativar o objeto de texto.
        dayText.text = "????";
    }
}