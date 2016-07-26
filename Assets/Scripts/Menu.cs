using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

    public void LoadLevel(int i)
    {
        SceneManager.LoadScene(i);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Website(string s)
    {
        Application.OpenURL(s);
    }
}
