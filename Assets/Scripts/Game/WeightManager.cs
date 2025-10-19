using UnityEngine;

public class WeightManager : MonoBehaviour
{
    // Singleton para acesso f�cil de outros scripts
    public static WeightManager Instance { get; private set; }

    // Propriedade p�blica para que outros scripts possam LER o peso atual, mas n�o modific�-lo diretamente.
    public int CurrentWeight { get; private set; } = 0;

    private void Awake()
    {
        // Configura��o do padr�o Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Fun��o chamada pelo GridManager quando uma constru��o � colocada.
    public void AddWeight(int amount)
    {
        CurrentWeight += amount;
        Debug.Log($"<color=orange>[WeightManager]</color> Peso adicionado: {amount}. Peso total agora: {CurrentWeight}");
        // No futuro, aqui voc� pode chamar um evento para atualizar a barra de peso na UI.
    }

    // Fun��o chamada pelo Destructible quando uma constru��o � desmantelada.
    public void RemoveWeight(int amount)
    {
        CurrentWeight -= amount;
        // Garante que o peso n�o fique negativo.
        if (CurrentWeight < 0)
        {
            CurrentWeight = 0;
        }
        Debug.Log($"<color=cyan>[WeightManager]</color> Peso removido: {amount}. Peso total agora: {CurrentWeight}");
    }
}