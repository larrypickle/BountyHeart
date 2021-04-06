using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ally : Orb
{
    private int maxMovement;
    public float healthPoints;
    public enum moveStates
    {
        Unselected,
        Selected,
        Moved,
        Wait
    }

    public moveStates moveState;
    public void Move()
    {
        
    }
    public int getMovement()
    {
        return maxMovement;
    }


}
