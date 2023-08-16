using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : Enemy {

    public bool activeted = false;
    int movesPerTurn = 3;
    int move = 1;
    protected override void Start()
    {
        base.Start();
        health = 3;
        favouriteAxis = new Vector3(1, 1, 0);//vertical
        //na ukos
        possibleMoves = new Vector3[] {new Vector3(1,0,0), new Vector3(-1, 0, 0), new Vector3(0, 1, 0),
            new Vector3(0,-1,0), new Vector3(1,1,0), new Vector3(-1, -1, 0), new Vector3(-1, 1, 0), new Vector3(1, -1, 0) };
        //ChooseTarget();
    }

    protected override void ChooseTarget()
    {
        if (!activeted)
        {
            chaseTarget = null;
            Vector2 start = transform.position;
            foreach (Vector2 direction in possibleMoves)
            {
                
                    //linecast
                    Vector2 end = direction;
                    boxCollider.enabled = false;
                    RaycastHit2D hit = Physics2D.Raycast(start, end, blockingLayer);
                    boxCollider.enabled = true;

                    if (hit)
                    {
                        if (hit.transform.tag == "Player")
                        {
                            activeted = true;
                            anim.Play("Wake");
                            return;
                        }
                    }
            }



        }
        else{
            if (chaseTarget == null)
                chaseTarget = gameManager.players[0];
            foreach (Player player in gameManager.players)
            {
                if (player.provocation > chaseTarget.provocation)
                {
                    chaseTarget = player;
                }
            }
        }
    }

    public override void Move()
    {
        if(move==1)
            numberOfSpecialEnemiesMoving++;
        if (move != 1 && chaseTarget == null)
            ChooseTarget();
        if (chaseTarget == null || gameManager.players.Capacity == 0 || gameManager.players == null)
        {
            numberOfSpecialEnemiesMoving--;
            return;
        }
        base.Move();
        if (move >= movesPerTurn)
        {
            numberOfSpecialEnemiesMoving--;
            move = 1;
        }
        else
        {
            Invoke("ChooseTile", 1f);
            Invoke("Move", 1.2f);
            move++;
        }
    }
    public override void ReceiveDamage(int amount)
    {
        base.ReceiveDamage(amount);
        activeted = true;
    }
}
