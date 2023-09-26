using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieLock : MonoBehaviour
{
    private int lockState;
    private Dice parentDice;
    SpriteRenderer spriteRenderer;
    [SerializeField] Sprite[] DiceSprites;

    private void Awake()
    {
        lockState = 0;
        spriteRenderer = GetComponent<SpriteRenderer>();
        parentDice = transform.parent.GetComponent<Dice>();
    }

    public void ChangeLock()
    {
        lockState++;
        if (lockState > 6)
        {
            lockState = 0;
        }

        if (lockState == 0)
        {
            spriteRenderer.color = Color.black;
        }
        else
        {
            spriteRenderer.color = Color.white;
            spriteRenderer.sprite = DiceSprites[lockState - 1];
        }

        parentDice.dieLock = lockState;
    }
}
