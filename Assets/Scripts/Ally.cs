using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ally : Orb
{
    public int maxMovement;
    public float healthPoints;
    public float maxHp;
    public Sprite waiting;
    public Sprite ready;
    public enum moveStates
    {
        Unselected,
        Selected,
        Moved,
        Wait
    }
    [HideInInspector]
    public moveStates moveState;
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
        if(healthPoints <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        
    }

}
