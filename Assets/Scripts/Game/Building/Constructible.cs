using UnityEngine;
using System.Collections;

public class Constructible : MonoBehaviour
{
    [Header("Referências Visuais")]
    [Tooltip("Arraste o objeto/pasta 'Preview' aqui.")]
    [SerializeField] private GameObject previewObject;
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

    public void Initialize(BuildingData data)
    {
        buildingData = data;
        isConstructing = true;
        constructionTimer = 0f;

        if (previewObject != null) previewObject.SetActive(false);
        if (constructionObject != null) constructionObject.SetActive(true);

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

    // Esta é a ÚNICA versão correta da função que deve existir no script.
    private IEnumerator CompleteConstructionSequence()
    {
        isConstructing = false;
        if (constructionBoxAnimator != null)
        {
            constructionBoxAnimator.SetBool(isConstructingHash, false);
        }

        yield return new WaitForSeconds(1f);

        if (constructionObject != null) constructionObject.SetActive(false);
        if (finalModelObject != null) finalModelObject.SetActive(true);

        // Adiciona o componente Destructible ao objeto raiz da construção.
        gameObject.AddComponent<Destructible>();

        Destroy(this); // O trabalho do Constructible acabou.
    }
}