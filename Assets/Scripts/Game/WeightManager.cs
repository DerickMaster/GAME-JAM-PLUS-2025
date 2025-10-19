using UnityEngine;

public class WeightManager : MonoBehaviour
{
    // Singleton para acesso fácil de outros scripts
    public static WeightManager Instance { get; private set; }

    // Propriedade pública para que outros scripts possam LER o peso atual, mas não modificá-lo diretamente.
    public int CurrentWeight { get; private set; } = 0;

    private void Awake()
    {
        // Configuração do padrão Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Função chamada pelo GridManager quando uma construção é colocada.
    public void AddWeight(int amount)
    {
        CurrentWeight += amount;
        Debug.Log($"<color=orange>[WeightManager]</color> Peso adicionado: {amount}. Peso total agora: {CurrentWeight}");
        // No futuro, aqui você pode chamar um evento para atualizar a barra de peso na UI.
    }

    // Função chamada pelo Destructible quando uma construção é desmantelada.
    public void RemoveWeight(int amount)
    {
        CurrentWeight -= amount;
        // Garante que o peso não fique negativo.
        if (CurrentWeight < 0)
        {
            CurrentWeight = 0;
        }
        Debug.Log($"<color=cyan>[WeightManager]</color> Peso removido: {amount}. Peso total agora: {CurrentWeight}");
    }
}