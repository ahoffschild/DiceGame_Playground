using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DiceButton : MonoBehaviour
{
    [SerializeField] Dice parentDice;
    [Tooltip("If true, the die will not roll when the roll button is pressed.")]
    public bool m_keepDice = false;

    [SerializeField] TMP_Text buttonText;
    private void Awake()
    {
        buttonText = GetComponentInChildren<TMP_Text>();
    }

    public void ToggleDice()
    {
        if (m_keepDice)
        {
            m_keepDice = false;
        }
        else
        {
            m_keepDice = true;
        }
        buttonText.text = m_keepDice ? "Keep" : "Roll";
    }

    public void ResetDice()
    {
        m_keepDice = false;
        buttonText.text = "Roll";
    }
}
