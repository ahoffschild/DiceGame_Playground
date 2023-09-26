using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalGUIManager : MonoBehaviour
{
    
    [SerializeField]
    ClaimButton[] goalButtons;
    public static GoalGUIManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ProtectButtons()
    {
        foreach (ClaimButton button in goalButtons)
        {
            button.GetComponent<Button>().interactable = false;
        }
    }

    //Outdated method?
    public void ReleaseButtons()
    {
        foreach (ClaimButton button in goalButtons)
        {
            if (!button.m_goalClaimed) 
            {
                button.GetComponent<Button>().interactable = true;
            }
            
        }
    }
    #region -- CLAIMING COMBOS

    /// Create logic in each section that prevents you from claiming the combination before it's valid.  
    /// 

    // method invoked after each roll to verify combos
    public void EvaluateButtonsRoll()
    {
        foreach (ClaimButton button in goalButtons)
        {
            button.EvaluateDiceRoll();
        }
    }

    // method invoked after control switches from player to AI or vice versa
    public void EvaluateButtonsControl()
    {
        foreach (ClaimButton button in goalButtons)
        {
            button.EvaluateDiceTurn();
        }
    }

    #endregion
}
