using UnityEngine;
using System.Collections; // Essencial para usar Corrotinas

public class LightingManager : MonoBehaviour
{
    public static LightingManager Instance { get; private set; }

    [Header("Luzes da Cena")]
    [SerializeField] private Light mainDirectionalLight;
    [SerializeField] private Light stormAccentLight;

    [Header("Configura��es de Intensidade")]
    [SerializeField] private float normalMainIntensity = 2f;
    [SerializeField] private float stormMainIntensity = 0.5f;
    [SerializeField] private float stormAccentIntensity = 2f;

    // --- NOVA VARI�VEL ---
    [Header("Transi��o")]
    [Tooltip("Quanto tempo (em segundos) a transi��o da ilumina��o leva.")]
    [SerializeField] private float transitionDuration = 2.5f;

    // --- NOVAS VARI�VEIS DE CONTROLE ---
    // Guardam a refer�ncia das corrotinas para podermos par�-las se necess�rio.
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

    // --- FUN��ES DE COMANDO ATUALIZADAS ---
    public void SetStormLighting()
    {
        // Para qualquer corrotina de luz que estiver rodando
        StopAllLightCoroutines();

        // Inicia as novas transi��es
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

    // --- A CORROTINA M�GICA ---
    // Esta fun��o executa ao longo de v�rios frames.
    private IEnumerator TransitionLight(Light light, float targetIntensity)
    {
        if (light == null) yield break; // Sai se a luz n�o estiver configurada

        float startIntensity = light.intensity;
        float elapsedTime = 0f;

        // Loop que roda a cada frame, enquanto a transi��o n�o terminar.
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            // Calcula o progresso da transi��o (um valor de 0 a 1).
            float t = elapsedTime / transitionDuration;

            // Mathf.Lerp calcula o valor intermedi�rio entre o in�cio e o fim.
            light.intensity = Mathf.Lerp(startIntensity, targetIntensity, t);

            yield return null; // Pausa a fun��o e continua no pr�ximo frame.
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