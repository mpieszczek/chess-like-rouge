using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeamManager : MonoBehaviour {
    public GameObject[] teams;
    public GameObject[] choosedTeam=null;
    public static TeamManager instance = null;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
    public void ChooseTeam(int num)
    {
        choosedTeam = teams[num].GetComponent<TeamScript>().units;
        SceneManager.LoadScene(1);
    }
}
