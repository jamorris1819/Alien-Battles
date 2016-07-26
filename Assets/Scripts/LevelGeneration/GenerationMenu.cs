using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class GenerationMenu : MonoBehaviour {

    public Transform player;
    public Camera cam;
    public Camera cam2;
    public Text roomSize;
    public Text currentState;
    public Slider sliderChoice;
    public GameObject warning;
    bool generating = false;
    GenerateDungeon generateDungeon;
    int[] roomSizes = new int[] { 25, 50, 150, 300, 500 };
    string[] roomSizeNames = new string[] { "Small", "Normal", "Large", "Huge", "Gigantic" };

    public GameObject title;
    public GameObject panel;
    public GameObject ui;
    public GameObject minimapCam;
    public GameObject manager;
    public GameObject playerObj;

    void Start()
    {
        generateDungeon = GetComponent<GenerateDungeon>();
        generateDungeon.Generated += Play;
    }

    void Play()
    {
        title.SetActive(false);
        panel.SetActive(false);
        ui.SetActive(true);
        minimapCam.SetActive(true);
        manager.SetActive(true);
        playerObj.SetActive(true);
        player.position = generateDungeon.startingPoint;
    }

    public void StartGeneration()
    {
        generateDungeon.StartGeneration(roomSizes[(int)sliderChoice.value]);
        generating = true;
    }

    void Update()
    {
        roomSize.text = roomSizeNames[(int)sliderChoice.value];
        warning.SetActive(sliderChoice.value > 2);
        if (generating)
            currentState.text = SplitCamelCase(generateDungeon.generationStage.ToString());
    }

    string SplitCamelCase(string s)
    {
        string output = Regex.Replace(s, "(?<!^)_?([A-Z])", " $1");
        return output;
    }
}
