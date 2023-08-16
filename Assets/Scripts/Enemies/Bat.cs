using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : Enemy {
    //Seaks the lowest on life character and chase him
    //Moves by 2 tiles(it ativates move function 2 times in a row) i nany direction
    //in case of draw he goes on the one with highest provocation
    //when choosing tile after seting target it checks which tile from accessible is the nearest to the character on straight line
    //when draw it should favourite vertical or horizontal direction
    int movesPerTurn = 2;
    int move = 1;
    protected override void Start()
    {
        base.Start();
        health = 2;
        favouriteAxis = new Vector3(0, 1, 0);//vertical
        possibleMoves = new Vector3[] {new Vector3(1,0,0), new Vector3(-1, 0, 0), new Vector3(0, 1, 0),
            new Vector3(0,-1,0), new Vector3(1,1,0), new Vector3(-1, -1, 0), new Vector3(-1, 1, 0), new Vector3(1, -1, 0) };
        ChooseTarget();
        chaseTarget = gameManager.players[0];
    }
    protected override void ChooseTarget()
    {
        foreach (Player player in gameManager.players)
        {
            if (chaseTarget == null)
                chaseTarget = player;
            if (player.health < chaseTarget.health)
            {
                chaseTarget = player;
            }
            else if (player.health == chaseTarget.health)
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
        if (move == 1)
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
    public override void PushBacked()
    {
        base.Move();
    }
}
