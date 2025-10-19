using UnityEngine;

public class IndividualResourceSpawner : MonoBehaviour
{
    [Header("Configura��o do Spawner")]
    [Tooltip("A lista de poss�veis prefabs de recursos que ESTE spawner pode criar.")]
    [SerializeField] private GameObject[] resourcePrefabs; // << MUDAN�A PRINCIPAL
    [Tooltip("O tempo em segundos entre cada spawn.")]
    [SerializeField] private float spawnInterval = 10f;

    private float spawnTimer;

    void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0)
        {
            SpawnResource();
            spawnTimer = spawnInterval;
        }
    }

    private void SpawnResource()
    {
        // Checagem de seguran�a para garantir que a lista n�o est� vazia.
        if (resourcePrefabs == null || resourcePrefabs.Length == 0)
        {
            Debug.LogError("A lista 'Resource Prefabs' est� vazia neste spawner!", this);
            return;
        }

        // 1. Escolhe um prefab aleat�rio da lista.
        int randomIndex = Random.Range(0, resourcePrefabs.Length);
        GameObject prefabToSpawn = resourcePrefabs[randomIndex];

        // 2. Cria o recurso na posi��o e rota��o deste spawner.
        Instantiate(prefabToSpawn, transform.position, transform.rotation);
    }
}