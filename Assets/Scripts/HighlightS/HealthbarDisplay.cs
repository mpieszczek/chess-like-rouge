using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthbarDisplay : MonoBehaviour {

    public int healthPoints;
    public Pawn owner;
    public bool showVerticaly =false;
    public List<GameObject> emptyHearthsSprites = new List<GameObject>();
    public List<GameObject> filledHearthsSprites = new List<GameObject>();
    private bool itIsPlayer = false;
    List<GameObject> filledHearths = new List<GameObject>();

    public Vector3 middlePoint;
    public float length;
	// Use this for initialization
	void Start () {
        owner = transform.parent.gameObject.GetComponent<Pawn>();
        if (owner as Player != null)
            itIsPlayer = true;
        transform.localPosition = Vector3.zero;
        healthPoints = owner.GetHealth();               //geting amount of health
        
        List<Vector3> points = new List<Vector3>();    
        Vector3 beginPoint = middlePoint - new Vector3(length / 2, 0, 0);
        Vector3 stepBetween;
        if (healthPoints == 1)
            stepBetween = new Vector3(0, 0, 0);
        else if (!showVerticaly)
            stepBetween = new Vector3(length / (healthPoints - 1), 0, 0);  //Creating pionts where hearts will be
        else
            stepBetween = new Vector3(0, length / (healthPoints - 1), 0);

        for (int i = 0; i < healthPoints; i++)
        {
            points.Add(beginPoint + i * stepBetween);
        }
        
        for (int i = 0; i < healthPoints; i++)
        {
            int index = Random.Range(0, emptyHearthsSprites.Capacity);
            GameObject emptyHearth = Instantiate(emptyHearthsSprites[index], points[i] + transform.position, Quaternion.identity);   //powinno losować
            emptyHearth.transform.SetParent(transform);
            
            
            GameObject filledHearth = Instantiate(filledHearthsSprites[index], points[i] + transform.position, Quaternion.identity);
            if (itIsPlayer)
                filledHearth.GetComponent<SpriteRenderer>().color = new Color(0,0.75f,0,1);
            else
                filledHearth.GetComponent<SpriteRenderer>().color = new Color(0.8f, 0, 0, 1);
            filledHearth.transform.SetParent(transform);
            filledHearths.Add(filledHearth);
        }
	}
    
	public void UpdateHealth(int health)
    {
        for(int i =0;i<healthPoints;i++)
        {
            if (i < health)
                filledHearths[i].GetComponent<SpriteRenderer>().enabled = true;
            else
                filledHearths[i].GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
