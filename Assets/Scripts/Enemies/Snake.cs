using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : Enemy {

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
            if ( currentDistance < chaseingDistance)
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
}
