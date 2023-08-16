using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Player {


    protected override void Start()
    {
        base.Start();
        directions = new Vector2[] { new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(0, -1) };
        health = 3;
    }
    public override void HighlightTiles(bool isItJustMousePassingBy)
    {
        foreach (Enemy enemy in gameManager.enemies)
        {
            if (enemy.isMoving || Enemy.numberOfSpecialEnemiesMoving != 0)
            {
                return;
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
                if(HasMoveAssigned)
                {
                    destinations.Remove(pointToMove);
                }
                Destroy(highlightedChoosedTile);
            }
            avialiablePositions.Clear();
            enemyPositions.Clear();
            specialPositions.Clear();

            Vector2 start = transform.position;
            foreach(Vector2 direction in directions) {
                for (int x = 1; x < boardManager.columns; x++)
                {
                    //linecast
                    Vector2 end = start + direction * x;
                    boxCollider.enabled = false;
                    RaycastHit2D hit = Physics2D.Linecast(start, end, blockingLayer);
                    boxCollider.enabled = true;

                    if (!hit)
                    {
                        bool isAlreadyChosen = false;
                        foreach (Vector3 destination in destinations)
                        {
                            if ((Vector2)destination == end)
                            {
                                isAlreadyChosen = true;
                            }
                        }
                        if (!isAlreadyChosen)
                            avialiablePositions.Add(end);
                    }
                    else if (hit.transform.tag == "Enemy")
                    {
                        enemyPositions.Add(hit.transform.gameObject.GetComponent<Enemy>());
                        break;
                    }
                    else break;
                }
            }


            ////////
            CreateHighlights(isItJustMousePassingBy);
        }
    }
    
}
