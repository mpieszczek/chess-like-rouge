using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialInformationController : MonoBehaviour {

    public string[] list1;
    public string[] list2;
    string[][] instructions;
    int turn = 0;
    int level = -1;
    
    public GameObject instructionsUI;
    public TextMeshProUGUI instructionText;

    void Awake()
    {
        instructions = new string[][] { list1, list2 };
    }
    public void NextPanel()
    {
        if (level<instructions.Length && turn<instructions[level].Length && instructions[level][turn] != null && !instructionsUI.activeSelf)
        {
            instructionsUI.SetActive(true);
            instructionText.text = instructions[level][turn];
            GameManager.instance.UIOn = true;
            turn++;
        }
        
    }
    public void NextLevel()
    {
        level++;
        turn = 0;
        NextPanel();
    }
    public void IReadInstructions()
    {
        instructionsUI.SetActive(false);
        GameManager.instance.UIOn = false;
    }
    public void FindPanel()
    {
        instructionsUI = GameObject.FindWithTag("InstPanel");
        instructionText = instructionsUI.GetComponentInChildren<TextMeshProUGUI>();
        instructionsUI.SetActive(false);
    }
}
