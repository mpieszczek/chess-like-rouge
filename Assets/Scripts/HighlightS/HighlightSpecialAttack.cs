using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightSpecialAttack : MonoBehaviour {

    public Player owner;
    private void OnMouseDown()
    {
        owner.GetComponent<Player>().SpecialMoveAssign(gameObject);
    }
}
