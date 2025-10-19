using UnityEngine;

public class RadioController : MonoBehaviour
{
    void Start()
    {
        // Avisa ao WeatherManager que este r�dio agora existe.
        if (WeatherManager.Instance != null)
        {
            WeatherManager.Instance.RegisterRadio();
        }
    }

    void OnDestroy()
    {
        // Avisa ao WeatherManager que este r�dio foi destru�do.
        // Importante checar se a inst�ncia ainda existe, pois o manager pode ser destru�do antes.
        if (WeatherManager.Instance != null)
        {
            WeatherManager.Instance.UnregisterRadio();
        }
    }
}