using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    public TeamManager teamManager;
    void Start()
    {
        teamManager = GameObject.FindWithTag("TeamManager").GetComponent<TeamManager>();
    }
    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Quited the game");
    }
    public void ChooseTeam(int num)
    {
        teamManager.ChooseTeam(num);
    }
    public void PlayTutorial()
    {
        SceneManager.LoadScene(2);
    }
}
