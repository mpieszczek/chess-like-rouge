using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NineYearOld : Player {


    protected override void Start()
    {
        base.Start();
        health = 1;
        directions = new Vector2[] { new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, -1), new Vector2(0, 1),
            new Vector2(1, 1), new Vector2(-1, 1), new Vector2(1, -1), new Vector2(-1, -1) };
    }
    public override void HighlightTiles(bool isItJustMousePassingBy)
    {

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
                if (HasMoveAssigned)
                {
                    destinations.Remove(pointToMove);
                }
                Destroy(highlightedChoosedTile);
            }
            avialiablePositions.Clear();
            enemyPositions.Clear();
            specialPositions.Clear();
            //TO DO!!!!!!
            //We have 2 special actions so we will have side buttons
            //which will appere here

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
                else
                {
                    if(blocker.transform.tag == "Enemy")
                    {
                        enemyPositions.Add(blocker.transform.gameObject.GetComponent<Enemy>());
                    }
                }
            }
        }
        ////////
        CreateHighlights(isItJustMousePassingBy);
    }
    void LateUpdate()
    {
        anim.SetBool("isMoving", isMoving);
    }
}
