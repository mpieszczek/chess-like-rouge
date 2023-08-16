using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HighlightChosingTile : MonoBehaviour {

    public Player owner;
    private void OnMouseDown()
    {
        owner.MoveAssignement(transform.position);
        owner.UnhighlightTiles();
    }
}
