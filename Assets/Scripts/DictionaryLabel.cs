using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class DictionaryLabel : MonoBehaviour, IPointerExitHandler
{

    bool labelExists = false;
    
    public GameObject LabelPrefab;

    public void ShowLabel(Pawn pawn)
    {
        //tuturutu tu znalazłem informacje o tej postaci

        GameObject Label;
        Label = Instantiate(LabelPrefab);
        Label.transform.SetParent(transform, false);
        labelExists = true;
        Text nameText=null;
        Text descriptionText=null;
        Text statsText=null;
        Image portraitImage=null;
        foreach(Transform child in Label.transform)
        {
            if (child.name == "Name")
                nameText = child.GetComponent<Text>();
            else if (child.name == "Description")
                descriptionText = child.GetComponent<Text>();
            else if (child.name == "Stats")
                statsText = child.GetComponent<Text>();
            else if (child.name == "Portrait")
                portraitImage = child.GetComponent<Image>();
        }
        if (nameText != null && descriptionText != null && statsText != null && portraitImage != null)
        {
            nameText.text = pawn.gameObject.name;
        }
        else
            Destroy(gameObject);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("The cursor entered the selectable UI element.");
        if (labelExists)
        {
            Destroy(gameObject);
        }
    }
}
