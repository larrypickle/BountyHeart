using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ally : Orb
{
    public int maxMovement;
    public float healthPoints;
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


}
