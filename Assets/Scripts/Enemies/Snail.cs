using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snail : Enemy {

    public GameObject Goo;

    bool hidding = false;
    protected override void Start()
    {
        base.Start();
        health = 3;
        favouriteAxis = new Vector3(0, 1, 0);//vertical
        //na 
        possibleMoves = new Vector3[] {new Vector3(1,0,0), new Vector3(-1, 0, 0), new Vector3(0, 1, 0),
            new Vector3(0,-1,0), new Vector3(1,1,0), new Vector3(-1, -1, 0), new Vector3(-1, 1, 0), new Vector3(1, -1, 0) };
        ChooseTarget();
    }
    protected override void ChooseTarget()
    { 
        if (chaseTarget == null)
            chaseTarget = gameManager.players[0];
        foreach (Player player in gameManager.players)
        {
            float chaseingDistance = (chaseTarget.transform.position - transform.position).magnitude;
            float currentDistance = (player.transform.position - transform.position).magnitude;
            if (currentDistance < chaseingDistance)
            {
                chaseTarget = player;
            }
            else if (currentDistance == chaseingDistance)
            {
                if (player.provocation > chaseTarget.provocation)
                {
                    chaseTarget = player;
                }
            }
        }
    }
    public override void ChooseTile()
    {
        //Searching for target
        if (hidding)
        {
            hidding = false;
            MoveAssignement(transform.position);
            anim.Play("CommingOut");
            return;
        }
        ChooseTarget();
        if (chaseTarget != null)
        {
            int i = 0;
            Vector3 tileWithShortestDistance = possibleMoves[0] + transform.position;
            bool everyTileOccupied = true;
            foreach (Vector3 move in possibleMoves)
            {
                Vector3 currentlyConsideredTile = move + transform.position;
                if (ChoosedTiles.Contains(currentlyConsideredTile))
                    continue;
                //sprawdzenie czy kafel jest wolny
                Collider2D blocker = Physics2D.OverlapPoint(move + transform.position, blockingLayer);

                //sprawdzenie czy jest kafel zarezerwowany
                if (blocker == null)
                {
                    if (i == 0)
                        tileWithShortestDistance = currentlyConsideredTile;
                    i++;
                    everyTileOccupied = false;
                    float dist = (chaseTarget.transform.position - currentlyConsideredTile).magnitude;
                    float shortestDist = (chaseTarget.transform.position - tileWithShortestDistance).magnitude;
                    if (dist > shortestDist)
                        tileWithShortestDistance = currentlyConsideredTile;
                    /*else if (dist == shortestDist)
                        if (Vector3.Dot(move, Vector2.up) != 0)
                            tileWithShortestDistance = currentlyConsideredTile;*/

                }
            }

            if (everyTileOccupied)
            {
                MoveAssignement(transform.position);            //z braku lepszego pomysłu
            }
            else
            {
                MoveAssignement(tileWithShortestDistance);
            }
        }
        else
        {
            MoveAssignement(transform.position);
        }
    }
    public override void Move()
    {
        if (HasMoveAssigned&&!hidding)
        {
            Instantiate(Goo, transform.position, Quaternion.identity);
        }
        base.Move();
        anim.Play("Move");

    }
    public override void ReceiveDamage(int amount)
    {
        base.ReceiveDamage(amount);
        hidding = true;
    }
}
