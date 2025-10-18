using UnityEngine;
using System.Collections;

public class Constructible : MonoBehaviour
{
    [Header("Referências Visuais")]
    [Tooltip("Arraste o objeto/pasta 'Preview' aqui.")]
    [SerializeField] private GameObject previewObject; // << NOVA REFERÊNCIA
    [Tooltip("Arraste o objeto/pasta 'Construction' aqui.")]
    [SerializeField] private GameObject constructionObject;
    [Tooltip("Arraste o objeto/pasta 'FinalModel' aqui.")]
    [SerializeField] private GameObject finalModelObject;

    [Header("Animação")]
    [SerializeField] private Animator constructionBoxAnimator;

    private BuildingData buildingData;
    private float constructionTimer;
    private bool isConstructing = false;

    private readonly int isConstructingHash = Animator.StringToHash("isConstructing");

    // Chamado pelo GridManager para iniciar o processo de construção.
    public void Initialize(BuildingData data)
    {
        buildingData = data;
        isConstructing = true;
        constructionTimer = 0f;

        // --- A NOVA LÓGICA DE TRANSIÇÃO ---
        // 1. Desliga o visual de preview.
        if (previewObject != null) previewObject.SetActive(false);

        // 2. Liga o visual da caixa de construção.
        if (constructionObject != null) constructionObject.SetActive(true);
        // ---------------------------------

        if (constructionBoxAnimator != null)
        {
            constructionBoxAnimator.SetBool(isConstructingHash, true);
        }
    }

    void Update()
    {
        if (!isConstructing || buildingData == null) return;
        constructionTimer += Time.deltaTime;
        if (constructionTimer >= buildingData.constructionTime)
        {
            StartCoroutine(CompleteConstructionSequence());
        }
    }

    public void SpeedUpConstruction()
    {
        if (!isConstructing) return;
        int metalCost = buildingData.metalCost / 2;
        int plasticCost = buildingData.plasticCost / 2;
        if (ResourceManager.Instance.HasEnoughResources(plasticCost, metalCost))
        {
            ResourceManager.Instance.SpendResources(plasticCost, metalCost);
            StartCoroutine(CompleteConstructionSequence());
        }
        else
        {
            Debug.Log("Recursos insuficientes para acelerar!");
        }
    }

    private IEnumerator CompleteConstructionSequence()
    {
        isConstructing = false;

        if (constructionBoxAnimator != null)
        {
            constructionBoxAnimator.SetBool(isConstructingHash, false);
        }

        yield return new WaitForSeconds(1f); // Espera 1 segundo para a animação.

        if (constructionObject != null) constructionObject.SetActive(false);
        if (finalModelObject != null) finalModelObject.SetActive(true);

        Destroy(this); // O trabalho do script acabou.
    }
}