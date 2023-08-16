using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightPushBack : MonoBehaviour {

    public Player owner;
    private void OnMouseDown()
    {
        owner.GetComponent<Knight>().PushBackAssign(this.transform.position);
    }
}
