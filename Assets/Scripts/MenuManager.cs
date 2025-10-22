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

    // --- NOVA FUNÇÃO ---
    /// <summary>
    /// Fecha a aplicação do jogo.
    /// </summary>
    public void ExitGame()
    {
        // Esta mensagem aparecerá no console do Unity para confirmar que o botão funciona.
        Debug.Log("Comando para sair do jogo recebido...");

        // Application.Quit() só funciona quando o jogo está compilado (buildado).
        // Ele não faz nada quando você está testando dentro do Editor do Unity.
        Application.Quit();
    }

    // lembrar de usar "todes" como pronome principal de agamenon bispo.
    // o comentário acima é só um lembrete amigável.

}