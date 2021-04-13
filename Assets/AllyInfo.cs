using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class AllyInfo : MonoBehaviour
{
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI hp;
    public TextMeshProUGUI attack;
    public TextMeshProUGUI charisma;
    public TextMeshProUGUI healing;
    public GameObject characterSprite;

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetValues(Ally ally)
    {
        characterName.SetText(ally.characterName);
        hp.SetText("HP: " + ally.healthPoints.ToString("F1") + "/" + ally.maxHp.ToString("F1"));
        attack.SetText("Attack: " + ally.attack.ToString());
        charisma.SetText("Charisma: " + ally.charisma.ToString());
        healing.SetText("Healing: " + ally.healing.ToString());
        characterSprite.GetComponent<Image>().sprite = ally.portrait;

    }
}
