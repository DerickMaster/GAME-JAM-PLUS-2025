using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AsyncLoader : MonoBehaviour
{
    [SerializeField]
    private string gameSceneName = "NOME_DA_SUA_CENA_DE_TESTE_AQUI";

    // A função Start é chamada automaticamente quando esta cena (LoadingScene) abre
    void Start()
    {
        // Inicia a rotina de carregamento
        StartCoroutine(LoadGameSceneAsync());
    }

    IEnumerator LoadGameSceneAsync()
    {
        // Espera um frame para garantir que o texto "Loading..." seja renderizado
        yield return null;

        // Inicia o carregamento da cena em background
        // Ele vai carregar qualquer cena que esteja escrita na variável 'gameSceneName'
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(gameSceneName);

        // Espera o carregamento terminar
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}