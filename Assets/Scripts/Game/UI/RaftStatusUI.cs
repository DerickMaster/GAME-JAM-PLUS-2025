using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RaftStatusUI : MonoBehaviour
{
    [Header("Referências da UI")]
    [SerializeField] private Slider powerWeightSlider;
    [SerializeField] private Image sliderFillImage;
    [SerializeField] private TMP_Text statusText;

    [Header("Cores de Feedback")]
    [SerializeField] private Color normalColor = new Color(0, 0.8f, 1f);
    [SerializeField] private Color overloadedColor = new Color(1f, 0.2f, 0f);

    void Update()
    {
        if (RaftStatusManager.Instance == null) return;

        float power = RaftStatusManager.Instance.CurrentPower;
        float weight = RaftStatusManager.Instance.CurrentWeight;

        // O valor máximo da barra é o maior entre a potência e o peso.
        // Isso garante que a barra sempre tenha a escala correta.
        powerWeightSlider.maxValue = Mathf.Max(power, weight, 1); // Adicionado '1' para evitar um max de 0.

        // O valor atual (o preenchimento) é sempre o peso.
        powerWeightSlider.value = weight;

        // A lógica de cor.
        sliderFillImage.color = RaftStatusManager.Instance.IsOverloaded ? overloadedColor : normalColor;

        if (statusText != null)
        {
            statusText.text = $"{power} / {weight}";
            statusText.color = RaftStatusManager.Instance.IsOverloaded ? overloadedColor : normalColor;
        }
    }
}