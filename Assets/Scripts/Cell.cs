using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    SpriteRenderer CellSprite;
    [SerializeField] bool IsAlive = false;

    // Start is called before the first frame update
    void Awake()
    {
        CellSprite = GetComponent<SpriteRenderer>();
    }
    
    public bool GetIsAlive()
    {
        return IsAlive;
    }

    public void SetIsAlive(bool alive)
    {
        IsAlive = alive;

        if (!IsAlive)
            CellSprite.color = Color.white;
        else
            CellSprite.color = Color.black;
    }

    public void SetScale(float scale)
    {
        transform.localScale = new Vector2(scale, scale);
    }
}
