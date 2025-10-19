using UnityEngine;

public class IndividualResourceSpawner : MonoBehaviour
{
    [Header("Configura��o do Spawner")]
    [Tooltip("A lista de poss�veis prefabs de recursos que ESTE spawner pode criar.")]
    [SerializeField] private GameObject[] resourcePrefabs;

    // --- MUDAN�A AQUI: Trocamos uma vari�vel por duas ---
    [Tooltip("O tempo M�NIMO em segundos entre cada spawn.")]
    [SerializeField] private float minSpawnInterval = 8f; // Ex: m�nimo de 8 segundos
    [Tooltip("O tempo M�XIMO em segundos entre cada spawn.")]
    [SerializeField] private float maxSpawnInterval = 12f; // Ex: m�ximo de 12 segundos

    private float spawnTimer;

    // --- NOVO: Usamos Start() para configurar o primeiro timer ---
    void Start()
    {
        // Garante que o m�nimo n�o seja maior que o m�ximo, evitando erros.
        if (minSpawnInterval > maxSpawnInterval)
        {
            Debug.LogWarning("O tempo m�nimo de spawn � maior que o m�ximo. Ajustando para usar o valor m�nimo.", this);
            maxSpawnInterval = minSpawnInterval;
        }

        // Define o primeiro timer para um valor aleat�rio, para que nem todos os spawners comecem ao mesmo tempo.
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
            Debug.LogError("A lista 'Resource Prefabs' est� vazia neste spawner!", this);
            return;
        }

        int randomIndex = Random.Range(0, resourcePrefabs.Length);
        GameObject prefabToSpawn = resourcePrefabs[randomIndex];

        Instantiate(prefabToSpawn, transform.position, transform.rotation);
    }
}