using UnityEngine;
using UnityEngine.SceneManagement; // Necessário para gerenciar cenas

public class LoseScreenManager : MonoBehaviour
{
    [Header("Configuração de Cena")]
    [Tooltip("O nome exato da sua cena de menu principal.")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [Tooltip("O nome exato da sua cena de jogo principal.")]
    [SerializeField] private string gameSceneName = "GameScene";

    void Start()
    {
        // Garante que o cursor do mouse esteja visível e livre na tela de derrota.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // --- NOVAS FUNÇÕES PÚBLICAS PARA OS BOTÕES ---

    /// <summary>
    /// Carrega a cena do jogo principal para o jogador tentar novamente.
    /// </summary>
    public void RestartGame()
    {
        // Garante que o tempo do jogo volte ao normal.
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    /// <summary>
    /// Carrega a cena do menu principal.
    /// </summary>
    public void ReturnToMainMenu()
    {
        // Garante que o tempo do jogo volte ao normal.
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}