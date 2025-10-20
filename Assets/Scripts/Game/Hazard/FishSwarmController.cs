using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class FishSwarmController : MonoBehaviour
{
    public static FishSwarmController Instance { get; private set; }

    private Animator animator;
    // Hashes para os triggers da anima��o
    private readonly int appearHash = Animator.StringToHash("OnAppear");
    private readonly int dieHash = Animator.StringToHash("OnDie");
    private readonly int attackHash = Animator.StringToHash("OnAttack");

    private readonly float threatDuration = 5f; // Tempo que eles ficam amea�ando

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        animator = GetComponent<Animator>();
        gameObject.SetActive(false); // Garante que comece desativado.
    }

    // O WeatherManager vai chamar esta fun��o para iniciar o ataque.
    public void StartAttackSequence()
    {
        gameObject.SetActive(true);
        StartCoroutine(AttackSequence());
    }

    private IEnumerator AttackSequence()
    {
        // 1. Fase de Aparecer
        animator.SetTrigger(appearHash);
        yield return new WaitForSeconds(2f); // Tempo da anima��o de aparecer

        // 2. Fase de Amea�ar
        Debug.Log("[FishSwarm] Cardume amea�ando a balsa...");
        yield return new WaitForSeconds(threatDuration);

        // 3. Fase de Decis�o
        // Pergunta ao ConstructionManager se existe uma metralhadora funcional.
        MachineGunController availableMachineGun = ConstructionManager.Instance.GetFirstAvailableMachineGun();

        if (availableMachineGun != null)
        {
            // SUCESSO: O jogador tem uma metralhadora.
            Debug.Log("[FishSwarm] Metralhadora detectada! O cardume ser� repelido.");
            availableMachineGun.Fire(); // Manda a metralhadora atirar.
            animator.SetTrigger(dieHash);
            yield return new WaitForSeconds(3f); // Espera a anima��o de morte/fuga.
            gameObject.SetActive(false);
        }
        else
        {
            // FALHA: O jogador n�o tem defesa.
            Debug.LogWarning("[FishSwarm] Nenhuma metralhadora encontrada! O cardume vai atacar.");
            animator.SetTrigger(attackHash);
            yield return new WaitForSeconds(1.5f);

            // Consequ�ncia: Vamos fazer eles quebrarem 1 slot, como uma vers�o mais fraca do caranguejo.
            GridManager.Instance.BreakRandomSlots(1);

            gameObject.SetActive(false);
        }
    }
}
