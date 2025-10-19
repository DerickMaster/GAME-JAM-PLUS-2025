using UnityEngine;
using TMPro; // Essencial para interagir com componentes TextMeshPro

public class ResourceUIManager : MonoBehaviour
{
    [Header("Referências do Texto")]
    [Tooltip("Arraste o objeto de texto que mostra a contagem de Metal aqui.")]
    [SerializeField] private TMP_Text metalCountText;

    [Tooltip("Arraste o objeto de texto que mostra a contagem de Plástico aqui.")]
    [SerializeField] private TMP_Text plasticCountText;

    // Guardamos os valores anteriores para otimizar e só atualizar o texto quando necessário.
    private int lastMetalCount = -1;
    private int lastPlasticCount = -1;

    void Update()
    {
        // Se o ResourceManager ainda não foi inicializado, não faz nada.
        if (ResourceManager.Instance == null) return;

        // Pega os valores atuais do manager.
        int currentMetal = ResourceManager.Instance.CurrentMetal;
        int currentPlastic = ResourceManager.Instance.CurrentPlastic;

        // --- LÓGICA DE ATUALIZAÇÃO ---

        // Só atualiza o texto do Metal se o valor mudou.
        if (currentMetal != lastMetalCount)
        {
            if (metalCountText != null)
            {
                metalCountText.text = currentMetal.ToString();
            }
            lastMetalCount = currentMetal;
        }

        // Só atualiza o texto do Plástico se o valor mudou.
        if (currentPlastic != lastPlasticCount)
        {
            if (plasticCountText != null)
            {
                plasticCountText.text = currentPlastic.ToString();
            }
            lastPlasticCount = currentPlastic;
        }
    }
}