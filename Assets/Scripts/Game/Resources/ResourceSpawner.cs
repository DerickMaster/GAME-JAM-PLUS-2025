using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    [Header("Configuração do Spawn")]
    [Tooltip("O prefab do objeto de recurso a ser gerado.")]
    [SerializeField] private GameObject resourcePrefab;
    [Tooltip("Os 4 pontos de onde os recursos irão nascer.")]
    [SerializeField] private Transform[] spawnPoints = new Transform[4];
    [Tooltip("O tempo em segundos entre cada novo recurso.")]
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
        if (resourcePrefab == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Prefab de recurso ou Spawn Points não foram definidos no ResourceSpawner!");
            return;
        }

        // 1. Escolhe uma das 4 lanes aleatoriamente.
        int laneIndex = Random.Range(0, spawnPoints.Length);
        Transform selectedSpawnPoint = spawnPoints[laneIndex];

        // 2. Cria o recurso na posição e rotação do spawn point.
        Instantiate(resourcePrefab, selectedSpawnPoint.position, selectedSpawnPoint.rotation);

        Debug.Log($"Gerando novo recurso na Lane {laneIndex + 1}.");
    }
}