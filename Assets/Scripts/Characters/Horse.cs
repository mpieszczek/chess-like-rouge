using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Horse : Player {

    // Use this for initialization
    protected override void Start () {
        base.Start();
        health = 3;
        directions = new Vector2[] { new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(0, -1) };
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
            foreach (Vector2 direction in directions)
            {
                for (int x = 1; x < boardManager.columns; x++)
                {
                    //overlap
                    Vector2 consideredTile = x*direction + (Vector2)transform.position;

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
                    else if (blocker.gameObject.GetComponent<Enemy>()!=null)
                    {
                        //enemyPositions.Add(hit.transform.gameObject.GetComponent<Enemy>());
                        continue;
                    }
                    else break; // 

                }
            }


            ////////
            CreateHighlights(isItJustMousePassingBy);
        }
    }

    public override void Move()
    {
        isMoving = true;
        boxCollider.enabled = false;
        RaycastHit2D[] hits = Physics2D.LinecastAll(transform.position, pointToMove, blockingLayer);
        boxCollider.enabled = true;
        foreach(RaycastHit2D hit in hits)
        {
            Pawn pawn = hit.collider.gameObject.GetComponent<Pawn>();
            if (pawn != null)
            {
                pawn.ReceiveDamage(dmg);
            }
        }
        base.Move();
    }
    void LateUpdate()
    {
        anim.SetBool("isMoving", isMoving);
    }
}
