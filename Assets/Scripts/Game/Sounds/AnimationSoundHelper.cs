using UnityEngine;
using FMODUnity; // Importante! Precisamos disso para o FMOD.

public class FMOD_AnimationSoundHelper : MonoBehaviour
{
    [Header("One-Shot Sounds")]
    [Tooltip("Lista de sons de disparo único. A animação chamará um som pelo seu índice (posição) nesta lista.")]
    [SerializeField] private EventReference[] oneShotSounds;

    [Header("Looping Sound")]
    [Tooltip("Arraste aqui o GameObject filho que tem o FMOD Emitter para o som em loop.")]
    [SerializeField] private GameObject loopingSoundObject;

    /// <summary>
    /// Toca um som de disparo único da lista 'oneShotSounds'.
    /// Esta função é chamada por um Animation Event.
    /// </summary>
    /// <param name="index">O índice do som na lista para tocar.</param>
    public void PlayObjectSound(int index)
    {
        // Checagem de segurança para evitar erros se o índice for inválido.
        if (index < 0 || index >= oneShotSounds.Length)
        {
            Debug.LogError($"Índice de som inválido: {index}. O objeto {gameObject.name} só tem {oneShotSounds.Length} sons na lista.", this);
            return;
        }

        // Toca o som FMOD na posição deste objeto.
        RuntimeManager.PlayOneShot(oneShotSounds[index], transform.position);
    }

    /// <summary>
    /// Ativa o GameObject que contém o som em loop.
    /// Esta função é chamada por um Animation Event.
    /// </summary>
    public void PlayLoopSound()
    {
        if (loopingSoundObject != null)
        {
            loopingSoundObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("LoopingSoundObject não foi definido no Inspector.", this);
        }
    }

    /// <summary>
    /// Desativa o GameObject que contém o som em loop.
    /// Esta função é chamada por um Animation Event para parar o som.
    /// </summary>
    public void StopLoopSound()
    {
        if (loopingSoundObject != null)
        {
            loopingSoundObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("LoopingSoundObject não foi definido no Inspector.", this);
        }
    }
}