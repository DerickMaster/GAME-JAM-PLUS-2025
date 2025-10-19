using UnityEngine;

// O Enum com os nomes em inglês para usarmos no código.
public enum WeatherType
{
    Calm,
    Cutters,
    Heavy,
    Storm,
    Predators,
    Sinkers,
    Earthquake,
    Everything
}

[CreateAssetMenu(fileName = "New Weather Data", menuName = "Wrecked Tide/Weather Data")]
public class WeatherData : ScriptableObject
{
    public WeatherType type;
    public string displayName_PT; // O nome em português para a UI
    public string hintText_PT;    // A dica em português para a UI
    public Sprite icon;           // (Opcional) Um ícone para a UI do forecast
}