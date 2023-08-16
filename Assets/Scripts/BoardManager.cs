using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using UnityEngine;

public class BoardManager : MonoBehaviour
{

    [Serializable]
    public class Count
    {

        public int maximum;
        public int minimum; 

        public Count (int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }
    public bool isItATutorial = false;
    public int columns = 8;
    public int rows = 8;
    public Count wallCount = new Count(5, 9);
    public Count foodCount = new Count(1, 5);
    
    //Tiles
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;
    public GameObject[] playerTiles;
    public GameObject[] bossTiles;
    private Transform boardHolder;
    private List<Vector3> gridPositions = new List<Vector3>();

    void Start()
    {

    }
    void InitialiseList()
    {
        gridPositions.Clear();

        for(int x = 1; x < columns - 1; x++)
        {
            for(int y = 1; y < rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    void BoardSetup()//podłoga i zewnętrzne ściany
    {
        boardHolder = new GameObject("Board").transform;

        for (int x = -1; x < columns+1; x++)
        {
            for (int y = -1; y < rows+1; y++)
            {
                GameObject toInstantiate =floorTiles[Random.Range(0,floorTiles.Length)];
                if (x == -1 || x == columns || y == -1 || y == rows)
                {
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                }
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 1f), Quaternion.identity)as GameObject;

                instance.transform.SetParent(boardHolder);
            }
        }
    }
    
    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject[] tileArray,int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);
        for(int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0,tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }
    void LayoutAllObjectAtRandomPosition(GameObject[] tileArray)
    {
        for (int i = 0; i < tileArray.Length; i++)
        {
            Vector3 randomPosition = RandomPosition();
            Instantiate(tileArray[i], randomPosition, Quaternion.identity);
        }
    }
    public void SetupScene(int level)
    {
        if (isItATutorial)
        {
            
            BoardSetup();
            InitialiseList();
            if (level == 1)
            {
                Instantiate(enemyTiles[0], new Vector3(2, 1), Quaternion.identity);
                Instantiate(playerTiles[0], new Vector3(4, 1), Quaternion.identity);
            }
            if (level == 2)
            {
                Instantiate(enemyTiles[0], new Vector3(2, 1), Quaternion.identity);
                Instantiate(enemyTiles[0], new Vector3(2, 3), Quaternion.identity);
                Instantiate(playerTiles[0], new Vector3(4, 1), Quaternion.identity);
                Instantiate(playerTiles[1], new Vector3(4, 3), Quaternion.identity);
            }
        }
        else
        {
            //get the choosen team
            if (level == 1)
            {
                GameObject teamManager = GameObject.FindWithTag("TeamManager");
                playerTiles = teamManager.GetComponent<TeamManager>().choosedTeam;
                Destroy(teamManager);
            }
            BoardSetup();
            InitialiseList();
            LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
            //LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
            //int enemyCount = (int)Mathf.Log(level, 2f)+1;

            if (level % 2 == 0)
            {
                //boss fight
                LayoutObjectAtRandom(bossTiles, 1, 1);
            }
            else
            {
                int enemyCount = 3 + (int)level / 2;
                LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
            }
            LayoutAllObjectAtRandomPosition(playerTiles);
            //LayoutObjectAtRandom(playerTiles,3,3);
            //Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
        }
    }
}
