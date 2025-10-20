using UnityEngine;
using System.Collections; // Essencial para usar Corrotinas

public class LightingManager : MonoBehaviour
{
    public static LightingManager Instance { get; private set; }

    [Header("Luzes da Cena")]
    [SerializeField] private Light mainDirectionalLight;
    [SerializeField] private Light stormAccentLight;

    [Header("Configurações de Intensidade")]
    [SerializeField] private float normalMainIntensity = 2f;
    [SerializeField] private float stormMainIntensity = 0.5f;
    [SerializeField] private float stormAccentIntensity = 2f;

    // --- NOVA VARIÁVEL ---
    [Header("Transição")]
    [Tooltip("Quanto tempo (em segundos) a transição da iluminação leva.")]
    [SerializeField] private float transitionDuration = 2.5f;

    // --- NOVAS VARIÁVEIS DE CONTROLE ---
    // Guardam a referência das corrotinas para podermos pará-las se necessário.
    private Coroutine mainLightCoroutine;
    private Coroutine accentLightCoroutine;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Start()
    {
        SetNormalLighting();
    }

    // --- FUNÇÕES DE COMANDO ATUALIZADAS ---
    public void SetStormLighting()
    {
        // Para qualquer corrotina de luz que estiver rodando
        StopAllLightCoroutines();

        // Inicia as novas transições
        mainLightCoroutine = StartCoroutine(TransitionLight(mainDirectionalLight, stormMainIntensity));

        if (stormAccentLight != null)
        {
            stormAccentLight.gameObject.SetActive(true); // Liga a luz de tempestade
            accentLightCoroutine = StartCoroutine(TransitionLight(stormAccentLight, stormAccentIntensity));
        }
    }

    public void SetNormalLighting()
    {
        StopAllLightCoroutines();
        mainLightCoroutine = StartCoroutine(TransitionLight(mainDirectionalLight, normalMainIntensity));
        accentLightCoroutine = StartCoroutine(TransitionLight(stormAccentLight, 0f)); // Transiciona a luz de tempestade para 0
    }

    private void StopAllLightCoroutines()
    {
        if (mainLightCoroutine != null) StopCoroutine(mainLightCoroutine);
        if (accentLightCoroutine != null) StopCoroutine(accentLightCoroutine);
    }

    // --- A CORROTINA MÁGICA ---
    // Esta função executa ao longo de vários frames.
    private IEnumerator TransitionLight(Light light, float targetIntensity)
    {
        if (light == null) yield break; // Sai se a luz não estiver configurada

        float startIntensity = light.intensity;
        float elapsedTime = 0f;

        // Loop que roda a cada frame, enquanto a transição não terminar.
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            // Calcula o progresso da transição (um valor de 0 a 1).
            float t = elapsedTime / transitionDuration;

            // Mathf.Lerp calcula o valor intermediário entre o início e o fim.
            light.intensity = Mathf.Lerp(startIntensity, targetIntensity, t);

            yield return null; // Pausa a função e continua no próximo frame.
        }

        // Garante que a intensidade final seja exatamente o valor alvo.
        light.intensity = targetIntensity;

        // Se o alvo era 0, desliga o objeto da luz para otimizar.
        if (targetIntensity <= 0)
        {
            light.gameObject.SetActive(false);
        }
    }
}