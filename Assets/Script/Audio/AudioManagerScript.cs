using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerScript : MonoBehaviour
{
    public List<AudioClip> audioClips; // Isi di Inspector
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // Fungsi buat play audio berdasarkan index
    public void PlayAudioByIndex(int index)
    {
        if (index >= 0 && index < audioClips.Count)
        {
            audioSource.clip = audioClips[index];
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("Index audio tidak valid: " + index);
        }
    }
}
