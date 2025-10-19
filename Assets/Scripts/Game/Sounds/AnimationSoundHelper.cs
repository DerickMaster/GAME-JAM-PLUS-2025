using UnityEngine;
using FMODUnity; // Importante! Precisamos disso para o FMOD.

public class FMOD_AnimationSoundHelper : MonoBehaviour
{
    [Header("One-Shot Sounds")]
    [Tooltip("Lista de sons de disparo �nico. A anima��o chamar� um som pelo seu �ndice (posi��o) nesta lista.")]
    [SerializeField] private EventReference[] oneShotSounds;

    [Header("Looping Sound")]
    [Tooltip("Arraste aqui o GameObject filho que tem o FMOD Emitter para o som em loop.")]
    [SerializeField] private GameObject loopingSoundObject;

    /// <summary>
    /// Toca um som de disparo �nico da lista 'oneShotSounds'.
    /// Esta fun��o � chamada por um Animation Event.
    /// </summary>
    /// <param name="index">O �ndice do som na lista para tocar.</param>
    public void PlayObjectSound(int index)
    {
        // Checagem de seguran�a para evitar erros se o �ndice for inv�lido.
        if (index < 0 || index >= oneShotSounds.Length)
        {
            Debug.LogError($"�ndice de som inv�lido: {index}. O objeto {gameObject.name} s� tem {oneShotSounds.Length} sons na lista.", this);
            return;
        }

        // Toca o som FMOD na posi��o deste objeto.
        RuntimeManager.PlayOneShot(oneShotSounds[index], transform.position);
    }

    /// <summary>
    /// Ativa o GameObject que cont�m o som em loop.
    /// Esta fun��o � chamada por um Animation Event.
    /// </summary>
    public void PlayLoopSound()
    {
        if (loopingSoundObject != null)
        {
            loopingSoundObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("LoopingSoundObject n�o foi definido no Inspector.", this);
        }
    }

    /// <summary>
    /// Desativa o GameObject que cont�m o som em loop.
    /// Esta fun��o � chamada por um Animation Event para parar o som.
    /// </summary>
    public void StopLoopSound()
    {
        if (loopingSoundObject != null)
        {
            loopingSoundObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("LoopingSoundObject n�o foi definido no Inspector.", this);
        }
    }
}