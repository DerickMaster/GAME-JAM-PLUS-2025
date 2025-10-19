using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RaftStatusUI : MonoBehaviour
{
    [Header("Referências da Barra")]
    [SerializeField] private Slider powerWeightSlider;
    [SerializeField] private Image sliderFillImage;
    [SerializeField] private TMP_Text statusText;

    // --- NOVAS REFERÊNCIAS ---
    [Header("Referências do Alerta de Sobrecarga")]
    [Tooltip("O objeto (painel) que contém a mensagem de alerta. Ele será ativado quando houver sobrecarga.")]
    [SerializeField] private GameObject overloadWarningObject;
    [Tooltip("O componente de texto TextMeshPro para mostrar o countdown de 30 segundos.")]
    [SerializeField] private TMP_Text countdownText;
    // -------------------------

    [Header("Cores de Feedback")]
    [SerializeField] private Color normalColor = new Color(0, 0.8f, 1f);
    [SerializeField] private Color overloadedColor = new Color(1f, 0.2f, 0f);

    void Start()
    {
        // Garante que o alerta comece desativado.
        if (overloadWarningObject != null)
        {
            overloadWarningObject.SetActive(false);
        }
    }

    void Update()
    {
        if (RaftStatusManager.Instance == null) return;

        // --- LÓGICA DA BARRA (não muda) ---
        float power = RaftStatusManager.Instance.CurrentPower;
        float weight = RaftStatusManager.Instance.CurrentWeight;
        powerWeightSlider.maxValue = Mathf.Max(power, weight, 1);
        powerWeightSlider.value = weight;
        sliderFillImage.color = RaftStatusManager.Instance.IsOverloaded ? overloadedColor : normalColor;
        if (statusText != null)
        {
            statusText.text = $"{power} / {weight}";
            statusText.color = RaftStatusManager.Instance.IsOverloaded ? overloadedColor : normalColor;
        }

        // --- NOVA LÓGICA DO ALERTA E COUNTDOWN ---
        bool isOverloaded = RaftStatusManager.Instance.IsOverloaded;

        // Ativa ou desativa o painel de aviso com base no estado de sobrecarga.
        if (overloadWarningObject != null && overloadWarningObject.activeSelf != isOverloaded)
        {
            overloadWarningObject.SetActive(isOverloaded);
        }

        // Se estiver sobrecarregado, atualiza o texto do countdown.
        if (isOverloaded && countdownText != null)
        {
            // O timer do manager conta para CIMA. Nós calculamos o tempo restante para contar para BAIXO.
            float timeToLose = 30f; // O tempo total de 30 segundos
            float elapsedTime = RaftStatusManager.Instance.OverloadTimer;
            float remainingTime = timeToLose - elapsedTime;

            // Garante que o texto não mostre um número negativo.
            if (remainingTime < 0) remainingTime = 0;

            // Atualiza o texto, usando Mathf.CeilToInt para arredondar para cima (mostra 30, 29, 28...).
            countdownText.text = Mathf.CeilToInt(remainingTime).ToString();
        }
        // ------------------------------------
    }
}