using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goo : TemporaryEffect {

    protected override void Start()
    {
        base.Start();
        timeinTurns = 3;
    }
	void OnTriggerEnter2D(Collider2D collision)
    {

        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null)
        {
            player.ReceiveDamage(1);
        }

    }
}
