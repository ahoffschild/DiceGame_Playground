using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DieLock : MonoBehaviour
{
    private int lockState;
    private Dice parentDice;
    Image imageRender;
    [SerializeField] Sprite[] DiceSprites;

    private void Awake()
    {
        lockState = 0;
        imageRender = GetComponent<Image>();
        parentDice = transform.parent.parent.GetComponent<Dice>();
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
            imageRender.color = Color.black;
        }
        else
        {
            imageRender.color = Color.grey;
            imageRender.sprite = DiceSprites[lockState - 1];
            parentDice.dieLock = lockState;
        }

        parentDice.dieLock = lockState;
    }
}
