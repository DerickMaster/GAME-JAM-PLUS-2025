using UnityEngine;

// Garante que este script só pode ser adicionado a objetos que tenham um Animator.
[RequireComponent(typeof(Animator))]
public class RandomizeAnimationStart : MonoBehaviour
{
    [Header("Configuração da Animação")]
    [Tooltip("O nome exato do estado de animação que você quer randomizar (ex: 'Grid_Idle', 'Water_Flow').")]
    [SerializeField] private string stateName;

    void Start()
    {
        Animator animator = GetComponent<Animator>();

        // Gera um número aleatório entre 0.0 (início da animação) e 1.0 (fim da animação).
        float randomStartTime = Random.Range(0f, 1f);

        // Força o Animator a começar a tocar o estado especificado, na primeira camada (0),
        // a partir do ponto aleatório que calculamos.
        animator.Play(stateName, 0, randomStartTime);
    }
}