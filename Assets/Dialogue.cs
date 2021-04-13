using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class Dialogue : MonoBehaviour, IPointerDownHandler
{
    public TextMeshProUGUI dialogue;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log(this.gameObject.name + " Was Clicked.");
        this.gameObject.SetActive(false);

    }

    public void DisplayText(string text, Sprite speaker)
    {
        this.gameObject.SetActive(true);
        dialogue.SetText(text);
    }
    public void CloseDialogueWindow()
    {
        this.gameObject.SetActive(false);
    }

}
