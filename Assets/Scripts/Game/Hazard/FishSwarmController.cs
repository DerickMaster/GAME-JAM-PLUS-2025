using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class FishSwarmController : MonoBehaviour
{
    public static FishSwarmController Instance { get; private set; }

    private Animator animator;
    // Hashes para os triggers da animação
    private readonly int appearHash = Animator.StringToHash("OnAppear");
    private readonly int dieHash = Animator.StringToHash("OnDie");
    private readonly int attackHash = Animator.StringToHash("OnAttack");

    private readonly float threatDuration = 5f; // Tempo que eles ficam ameaçando

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        animator = GetComponent<Animator>();
        gameObject.SetActive(false); // Garante que comece desativado.
    }

    // O WeatherManager vai chamar esta função para iniciar o ataque.
    public void StartAttackSequence()
    {
        gameObject.SetActive(true);
        StartCoroutine(AttackSequence());
    }

    private IEnumerator AttackSequence()
    {
        // 1. Fase de Aparecer
        animator.SetTrigger(appearHash);
        yield return new WaitForSeconds(2f); // Tempo da animação de aparecer

        // 2. Fase de Ameaçar
        Debug.Log("[FishSwarm] Cardume ameaçando a balsa...");
        yield return new WaitForSeconds(threatDuration);

        // 3. Fase de Decisão
        // Pergunta ao ConstructionManager se existe uma metralhadora funcional.
        MachineGunController availableMachineGun = ConstructionManager.Instance.GetFirstAvailableMachineGun();

        if (availableMachineGun != null)
        {
            // SUCESSO: O jogador tem uma metralhadora.
            Debug.Log("[FishSwarm] Metralhadora detectada! O cardume será repelido.");
            availableMachineGun.Fire(); // Manda a metralhadora atirar.
            animator.SetTrigger(dieHash);
            yield return new WaitForSeconds(3f); // Espera a animação de morte/fuga.
            gameObject.SetActive(false);
        }
        else
        {
            // FALHA: O jogador não tem defesa.
            Debug.LogWarning("[FishSwarm] Nenhuma metralhadora encontrada! O cardume vai atacar.");
            animator.SetTrigger(attackHash);
            yield return new WaitForSeconds(1.5f);

            // Consequência: Vamos fazer eles quebrarem 1 slot, como uma versão mais fraca do caranguejo.
            GridManager.Instance.BreakRandomSlots(1);

            gameObject.SetActive(false);
        }
    }
}
