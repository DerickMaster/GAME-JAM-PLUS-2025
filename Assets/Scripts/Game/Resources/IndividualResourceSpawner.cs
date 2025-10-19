using UnityEngine;

public class IndividualResourceSpawner : MonoBehaviour
{
    [Header("Configuração do Spawner")]
    [Tooltip("A lista de possíveis prefabs de recursos que ESTE spawner pode criar.")]
    [SerializeField] private GameObject[] resourcePrefabs; // << MUDANÇA PRINCIPAL
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
        // Checagem de segurança para garantir que a lista não está vazia.
        if (resourcePrefabs == null || resourcePrefabs.Length == 0)
        {
            Debug.LogError("A lista 'Resource Prefabs' está vazia neste spawner!", this);
            return;
        }

        // 1. Escolhe um prefab aleatório da lista.
        int randomIndex = Random.Range(0, resourcePrefabs.Length);
        GameObject prefabToSpawn = resourcePrefabs[randomIndex];

        // 2. Cria o recurso na posição e rotação deste spawner.
        Instantiate(prefabToSpawn, transform.position, transform.rotation);
    }
}