using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Pewds : Player {

    private List<GameObject> SpawnHighlights = new List<GameObject>();
    private Vector2 spawnPosition;
    private bool HasSpawnAssigned = false;
    private bool HasHealAssigned = false;
    public GameObject NineYearOldPrefab;
    private bool IamChoosingSpawnPlace = false;
    protected override void Start()
    {
        base.Start();
        health = 4;
        directions = new Vector2[] { new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, -1), new Vector2(0, 1),
            new Vector2(1, 1), new Vector2(-1, 1), new Vector2(1, -1), new Vector2(-1, -1) };
        
    }
    public override void HighlightTiles(bool isItJustMousePassingBy)
    {
        if (IamChoosingSpawnPlace)
            return;
        foreach (Enemy enemy in gameManager.enemies)
        {
            if (enemy.isMoving || Enemy.numberOfSpecialEnemiesMoving != 0)
            {
                return;                                                     //Jeśli jakikolwiek przeciwnik się rusza to nie wyświetlamy kafli
            }
        }
        
        if (!IsChoosing && !SpecialMoveIsAssigned)
        {
            //unhighlight other players and preper lists
            if (currentlyControlled != null && !isItJustMousePassingBy)
            {
                currentlyControlled.UnhighlightTiles();
            }
            if (!isItJustMousePassingBy)
            {
                HasAttackAssigned = false;
                currentlyControlled = this;
                IsChoosing = true;
                HasSpawnAssigned = false;
                HasHealAssigned = false;
                if (HasMoveAssigned)
                {
                    destinations.Remove(pointToMove);
                }
                Destroy(highlightedChoosedTile);
            }
            avialiablePositions.Clear();
            enemyPositions.Clear();
            specialPositions.Clear();

            if (!isItJustMousePassingBy)
            {
                GameObject panel = GameManager.instance.SkillsPanel;
                panel.SetActive(true);
                panel.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(HighlightSpawn);
                panel.transform.GetChild(0).GetComponentInChildren<Text>().text = "Spawn 9YO";
                panel.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(HealAssign);
                panel.transform.GetChild(1).GetComponentInChildren<Text>().text = "Check health";
                panel.transform.GetChild(2).gameObject.SetActive(false);
            }
            Vector2 start = transform.position;
            foreach (Vector2 direction in directions)
            {

                //overlap
                Vector2 consideredTile = direction + (Vector2)transform.position;

                Collider2D blocker = Physics2D.OverlapPoint(consideredTile, blockingLayer);
                if (blocker == null)
                {
                    bool isAlreadyChosen = false;
                    foreach (Vector3 destination in destinations)
                    {
                        if ((Vector2)destination == consideredTile)
                        {
                            isAlreadyChosen = true;
                        }
                    }
                    if (!isAlreadyChosen)
                        avialiablePositions.Add(consideredTile);
                }
            }
        }
        ////////
        CreateHighlights(isItJustMousePassingBy);
    }
    void HighlightSpawn()
    {
        IamChoosingSpawnPlace = true;
        UnhighlightTiles();
        SpecialMoveIsAssigned = true;
        IsChoosing = true;
        Debug.Log("Spawn");
        GameObject panel = GameManager.instance.SkillsPanel;
        panel.SetActive(false);
        foreach (Button child in panel.transform.GetComponentsInChildren<Button>()) {
            child.onClick.RemoveAllListeners();
        }
        SpawnHighlights.Clear();

        foreach (Vector2 direction in directions)
        {
            Vector2 consideredTile = direction + (Vector2)transform.position;
            Collider2D blocker = Physics2D.OverlapPoint(consideredTile, blockingLayer);
            if (blocker == null)
            {
                bool isAlreadyChosen = false;
                foreach (Vector3 destination in destinations)
                {
                    if ((Vector2)destination == consideredTile)
                    {
                        isAlreadyChosen = true;
                    }
                }
                if (!isAlreadyChosen)
                {
                    GameObject instance = Instantiate(SpecialMoveHighlight, consideredTile, Quaternion.identity);
                    SpawnHighlights.Add(instance);
                    highlightedTiles.Add(instance);
                    instance.GetComponent<HighlightSpecialAttack>().owner = this;
                }
            }
        }
        if (SpawnHighlights.Capacity == 0)
        {
            //////////////////////////////////////////Tutaj powinno być anulowanie albo wczęssniej sprawdzać czy można spawnować i disablować przycisk
            SpecialMoveIsAssigned = false;
            IamChoosingSpawnPlace = false;
        }
    }
    void HealAssign()
    {
        UnhighlightTiles();
        Debug.Log("Heal");
        GameObject panel = GameManager.instance.SkillsPanel;
        panel.SetActive(false);
        foreach (Button child in panel.transform.GetComponentsInChildren<Button>())
        {
            child.onClick.RemoveAllListeners();
        }
        HasHealAssigned = true;
        MoveAssignement(transform.position);
    }
    public override void SpecialMoveAssign(GameObject tile) //spawn
    {
        
        SpecialMoveIsAssigned = false;
        IamChoosingSpawnPlace = false;
        spawnPosition = tile.transform.position;
        UnhighlightTiles();
        HasSpawnAssigned = true;
        MoveAssignement(transform.position);
    }

    public override void Move()
    {
        if (HasMoveAssigned)
        {
            HasMoveAssigned = false;
            isMoving = true;
            if (HasHealAssigned)
            {
                isMoving = false;
                HasHealAssigned = false;
                Heal();
            }
            else if (HasSpawnAssigned)
            {
                isMoving = false;
                HasSpawnAssigned = false;
                anim.Play("Spawn");
                Invoke("Spawn", 1);
                SpecialMoveIsAssigned = true;
            }
        }
        UnhighlightTiles();
        Destroy(highlightedChoosedTile);
    }
    private void Heal()
    {
        anim.Play("Heal");
        health = GetComponentInChildren<HealthbarDisplay>().healthPoints;
        GetComponentInChildren<HealthbarDisplay>().UpdateHealth(GetComponentInChildren<HealthbarDisplay>().healthPoints);
    }
    private void Spawn()
    {
        GameObject nine =Instantiate(NineYearOldPrefab, spawnPosition, Quaternion.identity);
        GameManager.instance.players.Add(nine.GetComponent<Player>());
        SpecialMoveIsAssigned = false;
    }
    void LateUpdate()
    {
            anim.SetBool("isMoving", isMoving);
    }
}
