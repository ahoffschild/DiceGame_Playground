using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceGameManager : MonoBehaviour
{
    public Dice[] Dicelist;
    public DiceButton[] KeepDiceButtons;
    
    public bool isRolling;

    public static DiceGameManager Instance;

    public int rollCount = 0;
    public int score = 0;
    public int rollsLeft = 2;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            rollCount = 0;
            score = 0;
            rollsLeft = 2;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Roll()
    {
        if (!isRolling)
        {
            StartCoroutine(RollAllDice());
        }
    }

    IEnumerator RollAllDice()
    {
        isRolling = true;
        CheckRollsLeft();
        GoalGUIManager.Instance.ProtectButtons();
        for (int d = 0; d < Dicelist.Length; d++)
        {
            
            if (KeepDiceButtons[d].m_keepDice) 
            {
                continue;
            }
            else
            {
                Dicelist[d].RollToRandomSide();
            }
            yield return new WaitForSeconds(0.125f);
        }
        isRolling = false;
        GoalGUIManager.Instance.ReleaseButtons();
        rollCount += 1;
        rollsLeft -= 1;
    }

    void CheckRollsLeft()
    {
        if(rollsLeft < 0)
        {
            foreach (var d in KeepDiceButtons)
            {
                d.ResetDice();
            }
            rollsLeft = 2;
        }
    }
}
