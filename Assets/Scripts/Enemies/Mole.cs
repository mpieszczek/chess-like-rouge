using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mole : Enemy {

    bool underground = false;
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
        //base.ChooseTile();
        if (!underground)
        {
            ChooseTarget();
            MoveAssignement(chaseTarget.transform.position);
        }
    }
    public override void Move()
    {
        if (HasMoveAssigned)
        {
            //HIDDING
            if (!underground)
            {
                underground = true;
                isMoving = false;
                anim.Play("Hide");//animacja zakopania
                Invoke("DisableVisibility", 0.6f);
            }
            else
            {
                GetComponent<SpriteRenderer>().enabled = true;
                GetComponent<BoxCollider2D>().enabled = true;
                GetComponentInChildren<Transform>().gameObject.SetActive(true);
                underground = false;
                HasMoveAssigned = false;
                transform.position = pointToMove;
                GetComponent<SpriteRenderer>().sortingOrder = 100 - 10 * (int)transform.position.y + (int)transform.position.x;
                anim.Play("CommingOut");//animacja odkopania

                //Jeśli uderzył w gracza odepchnijgo o jedno pole
                boxCollider.enabled = false;
                Collider2D blocker = Physics2D.OverlapPoint(transform.position, blockingLayer);
                if (blocker != null)
                {
                    if (blocker.GetComponent<Pawn>() != null)
                    {
                        blocker.GetComponent<Pawn>().ReceiveDamage(dmg);
                        //Jeśli udzerzył z powodzeniem to od razu się chowa by znów zaatakować
                        Invoke("MoveAgain", 0.2f);
                    }
                }
                boxCollider.enabled = true;
                isMoving = false;
            }
        }
            

        //base.Move();
    }
    private void DisableVisibility()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponentInChildren<Transform>().gameObject.SetActive(false);
    }
    private void MoveAgain()
    {
        ChooseTile();
        Move();
    }
}
