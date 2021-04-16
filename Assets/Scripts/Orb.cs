using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour
{
    [System.Serializable]
    public enum OrbType
    {
        Attack,
        Talk,
        Heal,
        Ally,
        Enemy,
        Empty
    }
    public OrbType orbType;
    //public Sprite sprite;

    //public GameObject icon;
    [HideInInspector]
    protected Vector2Int position;
    public Sprite enemySprite;
    public Sprite sprite;

    protected void Start()
    {
        //SetSprite(sprite);
    }
    public Vector2Int getPosition()
    {
        return position;
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
    public void BecomeEnemy()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = enemySprite;
        gameObject.tag = "Enemy";
        orbType = OrbType.Enemy;
    }

   





}
