using UnityEngine;

public class IndividualResourceSpawner : MonoBehaviour
{
    [Header("Configuração do Spawner")]
    [Tooltip("A lista de possíveis prefabs de recursos que ESTE spawner pode criar.")]
    [SerializeField] private GameObject[] resourcePrefabs;

    // --- MUDANÇA AQUI: Trocamos uma variável por duas ---
    [Tooltip("O tempo MÍNIMO em segundos entre cada spawn.")]
    [SerializeField] private float minSpawnInterval = 8f; // Ex: mínimo de 8 segundos
    [Tooltip("O tempo MÁXIMO em segundos entre cada spawn.")]
    [SerializeField] private float maxSpawnInterval = 12f; // Ex: máximo de 12 segundos

    private float spawnTimer;

    // --- NOVO: Usamos Start() para configurar o primeiro timer ---
    void Start()
    {
        // Garante que o mínimo não seja maior que o máximo, evitando erros.
        if (minSpawnInterval > maxSpawnInterval)
        {
            Debug.LogWarning("O tempo mínimo de spawn é maior que o máximo. Ajustando para usar o valor mínimo.", this);
            maxSpawnInterval = minSpawnInterval;
        }

        // Define o primeiro timer para um valor aleatório, para que nem todos os spawners comecem ao mesmo tempo.
        ResetTimer();
    }

    void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0)
        {
            SpawnResource();
            ResetTimer();
        }
    }
    private void ResetTimer()
    {
        spawnTimer = Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    private void SpawnResource()
    {
        if (resourcePrefabs == null || resourcePrefabs.Length == 0)
        {
            Debug.LogError("A lista 'Resource Prefabs' está vazia neste spawner!", this);
            return;
        }

        int randomIndex = Random.Range(0, resourcePrefabs.Length);
        GameObject prefabToSpawn = resourcePrefabs[randomIndex];

        Instantiate(prefabToSpawn, transform.position, transform.rotation);
    }
}