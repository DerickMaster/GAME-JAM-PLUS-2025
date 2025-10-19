using UnityEngine;
using System.Collections.Generic;

public class ConstructionManager : MonoBehaviour
{
    public static ConstructionManager Instance { get; private set; }

    // Listas para rastrear constru��es espec�ficas.
    private List<CannonController> activeCannons = new List<CannonController>();

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void RegisterCannon(CannonController cannon) { activeCannons.Add(cannon); }
    public void UnregisterCannon(CannonController cannon) { activeCannons.Remove(cannon); }

    public CannonController GetFirstAvailableCannon()
    {
        // Retorna o primeiro canh�o da lista que n�o esteja quebrado.
        foreach (var cannon in activeCannons)
        {
            if (cannon != null && !cannon.GetComponent<Destructible>().IsBroken)
            {
                return cannon;
            }
        }
        return null; // Retorna nulo se n�o houver canh�es ou se todos estiverem quebrados.
    }
}