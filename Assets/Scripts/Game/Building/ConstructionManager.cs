using UnityEngine;
using System.Collections.Generic;

public class ConstructionManager : MonoBehaviour
{
    public static ConstructionManager Instance { get; private set; }

    // Listas para rastrear construções específicas.
    private List<CannonController> activeCannons = new List<CannonController>();

    private List<MachineGunController> activeMachineGuns = new List<MachineGunController>();

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void RegisterCannon(CannonController cannon) { activeCannons.Add(cannon); }
    public void UnregisterCannon(CannonController cannon) { activeCannons.Remove(cannon); }

    public void RegisterMachineGun(MachineGunController machineGun) { activeMachineGuns.Add(machineGun); }
    public void UnregisterMachineGun(MachineGunController machineGun) { activeMachineGuns.Remove(machineGun); }

    private List<LightningRodController> activeLightningRods = new List<LightningRodController>();

    public void RegisterLightningRod(LightningRodController rod) { activeLightningRods.Add(rod); }
    public void UnregisterLightningRod(LightningRodController rod) { activeLightningRods.Remove(rod); }

    public CannonController GetFirstAvailableCannon()
    {
        // Retorna o primeiro canhão da lista que não esteja quebrado.
        foreach (var cannon in activeCannons)
        {
            if (cannon != null && !cannon.GetComponent<Destructible>().IsBroken)
            {
                return cannon;
            }
        }
        return null; // Retorna nulo se não houver canhões ou se todos estiverem quebrados.
    }

    public LightningRodController GetFirstAvailableLightningRod()
    {
        foreach (var rod in activeLightningRods)
        {
            // Retorna o primeiro para-raios que não esteja quebrado.
            if (rod != null && !rod.GetComponent<Destructible>().IsBroken)
            {
                return rod;
            }
        }
        return null;
    }

    public MachineGunController GetFirstAvailableMachineGun()
    {
        Debug.Log($"[ConstructionManager] Procurando por uma metralhadora. Total registradas: {activeMachineGuns.Count}");
        foreach (var machineGun in activeMachineGuns)
        {
            if (machineGun != null && !machineGun.GetComponent<Destructible>().IsBroken)
            {
                Debug.Log($"<color=green>[ConstructionManager] Metralhadora funcional encontrada!</color>");
                return machineGun;
            }
        }
        Debug.LogWarning("[ConstructionManager] Nenhuma metralhadora funcional foi encontrada.");
        return null;
    }
}