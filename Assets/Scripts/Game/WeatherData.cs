using UnityEngine;

// O Enum com os nomes em ingl�s para usarmos no c�digo.
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
    public string displayName_PT; // O nome em portugu�s para a UI
    public string hintText_PT;    // A dica em portugu�s para a UI
    public Sprite icon;           // (Opcional) Um �cone para a UI do forecast
}