using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lord : Player {

    private bool liesDown=false;
    private bool hasSpecialAttackAssigned=false;
    
    List<Enemy> enemies = new List<Enemy>(); 
    protected override void Start()
    {
        base.Start();
        health = 3;
        directions = new Vector2[] { new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, -1), new Vector2(0, 1),
            new Vector2(1, 1), new Vector2(-1, 1), new Vector2(1, -1), new Vector2(-1, -1) };
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
                hasSpecialAttackAssigned = false;
                if (HasMoveAssigned)
                {
                    destinations.Remove(pointToMove);
                }
                Destroy(highlightedChoosedTile);
            }
            avialiablePositions.Clear();
            enemyPositions.Clear();
            specialPositions.Clear();

            if (!liesDown)
            {
                
                specialPositions.Add(transform.position);

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
    }
    public override void SpecialMoveAssign(GameObject tile)
    {
        enemies.Clear();
        MoveAssignement(transform.position);
        UnhighlightTiles();
        hasSpecialAttackAssigned = true;
        Vector2 start = transform.position;
        foreach (Vector2 direction in directions)
        {

            //overlap
            Vector2 consideredTile = direction + (Vector2)transform.position;

            Collider2D blocker = Physics2D.OverlapPoint(consideredTile, blockingLayer);
            if (blocker != null)
            {
                Enemy enemy = blocker.gameObject.GetComponent<Enemy>();
                if (enemy != null)
                    enemies.Add(enemy);
                    
            }
        }
    }
    public override void Move()
    {
        if (HasMoveAssigned)
        {
            HasMoveAssigned = false;
            isMoving = true;
            if (hasSpecialAttackAssigned)
            {
                hasSpecialAttackAssigned = false;
                anim.Play("Attack");
                foreach (Enemy enemy in enemies)
                {
                    if (enemy != null)
                    {
                            enemy.ReceiveDamage(dmg);
                    }
                    
                }
                enemies.Clear();
                liesDown = true;
                anim.SetBool("IsStanding",false);
            }
        }
        UnhighlightTiles();
        Destroy(highlightedChoosedTile);
    }
    public void GetUp()
    {
        anim.SetBool("IsStanding", true);
        liesDown = false;
        //anim.Play("Idle");
    }
    public bool IsStanding() { return !liesDown; }
}
