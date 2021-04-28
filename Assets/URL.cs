using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class URL : MonoBehaviour, IPointerDownHandler
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log(this.gameObject.name + " Was Clicked.");
        //Application.OpenURL("https://forms.gle/hTLocvAP4urQcqkf7");
        string YourLink = "https://forms.gle/hTLocvAP4urQcqkf7";
        Application.ExternalEval("window.open('" + YourLink + "', '_blank')");

        ///this.gameObject.SetActive(false);

    }
}
