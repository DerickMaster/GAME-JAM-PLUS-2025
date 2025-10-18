using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    // Singleton instance
    public static ResourceManager Instance { get; private set; }

    // Use [SerializeField] to set starting values in the Inspector for testing
    [SerializeField] private int startingPlastic = 50;
    [SerializeField] private int startingMetal = 50;

    // Public properties to read the current amount, but not change it from outside
    public int CurrentPlastic { get; private set; }
    public int CurrentMetal { get; private set; }

    private void Awake()
    {
        // Singleton pattern setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        CurrentPlastic = startingPlastic;
        CurrentMetal = startingMetal;
    }

    public void AddResources(int plasticToAdd, int metalToAdd)
    {
        CurrentPlastic = Mathf.Min(CurrentPlastic + plasticToAdd, 999);
        CurrentMetal = Mathf.Min(CurrentMetal + metalToAdd, 999);
        Debug.Log($"Added resources. Current state: {CurrentPlastic} Plastic, {CurrentMetal} Metal");
    }

    public bool HasEnoughResources(int plasticCost, int metalCost)
    {
        return CurrentPlastic >= plasticCost && CurrentMetal >= metalCost;
    }

    public void SpendResources(int plasticCost, int metalCost)
    {
        if (HasEnoughResources(plasticCost, metalCost))
        {
            CurrentPlastic -= plasticCost;
            CurrentMetal -= metalCost;
            Debug.Log($"Spent resources. Remaining: {CurrentPlastic} Plastic, {CurrentMetal} Metal");
        }
        else
        {
            Debug.LogWarning("Not enough resources to spend!");
        }
    }
}