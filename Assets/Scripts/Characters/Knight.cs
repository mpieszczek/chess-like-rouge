using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Player {
    public GameObject PushBackHighlight;
    private List<GameObject> PushBackHighlights=new List<GameObject>();
    
    private Vector2[] enemyPushBackDirections = { new Vector2(1,0), new Vector2(-1, 0), new Vector2(0, -1), new Vector2(0, 1), new Vector2(1, 1), new Vector2(-1, 1), new Vector2(1, -1), new Vector2(-1, -1) };
    private Vector2 enemyPushBackPosition;
    private bool willSmashEnemy = false;
    public float curveParameter=1;
    public float curveFlatteningRate;
    public float jumpHight;
    protected override void Start()
    {
        base.Start();
        directions = new Vector2[] { new Vector2(2, 1), new Vector2(2, -1), new Vector2(-2, 1), new Vector2(-2, -1),
        new Vector2(1, -2),  new Vector2(-1, -2), new Vector2(1, 2), new Vector2(-1, 2) };
        health = 4;
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
                willSmashEnemy = false;
                HasAttackAssigned = false;
                currentlyControlled = this;
                IsChoosing = true;
                if (HasMoveAssigned)
                {
                    destinations.Remove(pointToMove);
                    destinations.Remove(enemyPushBackPosition);
                }
                Destroy(highlightedChoosedTile);
            }
            avialiablePositions.Clear();
            enemyPositions.Clear();
            specialPositions.Clear();
            foreach (Vector2 direction in directions)
            {
                Vector2 consideredTile = direction + (Vector2)transform.position;

                Collider2D blocker = Physics2D.OverlapPoint(consideredTile, blockingLayer);
                if (blocker!=null)
                {
                    if(blocker.transform.tag == "Enemy")
                    {
                        enemyPositions.Add(blocker.transform.gameObject.GetComponent<Enemy>());
                    }
                }

                else//nie ma nic
                {
                    //sprawdzenie czy to pole jest na planszy
                    if (consideredTile.x < boardManager.columns && consideredTile.x > -1 &&
                        consideredTile.y < boardManager.rows && consideredTile.y > -1 )
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

            CreateHighlights(isItJustMousePassingBy);
        }
    }
    private void HighlightEnemyPushBack(Vector2 enemyPosition)
    {
        SpecialMoveIsAssigned = true;
        PushBackHighlights.Clear();
        foreach(Vector2 direction in enemyPushBackDirections)
        {
            Vector2 consideredTile = direction + enemyPosition;
            Collider2D blocker = Physics2D.OverlapPoint(consideredTile, blockingLayer);
            if (blocker == null)
            {
                bool isAlreadyChosen = false;
                foreach(Vector3 destination in destinations)
                {
                    if ((Vector2)destination == consideredTile)
                    {
                        isAlreadyChosen = true;
                    }
                }
                if (!isAlreadyChosen)
                {
                    GameObject instance = Instantiate(PushBackHighlight, consideredTile, Quaternion.identity);
                    PushBackHighlights.Add(instance);
                    instance.GetComponent<HighlightPushBack>().owner = this;
                }
            }
        }
        if (PushBackHighlights.Capacity == 0)
        {
            willSmashEnemy = true;
            SpecialMoveIsAssigned = false;
        }
    }
    public void PushBackAssign(Vector2 pushPosition)
    {
        SpecialMoveIsAssigned = false;
        enemyPushBackPosition = pushPosition;
        destinations.Add(pushPosition);
        foreach (GameObject tile in PushBackHighlights)
        {
            Destroy(tile);

        }
        PushBackHighlights.Clear();
        SpecialMoveIsAssigned = false;
    }
    public override void AttackAssignement(Enemy target)
    {
        //base.AttackAssignement(target);
        MoveAssignement(target.transform.position);
        HasAttackAssigned = true;
        targetEnemy = target;
        HighlightEnemyPushBack(target.transform.position);
    }
    public override void Move()
    {
        if (HasMoveAssigned)
        {
            UnhighlightTiles();
            Destroy(highlightedChoosedTile);
            HasMoveAssigned = false;
            isMoving = true;
            anim.Play("Jump");
        }
    }
    private void MoveEnemy()
    {
        if (willSmashEnemy)
        {
            targetEnemy.Death();
        }
        else
        {
            targetEnemy.ReceiveDamage(dmg);
            targetEnemy.MoveAssignement(enemyPushBackPosition);
            targetEnemy.PushBacked();
        }
    }
    protected override void FixedUpdate()
    {
        if (isMoving)
        {
            if (curveParameter > 0)
                curveParameter = curveParameter - curveFlatteningRate;
            else
                curveParameter = 0;
            Vector3 curveOffest = jumpHight*Vector3.up*curveParameter*inverseMoveTime;
            transform.position = Vector3.MoveTowards(transform.position, pointToMove + curveOffest, Time.fixedDeltaTime * inverseMoveTime * (16*curveParameter*curveParameter -16*curveParameter + 5));
            if ((transform.position - pointToMove).magnitude < Mathf.Epsilon)
            {
                if (HasAttackAssigned)
                {
                    HasAttackAssigned = false;
                    if(targetEnemy!=null)
                        MoveEnemy();
                    
                }
                isMoving = false;
                curveParameter = 1;
                anim.Play("Idle");
            }
        }
    }
}

