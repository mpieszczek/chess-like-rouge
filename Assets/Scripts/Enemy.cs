using System.Collections;
using System.Collections.Generic;
//using Random = UnityEngine.Random;
using UnityEngine;

public class Enemy : Pawn {
    protected int radiusOfSight;
    public static int numberOfSpecialEnemiesMoving=0;
    protected static List<Vector3> ChoosedTiles = new List<Vector3>();
    public bool targetInSight = false;
    public Player chaseTarget;
    protected Vector3 favouriteAxis;
    protected Vector3[] possibleMoves;
    public virtual void ChooseTile()
    {

        //Searching for target

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
                    if (dist < shortestDist)
                        tileWithShortestDistance = currentlyConsideredTile;
                    /*else if (dist == shortestDist)
                        if (Vector3.Dot(move, Vector2.up) != 0)
                            tileWithShortestDistance = currentlyConsideredTile;*/

                }
                else if (blocker.gameObject.tag == "Player")
                {
                    if (blocker.gameObject.GetComponent<Player>() == chaseTarget)
                    {
                        AttackAssignement(blocker.gameObject.GetComponent<Player>());
                        return;
                    }
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
        base.Move();
        ChoosedTiles.Clear();
    }
    
    protected void AttackAssignement(Player target)
    {
        HasAttackAssigned = true;
        targetEnemy = target;
        Vector3 offset = (transform.position - target.transform.position).normalized;
        offset.x = Mathf.RoundToInt(offset.x);
        offset.y = Mathf.RoundToInt(offset.y);
        offset.z = Mathf.RoundToInt(offset.z);
        MoveAssignement(target.transform.position + offset);
    }
    public override void MoveAssignement(Vector3 moveAssigned)
    {
        base.MoveAssignement(moveAssigned);
        ChoosedTiles.Add(moveAssigned);
    }
    protected virtual void ChooseTarget()
    {
        Debug.Log("Jestem nieprzeciążoną metodą");
    }

}
