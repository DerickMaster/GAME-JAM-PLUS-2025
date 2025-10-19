using UnityEngine;

public class RadioController : MonoBehaviour
{
    void Start()
    {
        // Avisa ao WeatherManager que este rádio agora existe.
        if (WeatherManager.Instance != null)
        {
            WeatherManager.Instance.RegisterRadio();
        }
    }

    void OnDestroy()
    {
        // Avisa ao WeatherManager que este rádio foi destruído.
        // Importante checar se a instância ainda existe, pois o manager pode ser destruído antes.
        if (WeatherManager.Instance != null)
        {
            WeatherManager.Instance.UnregisterRadio();
        }
    }
}