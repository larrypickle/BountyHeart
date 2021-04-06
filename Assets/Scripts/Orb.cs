using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour
{
    [System.Serializable]
    public enum OrbType
    {
        Attack,
        Shield,
        Heal,
        Ally,
        Enemy
    }
    public OrbType orbType;
    //public Sprite sprite;

    //public GameObject icon;
    [HideInInspector]
    protected Vector2Int position;

    protected void Start()
    {
        //SetSprite(sprite);
    }
    public Vector2Int getPosition()
    {
        return position;
    }

    public void SwapOrbs()
    {

    }

    public void SetPosition(int posX, int posY)
    {
        position.x = posX;
        position.y = posY;
    }

    /*public void SetSprite(Sprite s)
    {
        sprite = s;
        this.GetComponent<SpriteRenderer>().sprite = s;
    }*/

   





}
