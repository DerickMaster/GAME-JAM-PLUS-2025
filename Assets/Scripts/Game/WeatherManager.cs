using UnityEngine;

// An enum makes it easy and safe to select weather types in the Inspector
public enum WeatherType
{
    None, // A default calm state
    Storm,
    SharkSeason,
    TheCrab,
    TheOneIsComing
}

public class WeatherManager : MonoBehaviour
{
    // Singleton instance
    public static WeatherManager Instance { get; private set; }

    // This will create an editable list of 6 weather types in the Unity Inspector
    [Tooltip("The forecast for the next 6 days.")]
    public WeatherType[] forecast = new WeatherType[6];

    public int CurrentDay { get; private set; } = 0;

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

    public WeatherType GetCurrentWeather()
    {
        if (forecast.Length > 0 && CurrentDay < forecast.Length)
        {
            return forecast[CurrentDay];
        }
        return WeatherType.None;
    }

    // We can call this function when a day passes in the game
    public void AdvanceToNextDay()
    {
        CurrentDay++;
        Debug.Log($"A new day has begun! Today's forecast: {GetCurrentWeather()}");
    }
}