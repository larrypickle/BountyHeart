using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ally : Orb
{
    //character stats
    public string characterName;
    public int maxMovement;
    public float attack;
    public float charisma;
    public float healing;
    public float maxHp;
    public Sprite waiting;
    public Sprite ready;
    public Sprite portrait;

    [HideInInspector]
    public float healthPoints;
    public GameObject healthBar;

    public string deathText;

    public enum moveStates
    {
        Unselected,
        Selected,
        Moved,
        Wait
    }
    [HideInInspector]
    public moveStates moveState;
    public List<GameObject> orbSprites;

    new private void Start()
    {
        healthPoints = maxHp;        
    }
    public void Move()
    {
        
    }
    public int getMovement()
    {
        return maxMovement;
    }
    public void Wait()
    {
        this.GetComponent<SpriteRenderer>().sprite = waiting;
        this.moveState = moveStates.Wait;
    }
    public void Ready()
    {
        this.GetComponent<SpriteRenderer>().sprite = ready;
        this.moveState = moveStates.Unselected;
    }

    public void TakeDamage()
    {
        //should return current hp / max hp so that can be applied to the health bar
        healthPoints--;
        Vector3 temp = healthBar.transform.localScale;
        temp.x = healthPoints / maxHp;

        Debug.Log("Character" + name + "took damage. HP is at " + healthPoints + ", max HP is " + maxHp + "healthbar.x = " + temp.x);

        healthBar.gameObject.transform.localScale = temp;
        if(healthPoints <= 0)
        {
            StartCoroutine(Die());
        }
    }
    public void GainHealth()
    {
        
        healthPoints += healing / maxHp;
        if(healthPoints >= maxHp)
        {
            healthPoints = maxHp;
        }
        Vector3 temp = healthBar.transform.localScale;
        temp.x = healthPoints / maxHp;

        Debug.Log("Character" + name + "gained " + healing / maxHp + "HP. HP is at " + healthPoints + ", max HP is " + maxHp + "healthbar.x = " + temp.x);

        healthBar.gameObject.transform.localScale = temp;
        

    }

    private IEnumerator Die()
    {
        yield return new WaitForSeconds(0.5f);
        Debug.Log("Character " + name + "fucking died");
        //FindObjectOfType<Dialogue>().DisplayText(deathText, this.GetComponent<SpriteRenderer>().sprite);
        GameObject orb = orbSprites[Random.Range(0, orbSprites.Count)];
        
        this.GetComponent<SpriteRenderer>().sprite = orb.GetComponent<SpriteRenderer>().sprite;
        this.orbType = orb.GetComponent<Orb>().orbType;
        this.tag = "Untagged";
    }
    
    

}
