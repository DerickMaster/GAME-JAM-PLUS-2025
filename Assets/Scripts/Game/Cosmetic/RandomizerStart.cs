using UnityEngine;

// Garante que este script s� pode ser adicionado a objetos que tenham um Animator.
[RequireComponent(typeof(Animator))]
public class RandomizeAnimationStart : MonoBehaviour
{
    [Header("Configura��o da Anima��o")]
    [Tooltip("O nome exato do estado de anima��o que voc� quer randomizar (ex: 'Grid_Idle', 'Water_Flow').")]
    [SerializeField] private string stateName;

    void Start()
    {
        Animator animator = GetComponent<Animator>();

        // Gera um n�mero aleat�rio entre 0.0 (in�cio da anima��o) e 1.0 (fim da anima��o).
        float randomStartTime = Random.Range(0f, 1f);

        // For�a o Animator a come�ar a tocar o estado especificado, na primeira camada (0),
        // a partir do ponto aleat�rio que calculamos.
        animator.Play(stateName, 0, randomStartTime);
    }
}