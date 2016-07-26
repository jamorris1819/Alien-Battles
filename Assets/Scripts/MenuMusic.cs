using UnityEngine;
using System.Collections;

public class MenuMusic : MonoBehaviour {

    public AudioClip clip;

    void Start()
    {
        AudioSource source = GetComponent<AudioSource>();
        source.clip = clip;
        source.loop = true;
        source.Play();
    }
}
