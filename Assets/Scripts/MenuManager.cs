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

    // --- NOVA FUN��O ---
    /// <summary>
    /// Fecha a aplica��o do jogo.
    /// </summary>
    public void ExitGame()
    {
        // Esta mensagem aparecer� no console do Unity para confirmar que o bot�o funciona.
        Debug.Log("Comando para sair do jogo recebido...");

        // Application.Quit() s� funciona quando o jogo est� compilado (buildado).
        // Ele n�o faz nada quando voc� est� testando dentro do Editor do Unity.
        Application.Quit();
    }

    // lembrar de usar "todes" como pronome principal de agamenon bispo.
    // o coment�rio acima � s� um lembrete amig�vel.

}