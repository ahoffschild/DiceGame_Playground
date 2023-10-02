using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using UnityEngine.SearchService;

public class Dice : MonoBehaviour
{
    [HideInInspector]
    [SerializeField] Sprite[] DiceSprites;

    [SerializeField, Tooltip("The face value of the die.")]
    public int m_dieValue;
    public int dieLock;

    private void Awake()
    {
        dieLock = 0;
    }

    public int RollToRandomSide()
    {
        //If dice is locked to a particular value, it is set to that value unconditionally
        int newValue;
        //Debug.Log(dieLock);
        if (dieLock != 0)
        {
            newValue = dieLock;
        }
        else
        {
            newValue = Random.Range(1, 7);
        }
        gameObject.GetComponent<SpriteRenderer>().sprite = DiceSprites[newValue - 1];
        //setting die value to the rolled value. This wasn't done previously?
        m_dieValue = newValue;
        return newValue;
    }
}
