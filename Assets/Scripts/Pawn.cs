using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pawn : MonoBehaviour {
    public float MoveTime = .5f;
    public LayerMask blockingLayer;
    public int health = 3;

    protected BoxCollider2D boxCollider;
    protected float inverseMoveTime;
    protected BoardManager boardManager;
    protected GameManager gameManager;
    public bool isMoving=false;
    public int dmg=1;
    public Vector3 pointToMove;
    public bool HasMoveAssigned = false;
    public bool HasAttackAssigned = false;
    protected Pawn targetEnemy;
    protected Animator anim;
    protected virtual void Start()
    {

        boxCollider = GetComponent<BoxCollider2D>();
        boardManager = GameManager.instance.GetComponent<BoardManager>();
        gameManager = GameManager.instance.GetComponent<GameManager>();
        anim = GetComponent<Animator>();
        if (boardManager == null)
            Debug.Log("no i kupa");
        inverseMoveTime = 1f / MoveTime;
        GetComponent<SpriteRenderer>().sortingOrder = 100 - (10 * (int)transform.position.y + (int)transform.position.x);
    }
    public virtual void MoveAssignement(Vector3 moveAssigned)              
    {
        HasMoveAssigned = true;
        pointToMove = moveAssigned;
        pointToMove.z = -1f;
    }
    void OnMouseOver()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            Debug.Log("Informacje");
            //Give Info As Pop Up
            GameObject instance = new GameObject("Label", typeof(RectTransform));
            instance.transform.SetParent(FindObjectOfType<Canvas>().transform);
            instance.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);
            instance.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            instance.GetComponent<RectTransform>().position = Input.mousePosition;
            instance.transform.localRotation= Quaternion.identity;
            instance.AddComponent<DictionaryLabel>();
            DictionaryLabel Label = instance.GetComponent<DictionaryLabel>();
            Label.LabelPrefab = gameManager.InfoLabelTemplate;
            Label.ShowLabel(this);
        }
    }
    void CreateLabel()
    {

    }
    public int GetHealth() { return health; }
    public virtual void Move()
    {
        if (HasMoveAssigned)
        {
            HasMoveAssigned = false;
            isMoving = true;
            if (HasAttackAssigned)
            {
                HasAttackAssigned = false;
                if (targetEnemy != null)
                {
                    targetEnemy.ReceiveDamage(dmg);
                }
                anim.Play("Attack");
            }
        }
    }
    public virtual void PushBacked()
    {
        Move();
    }
    public virtual void ReceiveDamage(int amount)
    {
        health = health - amount;
        GetComponentInChildren<HealthbarDisplay>().UpdateHealth(health);
        if (health <= 0)
        {
            gameManager.DestroyPawn(this);
            //anim.Play("Damage");
            anim.Play("Death");
            Invoke("Death", 1);
        }
        else
        {
            anim.Play("Damage");
        }
    }
    public virtual void Death()
    {
        Destroy(gameObject);
    }
    protected virtual void FixedUpdate()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, pointToMove, Time.fixedDeltaTime * inverseMoveTime);
            if((transform.position -pointToMove).magnitude < Mathf.Epsilon)
            {
                isMoving = false;
                GetComponent<SpriteRenderer>().sortingOrder = 100-10 * (int)transform.position.y + (int)transform.position.x;
            }
        }
    }

}