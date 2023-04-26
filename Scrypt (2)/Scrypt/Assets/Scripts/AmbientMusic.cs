using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientMusic : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip ambientClip;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = ambientClip;
        audioSource.loop = true;
        audioSource.Play();
    }
}
