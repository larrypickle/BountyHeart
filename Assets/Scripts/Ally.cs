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

    [HideInInspector]
    public float healthPoints;
    public GameObject healthBar;

    public enum moveStates
    {
        Unselected,
        Selected,
        Moved,
        Wait
    }
    [HideInInspector]
    public moveStates moveState;

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
            Die();
        }
    }
    private void Die()
    {
        Debug.Log("Character " + name + "fucking died");
    }
    private void DisplayAll()
    {

    }

}
