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


    public int RollToRandomSide()
    {
        int newValue = Random.Range(1, 6);
        gameObject.GetComponent<SpriteRenderer>().sprite = DiceSprites[newValue - 1];
        return newValue;
    }
}
