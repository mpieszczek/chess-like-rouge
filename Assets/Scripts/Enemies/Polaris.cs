using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polaris : Enemy {

    public GameObject Goo;

    int movesPerTurn = 3;
    int move = 1;
    bool underground=false;
    protected override void Start()
    {
        base.Start();
        health = 5;
        favouriteAxis = new Vector3(1, 1, 0);//vertical
        //na ukos
        possibleMoves = new Vector3[] {new Vector3(1,0,0), new Vector3(-1, 0, 0), new Vector3(0, 1, 0),
            new Vector3(0,-1,0), new Vector3(1,1,0), new Vector3(-1, -1, 0), new Vector3(-1, 1, 0), new Vector3(1, -1, 0) };
        //ChooseTarget();
    }

    public override void ChooseTile()
    {
        if (underground)
        {
            return;
        }
        else
        {
            base.ChooseTile();
        }
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
        Vector3 currentPosition = transform.position;
        if (underground)
        {
            //odkopanie
            GetComponent<SpriteRenderer>().enabled = true;
            GetComponent<BoxCollider2D>().enabled = true;
            GetComponentInChildren<Transform>().gameObject.SetActive(true);
            underground = false;
            HasMoveAssigned = false;
            transform.position = pointToMove;
            GetComponent<SpriteRenderer>().sortingOrder = 100 - 10 * (int)transform.position.y + (int)transform.position.x;
            //anim.Play("CommingOut");//animacja odkopania

            //Jeśli uderzył w gracza odepchnijgo o jedno pole
            boxCollider.enabled = false;
            Collider2D blocker = Physics2D.OverlapPoint(transform.position, blockingLayer);
            if (blocker != null)
            {
                if (blocker.GetComponent<Pawn>() != null)
                {
                    blocker.GetComponent<Pawn>().ReceiveDamage(dmg);
                    //Jeśli udzerzył z powodzeniem to od razu się chowa by znów zaatakować
                    underground = true;
                    isMoving = false;
                    anim.Play("UndiggAttack");
                    ChooseTarget();
                    MoveAssignement(chaseTarget.transform.position);
                    Invoke("DisableVisibility", 0.5f);
                }
            }
            else
                anim.Play("UndiggNormal");
            boxCollider.enabled = true;
            isMoving = false;
        }
        else
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
            if (move > movesPerTurn)
            {
                Instantiate(Goo, currentPosition, Quaternion.identity);
                numberOfSpecialEnemiesMoving--;
                move = 1;
                underground = true;
                isMoving = false;
                anim.Play("Hide");//animacja zakopania
                ChooseTarget();
                MoveAssignement(chaseTarget.transform.position);
                HasAttackAssigned = false;
                Invoke("DisableVisibility", 0.6f);
            }
            else
            {
                if (!HasAttackAssigned)
                    Instantiate(Goo, currentPosition, Quaternion.identity);
                base.Move();
                anim.SetBool("isMoving", isMoving);
                //anim.SetBool("isMoving", true);
                //anim.Play("Move");
                Invoke("ChooseTile", 1f);
                Invoke("Move", 1.2f);
                move++;
            }
        }
    }
    private void DisableVisibility()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponentInChildren<Transform>().gameObject.SetActive(false);
    }
    void LateUpdate()
    {
        anim.SetBool("isMoving", isMoving);
    }
}
