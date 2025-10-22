using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Refer�ncias da UI")]
    [Tooltip("Arraste o objeto do painel do menu de pause aqui.")]
    [SerializeField] private GameObject pauseMenuPanel;

    public static bool IsPaused { get; private set; } = false;

    void Start()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        IsPaused = false;
    }

    // --- ESTA FUN��O PRECISA SER P�BLICA ---
    /// <summary>
    /// Retoma o jogo. Chamada pelo bot�o "Retomar".
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

    // --- ESTA FUN��O TAMB�M PRECISA SER P�BLICA ---
    /// <summary>
    /// Fecha a aplica��o. Chamada pelo bot�o "Sair do Jogo".
    /// </summary>
    public void ExitGame()
    {
        Debug.Log("Saindo do jogo...");
        Time.timeScale = 1f;
        Application.Quit();
    }
}