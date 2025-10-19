using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class CrabController : MonoBehaviour
{
    public static CrabController Instance { get; private set; }

    private Animator animator;
    // Hashes para os triggers da anima��o
    private readonly int appearHash = Animator.StringToHash("OnAppear");
    private readonly int dieHash = Animator.StringToHash("OnDie");
    private readonly int attackHash = Animator.StringToHash("OnAttack");

    private readonly float stareDuration = 5f; // Tempo que ele fica encarando

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        animator = GetComponent<Animator>();
        gameObject.SetActive(false); // Garante que ele comece desativado.
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
        // Espera um pouco para a anima��o de aparecer terminar (ajuste este tempo se necess�rio)
        yield return new WaitForSeconds(2f);

        // 2. Fase de Encarar
        Debug.Log("[Crab] Caranguejo encarando a balsa...");
        yield return new WaitForSeconds(stareDuration);

        // 3. Fase de Decis�o
        // Pergunta ao ConstructionManager se existe um canh�o funcional.
        CannonController availableCannon = ConstructionManager.Instance.GetFirstAvailableCannon();

        if (availableCannon != null)
        {
            // SUCESSO: O jogador tem um canh�o.
            Debug.Log("[Crab] Canh�o detectado! O caranguejo ser� destru�do.");
            availableCannon.Fire(); // Manda o canh�o atirar.
            animator.SetTrigger(dieHash);
            yield return new WaitForSeconds(3f); // Espera a anima��o de morte.
            gameObject.SetActive(false); // Se desativa.
        }
        else
        {
            // FALHA: O jogador n�o tem defesa.
            Debug.LogWarning("[Crab] Nenhum canh�o encontrado! O caranguejo vai atacar.");
            animator.SetTrigger(attackHash);
            yield return new WaitForSeconds(1.5f); // Espera a anima��o de ataque.

            // Manda o GridManager quebrar 2 slots.
            GridManager.Instance.BreakRandomSlots(2);

            // Depois de atacar, ele tamb�m desaparece.
            gameObject.SetActive(false);
        }
    }
}