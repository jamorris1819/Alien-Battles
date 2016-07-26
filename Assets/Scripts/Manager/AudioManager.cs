using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {

    public Transform prefab;

    public AudioClip[] hurt;
    public AudioClip[] shoot;

    public AudioClip explosion;
    public AudioClip laser;

    public AudioClip pickup;

    List<Transform> parts;

    Transform currentSong;
    Transform currentBossSong;

    void Awake()
    {
        parts = new List<Transform>();
    }

    void Update()
    {

    }

    public Transform Play(AudioClip clip, bool loop = false)
    {
        Transform t = (Transform)Instantiate(prefab, Vector3.zero, Quaternion.identity);
        t.GetComponent<AudioSource>().clip = clip;
        t.GetComponent<AudioSource>().loop = loop;
        t.GetComponent<AudioSource>().Play();
        parts.Add(t);
        return t;
    }

    public Transform PlayDistorted(AudioClip clip, float bend = 1f, bool loop = false)
    {
        Transform t = (Transform)Instantiate(prefab, Vector3.zero, Quaternion.identity);
        t.GetComponent<AudioSource>().clip = clip;
        t.GetComponent<AudioSource>().loop = loop;
        t.GetComponent<AudioSource>().pitch += (Random.value * 2 - 1) * bend;
        t.GetComponent<AudioSource>().Play();
        parts.Add(t);
        return t;
    }

    public void PlaySong(AudioClip song)
    {
        Transform t = Play(song, true);
        t.GetComponent<AudioSource>().volume = 0.6f;
        currentSong = t;
    }

    public void StartBoss(AudioClip song)
    {
        currentSong.GetComponent<AudioSource>().Pause();
        Transform t = Play(song, true);
        t.GetComponent<AudioSource>().volume = 0.8f;
        currentBossSong = t;
    }

    public void StopAll()
    {
        foreach (Transform part in parts)
        {
            if(part != null)
                part.GetComponent<AudioSource>().Stop();
        }
    }

    public void EndBoss()
    {
        Destroy(currentBossSong.gameObject);
        currentBossSong.GetComponent<AudioSource>().Stop();
        currentSong.GetComponent<AudioSource>().UnPause();
    }

    public void Damage()
    {
        Play(hurt[(int)Mathf.Floor(Random.value * hurt.Length)]);
    }

    public void Shoot()
    {
        Play(shoot[(int)Mathf.Floor(Random.value * shoot.Length)]);
    }

    public void Explosion()
    {
        PlayDistorted(explosion);
    }

    public void Laser()
    {
        PlayDistorted(laser, 0.25f);
    }

    public void Pickup()
    {
        PlayDistorted(pickup, 0.1f);
    }
}
