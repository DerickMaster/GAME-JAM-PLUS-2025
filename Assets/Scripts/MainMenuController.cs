using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management

public class MainMenuManager : MonoBehaviour
{
    // This function will be called by the "New Game" button.
    public void StartGame()
    {
        // Make sure "GameScene" is the exact name of your main gameplay scene.
        // If your scene has a different name, change it here.
        SceneManager.LoadScene("GameScene");
    }

    // This function will be called by the "Options" button.
    public void OpenOptions()
    {
        // For now, we'll just print a message to the console to test it.
        Debug.Log("Options button clicked!");
        // Later, you could activate an options panel here.
    }

    // This function will be called by the "Quit" button.
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        // This command closes the application (only works in a built game, not the editor).
        Application.Quit();
    }
}