using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TriggerBossFight : MonoBehaviour {

    PlayerController player;
    EnemyBase bossMove;
    Spawner bossSpawner;
    LevelManager levelManager;
    public Transform boss;
    public CameraFollow camera;
    public Transform doorClose;
    public Transform doorOpen;
    public Transform spawner;
    public AudioClip bossMusic;
    public bool endLevel;

    public List<Transform> children;

    AudioManager audio;

    bool triggered = false;
    bool complete = false;

    int AliveChildren
    {
        get
        {
            return 0;
        }
    }

    void Start()
    {
        children = new List<Transform>();
        audio = GameObject.Find("LevelManager").GetComponent<AudioManager>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        bossMove = boss.GetComponent<EnemyBase>();
        bossSpawner = boss.GetComponent<Spawner>();
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
    }
	
    void Update()
    {
        if(bossSpawner != null)
            children = bossSpawner.children;

        if (bossMove == null && complete == false && AliveChildren == 0)
        {
            complete = true;
            StartCoroutine(EndScene());
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.transform == player.transform && triggered == false)
        {
            triggered = true;
            StartCoroutine(TriggerScene());
        }
    }

    IEnumerator TriggerScene()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        levelManager.playerLocked = true;
        audio.StartBoss(bossMusic);
        player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        camera.target = bossMove.transform;
        yield return new WaitForSeconds(1.5f);
        doorClose.gameObject.SetActive(true);
        player.transform.position = transform.position;
        bossMove.currentState = EnemyState.Normal;
        yield return new WaitForSeconds(1f);
        camera.target = player.transform;
        levelManager.playerLocked  = false;
        yield return new WaitForSeconds(0.2f);
        bossSpawner.Begin();
        if (spawner != null)
            spawner.GetComponent<Spawner>().Begin();
    }

    IEnumerator EndScene()
    {
        if(doorOpen != null)
            doorOpen.gameObject.SetActive(false);
        audio.EndBoss();
        if (spawner != null)
            spawner.GetComponent<Spawner>().Stop();
        yield return new WaitForSeconds(0.2f);
        if (endLevel)
        {
            GameObject.Find("LevelManager").GetComponent<EndLevel>().TriggerEnd();
        }
    }
}
