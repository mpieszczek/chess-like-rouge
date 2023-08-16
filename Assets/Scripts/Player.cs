using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Jedyne co nadpisujemy przy konkretnych postaciach to kierunki w ktorych moze poruszac się postac w highligh tiles i specjalny ruch
public class Player : Pawn {
    protected bool IsChoosing=false;
    protected static Player currentlyControlled;
    protected static List<Vector3> destinations = new List<Vector3>();
    protected List<Vector3> avialiablePositions = new List<Vector3>();
    protected List<Enemy> enemyPositions = new List<Enemy>();
    protected List<Vector3> specialPositions = new List<Vector3>();
    public static bool SpecialMoveIsAssigned = false;

    public GameObject MoveHighlight;
    public GameObject EnemyHighlight;
    public GameObject SpecialMoveHighlight;
    public GameObject PassingMouseMoveHighlight;
    public GameObject PassingMouseEnemyHighlight;
    public GameObject PassingMouseSpecialmoveHighlight;
    public GameObject ReservedHighlight;

    protected GameObject highlightedChoosedTile;
    public int provocation;
    protected List<GameObject> highlightedTiles = new List<GameObject>();
    protected Vector2[] directions;
    void OnMouseDown()
    {
        //Po nacisnieciu postaci wylaczamy wczesniejsze podswietlenia i pokazujemy mozliwe ruchy dla danego pionka
        if (!gameManager.UIOn)
        {
            UnhighlightTiles();
            HighlightTiles(false);//w kafelkach jest funkcja po wciśnięciu której wybierany jest docelowy kafel porzuszenia się
            //Tutaj powinno uruchomic wyswietlanie informacji o postaci na boku
        }
    }
    void OnMouseEnter()
    {
        //Po najechaniu lekko podswietlamy mozliwe ruchy dla postaci najechanej
        if (!IsChoosing && !isMoving && !gameManager.UIOn)
        {
            HighlightTiles(true);
        }
    }
    void OnMouseExit()
    {
        // Podswietlenie ruchow zostaje tylko jak nacisniemy postac
        if (!IsChoosing)
        {
            UnhighlightTiles();
        }
    }
    public override void Move()
    {
        //dodatkowo wylaczamy podswietlenie docelowego pola
        base.Move();
        UnhighlightTiles(); //to na wszelki wypadek
        Destroy(highlightedChoosedTile);
    }
    public static void ClearingDestinationList()
    {
        //czysci liste wybranych ruchow dla Playerow. Odpalane po ruszeniu wszystkimi pionkami w GameMenagerze
        destinations.Clear();
    }
    public override void MoveAssignement(Vector3 moveAssigned)
    {
        //wylaczamy interfejs z dostepnymi ruchami specjanymi
        GameObject panel = GameManager.instance.SkillsPanel; 
        panel.SetActive(false); 
        foreach (Button child in panel.transform.GetComponentsInChildren<Button>())
        {
            child.onClick.RemoveAllListeners();
        }

        //gdybyśmy przypadkiem nie sprawdzili w HighlighTiles czy pozycje już są wybrane przez inne pionki to robimy to tutaj
        if (destinations.Count != 0)                           
        {
            foreach (Vector3 destination in destinations)
            {
                if (destination == moveAssigned)
                {
                    Debug.Log("You cannot choose it");
                    Debug.Log(moveAssigned);
                    return;
                }
            }
        }
        //jesli wszytsko jest ok to dodajemy do ruch do listy ruchow do wykonania
        base.MoveAssignement(moveAssigned);
        destinations.Add(moveAssigned);
        highlightedChoosedTile = Instantiate(ReservedHighlight, pointToMove, Quaternion.identity);//zaznaczamy na planszy ze pole jest wybrane
    }
    public virtual void AttackAssignement(Enemy target)
    {
        //podobnie jak ruch ale zapisujemy wroga ktorego uderzymy i pozycje na ktora sie przemiescimy
        Vector3 offset = (transform.position - target.transform.position).normalized; 
        offset.x = Mathf.RoundToInt(offset.x);
        offset.y = Mathf.RoundToInt(offset.y);
        offset.z = Mathf.RoundToInt(offset.z);

        MoveAssignement(target.transform.position + offset);
        HasAttackAssigned = true;
        targetEnemy = target;
    }
    public void UnhighlightTiles()
    {
        foreach (GameObject tile in highlightedTiles)
        {
            Destroy(tile);
        }
        highlightedTiles.Clear();
        IsChoosing = false;
    }
    public virtual void HighlightTiles(bool isItJustMousePassingBy)
    {
        
    }
    protected void CreateHighlights(bool justPassingBy)
    {
        //kazdy typ ruchu zaznaczamy innym kolorem/pewnie pozniej grafika

        //jezeli nie ma dostepnych ruchow to automatycznie przypisujemy postaci stanie w miejscu
        if (avialiablePositions.Count == 0 && enemyPositions.Count == 0 && specialPositions.Count == 0 && !justPassingBy)
        {
            UnhighlightTiles();
            MoveAssignement(transform.position);
        }
        foreach (Vector3 v in avialiablePositions)
        {
            GameObject instance;
            if (!justPassingBy)
            {
                instance = Instantiate(MoveHighlight, v + new Vector3(0, 0, -1), Quaternion.identity);
                instance.GetComponent<HighlightChosingTile>().owner = this;
            }
            else
                instance = Instantiate(PassingMouseMoveHighlight, v + new Vector3(0, 0, -1), Quaternion.identity);
            highlightedTiles.Add(instance);

        }
        foreach (Enemy enemy in enemyPositions)
        {
            GameObject instance;
            if (!justPassingBy)
            {
                instance = Instantiate(EnemyHighlight, enemy.transform.position + new Vector3(0, 0, -1), Quaternion.identity);
                instance.GetComponent<HighlightEnemyTile>().enemy = enemy;
                instance.GetComponent<HighlightEnemyTile>().owner = this;
            }
            else
                instance = Instantiate(PassingMouseEnemyHighlight, enemy.transform.position + new Vector3(0, 0, -1), Quaternion.identity);
            highlightedTiles.Add(instance);

        }
        foreach(Vector3 specMove in specialPositions)
        {
            GameObject instance;
            if (!justPassingBy)
            {
                instance = Instantiate(SpecialMoveHighlight, specMove + new Vector3(0, 0, -1), Quaternion.identity);
                instance.GetComponent<HighlightSpecialAttack>().owner = this;
            }
            else
                instance = Instantiate(PassingMouseSpecialmoveHighlight, specMove + new Vector3(0, 0, -1), Quaternion.identity);
            highlightedTiles.Add(instance);
        }
    }
    public virtual void SpecialMoveAssign(GameObject tile)
    {

    }
    public override void Death()
    {
        base.Death();
        UnhighlightTiles();
    }
}
