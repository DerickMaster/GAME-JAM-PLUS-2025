using UnityEngine;
using TMPro; // Essencial para interagir com componentes TextMeshPro

public class ResourceUIManager : MonoBehaviour
{
    [Header("Refer�ncias do Texto")]
    [Tooltip("Arraste o objeto de texto que mostra a contagem de Metal aqui.")]
    [SerializeField] private TMP_Text metalCountText;

    [Tooltip("Arraste o objeto de texto que mostra a contagem de Pl�stico aqui.")]
    [SerializeField] private TMP_Text plasticCountText;

    // Guardamos os valores anteriores para otimizar e s� atualizar o texto quando necess�rio.
    private int lastMetalCount = -1;
    private int lastPlasticCount = -1;

    void Update()
    {
        // Se o ResourceManager ainda n�o foi inicializado, n�o faz nada.
        if (ResourceManager.Instance == null) return;

        // Pega os valores atuais do manager.
        int currentMetal = ResourceManager.Instance.CurrentMetal;
        int currentPlastic = ResourceManager.Instance.CurrentPlastic;

        // --- L�GICA DE ATUALIZA��O ---

        // S� atualiza o texto do Metal se o valor mudou.
        if (currentMetal != lastMetalCount)
        {
            if (metalCountText != null)
            {
                metalCountText.text = currentMetal.ToString();
            }
            lastMetalCount = currentMetal;
        }

        // S� atualiza o texto do Pl�stico se o valor mudou.
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