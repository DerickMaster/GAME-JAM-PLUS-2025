using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BuildUIManager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject buildMenuPanel;
    [SerializeField] private List<Button> buildButtons;

    [Header("Player Components")]
    [SerializeField] private PlayerBuilder playerBuilder;
    [SerializeField] private PlayerController playerController;

    [Header("Settings")]
    [Tooltip("O tempo em segundos que o jogador precisa esperar antes de poder selecionar um item após abrir o menu.")]
    [SerializeField] private float selectionDelay = 0.2f; // Timer público para ajuste

    private int currentIndex = 0;
    private Vector2 lastNavigateInput = Vector2.zero;

    // --- NOVA VARIÁVEL DE CONTROLE ---
    // Guarda o momento exato em que o menu foi aberto.
    private float menuOpenTime;

    private void Start()
    {
        buildMenuPanel.SetActive(false);
    }

    public void OpenBuildMenu()
    {
        buildMenuPanel.SetActive(true);
        playerController.SetState(PlayerState.BuildMenu);
        SelectButton(0);

        // --- A MÁGICA DO TIMER COMEÇA AQUI ---
        // Registra o tempo atual quando o menu é aberto.
        menuOpenTime = Time.time;
    }

    public void CloseBuildMenu()
    {
        buildMenuPanel.SetActive(false);
        playerController.SetState(PlayerState.Gameplay);
    }

    public void Navigate(Vector2 input)
    {
        if (!buildMenuPanel.activeSelf) return;

        Vector2 currentNavigateInput = Vector2.zero;
        if (Mathf.Abs(input.x) > 0.5f) { currentNavigateInput.x = Mathf.Sign(input.x); }
        else if (Mathf.Abs(input.y) > 0.5f) { currentNavigateInput.y = Mathf.Sign(input.y); }

        if (currentNavigateInput != Vector2.zero && lastNavigateInput == Vector2.zero)
        {
            int newIndex = currentIndex;
            if (currentNavigateInput.y > 0) newIndex -= 4;
            else if (currentNavigateInput.y < 0) newIndex += 4;
            else if (currentNavigateInput.x > 0) newIndex += 1;
            else if (currentNavigateInput.x < 0) newIndex -= 1;

            if (newIndex >= 0 && newIndex < buildButtons.Count)
            {
                SelectButton(newIndex);
            }
        }
        lastNavigateInput = currentNavigateInput;
    }

    // A função de Submit agora checa o timer.
    public void SubmitSelection()
    {
        // --- CLÁUSULA DE GUARDA COM O TIMER ---
        // Se o tempo atual for menor que o tempo de abertura + o delay, ignora a ação.
        if (Time.time < menuOpenTime + selectionDelay) return;

        if (!buildMenuPanel.activeSelf) return;
        buildButtons[currentIndex].onClick.Invoke();
    }

    // A função chamada pelos botões também checa o timer.
    public void OnBuildItemSelected(BuildingData selectedBuilding)
    {
        // --- CLÁUSULA DE GUARDA COM O TIMER ---
        if (Time.time < menuOpenTime + selectionDelay) return;

        if (selectedBuilding != null)
        {
            CloseBuildMenu();
            playerBuilder.EnterBuildMode(selectedBuilding);
        }
    }

    private void SelectButton(int index)
    {
        if (currentIndex >= 0 && currentIndex < buildButtons.Count)
        {
            buildButtons[currentIndex].transform.localScale = Vector3.one;
        }
        currentIndex = index;
        if (currentIndex >= 0 && currentIndex < buildButtons.Count)
        {
            buildButtons[currentIndex].transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            buildButtons[currentIndex].Select();
        }
    }
}