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
    [SerializeField] private GameObject announcementPanel;
    [SerializeField] private TMP_Text announcementTitleText;
    [SerializeField] private TMP_Text announcementHintText;
    [SerializeField] private float announcementDuration = 4f;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Start()
    {
        if (announcementPanel != null)
            announcementPanel.SetActive(false);
        UpdateForecastUI();
    }

    void Update()
    {
        UpdateForecastUI();
    }

    public void UpdateForecastUI()
    {
        if (WeatherManager.Instance == null) return;

        int daysVisible = 1 + WeatherManager.Instance.RadioCount;

        for (int i = 0; i < forecastDays.Length; i++)
        {
            if (i < daysVisible)
            {
                // Mostra a informação do dia
                WeatherData data = WeatherManager.Instance.GetDataForDay(i);
                if (data != null)
                {
                    forecastDays[i].SetData(data.icon, data.displayName_PT);
                }
            }
            else
            {
                // Esconde a informação do dia
                forecastDays[i].SetHidden();
            }
        }
    }

    public void ShowDayAnnouncement(WeatherData data)
    {
        StartCoroutine(AnnouncementSequence(data));
    }

    private IEnumerator AnnouncementSequence(WeatherData data)
    {
        announcementTitleText.text = "Dia do " + data.displayName_PT;
        announcementHintText.text = data.hintText_PT;
        announcementPanel.SetActive(true);

        yield return new WaitForSeconds(announcementDuration);

        announcementPanel.SetActive(false);
    }
}

// --- A LÓGICA CORRIGIDA ESTÁ AQUI ---
[System.Serializable]
public class ForecastDayUI
{
    [Tooltip("A imagem para o ícone do clima.")]
    public Image iconImage;
    [Tooltip("O texto que mostrará o nome do clima OU '????'.")]
    public TMP_Text dayText; // Renomeado para mais clareza

    public void SetData(Sprite icon, string name)
    {
        // Mostra o ícone e define a imagem e o texto corretos.
        iconImage.gameObject.SetActive(true);
        iconImage.sprite = icon;
        dayText.text = name;
    }

    public void SetHidden()
    {
        // Esconde o ícone e apenas troca o texto.
        iconImage.gameObject.SetActive(false);
        dayText.text = "????";
    }
}