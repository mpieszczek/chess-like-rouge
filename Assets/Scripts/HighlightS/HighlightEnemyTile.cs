using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightEnemyTile : MonoBehaviour {

    public Player owner;
    public Enemy enemy;
    private void OnMouseDown()
    {
        owner.AttackAssignement(enemy);
        owner.UnhighlightTiles();
    }
}
