using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryEffect : MonoBehaviour {

    protected int timeinTurns;

    protected virtual void Start()
    {
        GameManager.instance.effects.Add(this);
    }
	public void Fattigue()
    {
        
        timeinTurns = timeinTurns - 1;
        if (timeinTurns<=0)
        {
            Destroy(this.gameObject);
        }
    }
}
