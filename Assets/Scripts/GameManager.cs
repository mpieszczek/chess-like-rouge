using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameManager : MonoBehaviour {
    public static GameManager instance = null;
    public List<Player> players = new List<Player>();
    public List<Enemy> enemies = new List<Enemy>();
    public List<TemporaryEffect> effects = new List<TemporaryEffect>();
    BoardManager boardScript;
    public GameObject SkillsPanel;
    private int level = 0;
    public bool UIOn = false;
    public GameObject InfoLabelTemplate;
	void Awake () {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        boardScript = GetComponent<BoardManager>();
        
    }
    public void MoveAll()
    {
        bool allAssigned = true;
        foreach (Player player in players)
        {
            if (!player.GetComponent<Player>().HasMoveAssigned)
                allAssigned = false;
        }
        
        if (allAssigned && !Player.SpecialMoveIsAssigned)
        {
            foreach (Player player in players)
            {
                player.GetComponent<Player>().Move();
            }
            Player.ClearingDestinationList();
            //Now move all enemies
            Invoke("MoveEnemies", .5f);
        }
        
    }
    void MoveEnemies()
    {
        foreach(Player player in players)
        {
            if (player.isMoving || Player.SpecialMoveIsAssigned)
            {
                Invoke("MoveEnemies", .5f);
                return;
            }
        }
        foreach(Enemy enemy in enemies)
        {
            if (enemy.isMoving)
            {
                Invoke("MoveEnemies", .5f);
                return;
            }
        }
        foreach (Enemy enemy in enemies)
        {
            enemy.GetComponent<Enemy>().ChooseTile();
        }
        foreach (Enemy enemy in enemies)
        {
            enemy.GetComponent<Enemy>().Move();
        }
        Invoke("FattigueEffects", .5f);
        if (boardScript.isItATutorial)
            GetComponent<TutorialInformationController>().Invoke("NextPanel", 2);
    }
    void FattigueEffects()
    {
        List<TemporaryEffect> toRemove = new List<TemporaryEffect>();
        foreach (TemporaryEffect effect in effects)
        {
            if (effect != null)
                effect.Fattigue();
            else
                toRemove.Add(effect);
        }
        //cleaning
        foreach(TemporaryEffect effect in toRemove)
        {
            effects.Remove(effect);
        }
    }
	void InitGame()
    {
        players.Clear();
        enemies.Clear();
        boardScript.SetupScene(level);
        foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            players.Add(player.GetComponent<Player>());
            
        }
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemies.Add(enemy.GetComponent<Enemy>());
        }
        if (!boardScript.isItATutorial)
        {
            Text levelDisplay = GameObject.FindGameObjectWithTag("LevelCount").GetComponent<Text>();
            levelDisplay.text = "Level: " + level;
        }
    }
	void Update () {
        if (Input.GetButtonUp("Jump"))
            MoveAll();
    }
    public void DestroyPawn (Pawn pawn)
    {
        if (pawn as Player !=null)
            players.Remove(pawn as Player);
        if (pawn as Enemy != null)
            enemies.Remove(pawn as Enemy);
        //We do that in Pawn class just to leave time for death animation
        //Destroy(pawn.gameObject);
        Debug.Log("updated lists");
        if (enemies==null || enemies.Count == 0)
        {
            //Debug.Log("You won");
            Invoke("LoadNextLevel", 2);
        }

        if (players == null || players.Count == 0)
        {
            //Debug.Log("You lost");
            level = 0;
            Invoke("LoadMainMenu", 1);

        }
    }
    void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
        Destroy(gameObject); // niszczymy gameManagera żeby nie mieć problemu przy zmianie trybu gry gdzie wystepują pewne zmiany przy nich
    }
    void LoadNextLevel()
    {
        if(boardScript.isItATutorial)
            SceneManager.LoadScene(2);
        else
            SceneManager.LoadScene(1);
    }
    void OnEnable()
    {
        
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 1)
        {
            level++;
            CancelInvoke();                 //żeby nie odpalić ruchu przeciwników który jest invokowany
            InitGame();
        }
        if (scene.buildIndex == 2)
        {
            level++;
            if (level == 3)
            {
                LoadMainMenu();
            }
            CancelInvoke();                 //żeby nie odpalić ruchu przeciwników który jest invokowany

            InitGame();
            GetComponent<TutorialInformationController>().FindPanel();
            GetComponent<TutorialInformationController>().NextLevel();
            
        }
        SkillsPanel = GameObject.FindWithTag("SkillsPanel");
        SkillsPanel.SetActive(false);
    }
}
