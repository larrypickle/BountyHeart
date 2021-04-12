using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject healthBar;
    public GameObject talkBar;
    public float MaxHP;
    public float MaxHostility;
    private float currentHP;
    private float currentHostility;

    void Start()
    {
        currentHP = MaxHP;
        currentHostility = MaxHostility;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage()
    {
        currentHP--;
        Vector3 temp = healthBar.transform.localScale;
        temp.x = currentHP / MaxHP;
        healthBar.gameObject.transform.localScale = temp;
        StartCoroutine(characterFlash(Color.red));

        if(currentHP <= 0)
        {
            Debug.Log("Enemy Died");
        }
    }

    public void LowerHostility()
    {
        currentHostility--;
        Vector3 temp = talkBar.transform.localScale;
        temp.x = currentHostility / MaxHostility;
        talkBar.gameObject.transform.localScale = temp;
        StartCoroutine(characterFlash(Color.green));
        if (currentHostility <= 0)
        {
            Debug.Log("Enemy is chill with you now");
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
