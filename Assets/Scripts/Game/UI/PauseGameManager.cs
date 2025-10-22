using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Referências da UI")]
    [Tooltip("Arraste o objeto do painel do menu de pause aqui.")]
    [SerializeField] private GameObject pauseMenuPanel;

    public static bool IsPaused { get; private set; } = false;

    void Start()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        IsPaused = false;
    }

    // --- ESTA FUNÇÃO PRECISA SER PÚBLICA ---
    /// <summary>
    /// Retoma o jogo. Chamada pelo botão "Retomar".
    /// </summary>
    public void Resume()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        IsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Pause()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        IsPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void TogglePause()
    {
        if (IsPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    // --- ESTA FUNÇÃO TAMBÉM PRECISA SER PÚBLICA ---
    /// <summary>
    /// Fecha a aplicação. Chamada pelo botão "Sair do Jogo".
    /// </summary>
    public void ExitGame()
    {
        Debug.Log("Saindo do jogo...");
        Time.timeScale = 1f;
        Application.Quit();
    }
}