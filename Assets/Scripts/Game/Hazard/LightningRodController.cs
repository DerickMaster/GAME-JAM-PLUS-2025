using UnityEngine;
using System.Collections;

public class LightningRodController : MonoBehaviour
{
    [Header("Efeito do Raio")]
    [Tooltip("Arraste o objeto filho que contém o modelo 3D do raio aqui.")]
    [SerializeField] private GameObject strikeEffectObject;
    [Tooltip("Quanto tempo o efeito do raio fica visível.")]
    [SerializeField] private float effectDuration = 1.5f;

    void Awake()
    {
        // Garante que o efeito comece desativado.
        if (strikeEffectObject != null)
        {
            strikeEffectObject.SetActive(false);
        }
    }

    void Start()
    {
        // Se registra no gerenciador de construções.
        if (ConstructionManager.Instance != null)
        {
            ConstructionManager.Instance.RegisterLightningRod(this);
        }
    }

    void OnDestroy()
    {
        if (ConstructionManager.Instance != null)
        {
            ConstructionManager.Instance.UnregisterLightningRod(this);
        }
    }

    // Chamado pelo WeatherManager quando este para-raios absorve um raio.
    public void AbsorbStrike()
    {
        Debug.Log($"<color=yellow>[LightningRod]</color> Para-raios absorveu o raio!");
        if (strikeEffectObject != null)
        {
            StartCoroutine(ShowStrikeEffect());
        }
        // Aqui você pode adicionar um som de raio sendo absorvido.
    }

    private IEnumerator ShowStrikeEffect()
    {
        strikeEffectObject.SetActive(true);
        yield return new WaitForSeconds(effectDuration);
        strikeEffectObject.SetActive(false);
    }
}