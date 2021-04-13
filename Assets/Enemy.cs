using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject healthBar;
    public GameObject talkBar;
    public float MaxHP;
    public float MaxHostility;
    private float currentHP;
    private float currentHostility;
    public TextMeshProUGUI HPText;
    public TextMeshProUGUI HostilityText;

    [Header ("Dialogue")]
    public string enterDialogue;
    public string friendlyDialogue;
    public string recruitDialogue;
    public string deathDialogue;
    public Dialogue dialogue;
    public Sprite dialogueSprite;

    private bool friendly;

    void Start()
    {
        friendly = false;
        currentHP = MaxHP;
        currentHostility = MaxHostility;
        HPText.SetText(currentHP + " / " + MaxHP);
        HostilityText.SetText(currentHostility + " / " + MaxHostility);
        dialogue.DisplayText(enterDialogue, dialogueSprite);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float attack)
    {
        currentHP -= attack;
        Vector3 temp = healthBar.transform.localScale;
        temp.x = currentHP / MaxHP;
        healthBar.gameObject.transform.localScale = temp;
        HPText.SetText(currentHP + " / " + MaxHP);
        StartCoroutine(characterFlash(Color.red));

        

        if (currentHP <= 0)
        {
            dialogue.DisplayText(deathDialogue, dialogueSprite);
            Debug.Log("Enemy Died");
        }

    }

    public void LowerHostility(float charisma)
    {
        currentHostility -= charisma;
        Vector3 temp = talkBar.transform.localScale;
        temp.x = currentHostility / MaxHostility;
        talkBar.gameObject.transform.localScale = temp;
        HostilityText.SetText(currentHostility + " / " + MaxHostility);
        StartCoroutine(characterFlash(Color.green));
        
        if(currentHostility <= MaxHostility / 2 && !friendly)
        {
            friendly = true;
            dialogue.DisplayText(friendlyDialogue, dialogueSprite);

        }

        if (currentHostility <= 0)
        {
            Debug.Log("Enemy is chill with you now");
            dialogue.DisplayText(recruitDialogue, dialogueSprite);
        }
    }
    


    private IEnumerator characterFlash(Color color)
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        float flashingFor = 0;
        float flashSpeed = 0.1f;
        float flashTime = 0.3f;
        var flashColor = color;
        var newColor = flashColor;
        var originalColor = Color.white;
        while (flashingFor < flashTime)
        {
            sprite.color = newColor;
            flashingFor += Time.deltaTime;
            yield return new WaitForSeconds(flashSpeed);
            flashingFor += flashSpeed;
            if (newColor == flashColor)
            {
                newColor = originalColor;
            }
            else
            {
                newColor = flashColor;
            }
        }
        sprite.color = originalColor;
    }
}
