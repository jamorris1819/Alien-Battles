using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class WindowManager : MonoBehaviour {
    public GameObject panel;
    public Text message;
    public bool visible;
    public AudioClip bleep;

    Vector3 originalPos;

    LevelManager levelManager;

    void Start()
    {
        levelManager = GetComponent<LevelManager>();
        originalPos = panel.transform.position;
    }

    public void Close()
    {
        visible = false;
    }

    public void Create(string title, string message)
    {
        this.message.text = message;
        panel.transform.position = originalPos;
        panel.gameObject.SetActive(true);
        visible = true;

        //levelManager.playerLocked = true;
        GameObject.Find("LevelManager").GetComponent<AudioManager>().Play(bleep);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene(0);

        panel.SetActive(visible);

        /*if(visible)
            panel.transform.position = Vector3.Lerp(panel.transform.position, originalPos + new Vector3(panel.transform.position.x, 220), 0.3f);
        else
            panel.transform.position = Vector3.Lerp(panel.transform.position, originalPos, 0.3f);*/
    }
}
