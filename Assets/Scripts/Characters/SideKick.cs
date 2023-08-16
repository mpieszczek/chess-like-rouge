using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideKick : Player {
    private Lord hisLord;
    private bool hasSpecialMoveAssigned=false;
    protected override void Start()
    {
        base.Start();
        health = 2;
        directions = new Vector2[] { new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, -1), new Vector2(0, 1),
            new Vector2(1, 1), new Vector2(-1, 1), new Vector2(1, -1), new Vector2(-1, -1),
            new Vector2(2, 0), new Vector2(-2, 0), new Vector2(0, -2), new Vector2(0, 2),
            new Vector2(2, 2), new Vector2(-2, 2), new Vector2(2, -2), new Vector2(-2, -2),
            new Vector2(2, 1), new Vector2(-2, 1), new Vector2(2, -1), new Vector2(-2, -1),
            new Vector2(1, 2), new Vector2(-1, 2), new Vector2(1, -2), new Vector2(-1, -2)};
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
                if (HasMoveAssigned)
                {
                    destinations.Remove(pointToMove);
                }
                Destroy(highlightedChoosedTile);
            }
            avialiablePositions.Clear();
            enemyPositions.Clear();
            specialPositions.Clear();
            Vector2 start = transform.position;
            hisLord = null;
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
                        //sprawdzenie czy to pole jest na planszy
                        if (consideredTile.x < boardManager.columns && consideredTile.x > -1 &&
                            consideredTile.y < boardManager.rows && consideredTile.y > -1)
                        {
                            avialiablePositions.Add(consideredTile);
                        }
                }
                else
                {
                    Lord lord = blocker.gameObject.GetComponent<Lord>();
                    if (lord != null)
                    {
                        hisLord = lord;
                        if(!hisLord.IsStanding())
                            specialPositions.Add(hisLord.transform.position);
                    }
                }
            }
            CreateHighlights(isItJustMousePassingBy);
        }
    }
    //Assigning picking up the lord
    public override void SpecialMoveAssign(GameObject tile)
    {
        MoveAssignement(transform.position);
        hasSpecialMoveAssigned = true;
        pointToMove = transform.position;
        UnhighlightTiles();
    }
    public override void Move()
    {
        if (HasMoveAssigned)
        {
            HasMoveAssigned = false;
            isMoving = true;
            if (hasSpecialMoveAssigned)
            {
                HasAttackAssigned = false;
                if (hisLord != null)
                {
                    hisLord.GetUp();
                }
                anim.Play("Special");
                hasSpecialMoveAssigned = false;
                isMoving = false;
            }
            else {
                transform.GetChild(0).gameObject.SetActive(false);
                anim.Play("MoveStart");
                Invoke("EndMove", 1);
            }
        }
        UnhighlightTiles();
        Destroy(highlightedChoosedTile);
    }
    private void EndMove()
    {
        transform.position = pointToMove;
        anim.Play("MoveEnd");
        isMoving = false;
        transform.GetChild(0).gameObject.SetActive(true);
        GetComponent<SpriteRenderer>().sortingOrder = 100 - 10 * (int)transform.position.y + (int)transform.position.x;
    }
    protected override void FixedUpdate()
    {
        
    }
}
