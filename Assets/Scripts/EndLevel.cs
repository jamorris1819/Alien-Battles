using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class EndLevel : MonoBehaviour {

    enum Modifiers { AllEnemiesKilled, FullHealth, WeaponBonus }

    Dictionary<Modifiers, float> multipliers;
    Dictionary<char, string> rankComments;

    public Transform panel;
    public Text score;
    public Text modifiers;
    public Text totalScore;
    public Text rank;
    public Text comment;

    public Image fadePanel;

    public AudioClip successSound;
    public AudioClip blip;

    public int maxPoints;

    PlayerController player;
    AudioManager audio;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        multipliers = new Dictionary<Modifiers, float>();
        multipliers.Add(Modifiers.AllEnemiesKilled, 1.5f);
        multipliers.Add(Modifiers.FullHealth, 1.2f);
        multipliers.Add(Modifiers.WeaponBonus, 1.1f);

        score.gameObject.SetActive(false);
        modifiers.gameObject.SetActive(false);
        totalScore.gameObject.SetActive(false);
        rank.gameObject.SetActive(false);
        comment.gameObject.SetActive(false);
        audio = GetComponent<AudioManager>();

        rankComments = new Dictionary<char, string>();
        rankComments.Add('A', "Amazing work!");
        rankComments.Add('B', "Good job.");
        rankComments.Add('C', "Not bad.");
        rankComments.Add('D', "You need to practice...");
        rankComments.Add('E', "Oh dear...");
        rankComments.Add('F', "You suck!");
    }

    public void TriggerEnd()
    {
        StartCoroutine(Fade());
        StartCoroutine(FinishFunction());
    }

    IEnumerator Fade()
    {
        for (int i = 0; i < 256; i++)
        {
            fadePanel.color = new Color(0, 0, 0, i);
            yield return new WaitForSeconds(1 / 255);
        }
    }

    IEnumerator FinishFunction()
    {
        player.vulnerable = false;
        player.locked = true;
        audio.StopAll();
        audio.Play(successSound);
        List<Modifiers> mods = new List<Modifiers>();
        panel.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.75f);
        audio.Play(blip);
        score.gameObject.SetActive(true);
        score.text = "Score: " + player.score;
        float endScore = player.score;

        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
            mods.Add(Modifiers.AllEnemiesKilled);
        if (player.health == 35)
            mods.Add(Modifiers.FullHealth);
        //if (player.currentWeapon != player.pistol)
        //    mods.Add(Modifiers.WeaponBonus);
        yield return new WaitForSeconds(0.75f);
        audio.Play(blip);
        modifiers.gameObject.SetActive(true);
        modifiers.text = "Modifiers:";
        foreach (Modifiers mod in mods)
        {
            yield return new WaitForSeconds(0.75f);
            audio.Play(blip);
            modifiers.text += "\n" + SplitCamelCase(mod.ToString()) + " - " + multipliers[mod] + "x";
            endScore *= multipliers[mod];
        }
        if (mods.Count == 0)
        {
            yield return new WaitForSeconds(0.75f);
            audio.Play(blip);
            modifiers.text += "\nNone";
        }
        yield return new WaitForSeconds(0.75f);
        audio.Play(blip);
        totalScore.gameObject.SetActive(true);
        totalScore.text = "Total score: " + Mathf.Ceil(endScore);
        yield return new WaitForSeconds(0.75f);
        audio.Play(blip);
        rank.gameObject.SetActive(true);
        char rankChar;
        if (endScore > maxPoints * 0.9f)
            rankChar = 'A';
        else if (endScore > maxPoints * 0.8f)
            rankChar = 'B';
        else if (endScore > maxPoints * 0.6f)
            rankChar = 'C';
        else if (endScore > maxPoints * 0.5f)
            rankChar = 'D';
        else if (endScore > maxPoints * 0.4f)
            rankChar = 'E';
        else
            rankChar = 'F';
        rank.text = "Rank: " + rankChar;
        yield return new WaitForSeconds(0.75f);
        audio.Play(blip);
        comment.gameObject.SetActive(true);
        comment.text = "Comment: " + rankComments[rankChar];
    }

    string SplitCamelCase(string s)
    {
        string output = Regex.Replace(s, "(?<!^)_?([A-Z])", " $1");
        return output;
    }
}
