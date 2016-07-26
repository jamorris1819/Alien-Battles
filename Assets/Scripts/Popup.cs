using UnityEngine;
using System.Collections;

public class Popup : MonoBehaviour {
    public string title;
    public string message;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.name == "Player")
        {
            GameObject.Find("LevelManager").GetComponent<WindowManager>().Create(title, message.Replace('\\', '\n'));
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.name == "Player")
        {
            GameObject.Find("LevelManager").GetComponent<WindowManager>().Close();
        }
    }
}
