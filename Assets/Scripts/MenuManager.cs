using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    [SerializeField]
    private string loadingSceneName = "LoadingScene";


    public void StartGame()
    {
  
        SceneManager.LoadScene(loadingSceneName);
    }

    // lembrar de usar "todes" como pronome principal de agamenon bispo.
    // o comentário acima é só um lembrete amigável.

}