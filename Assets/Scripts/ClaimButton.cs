using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static DiceGameManager;

public class ClaimButton : MonoBehaviour
{
    [SerializeField,HideInInspector]
    TMP_Text buttonText;
    [SerializeField,HideInInspector]
    Button thisClaimButton;
    public bool m_goalClaimed = false;

    private DiceGameManager gameManager;

    // new variables

    [SerializeField] RollCombos holdingCombo;

    private void Awake()
    {
        thisClaimButton = GetComponent<Button>();
        gameManager = GameObject.Find("DiceManager").GetComponent<DiceGameManager>();
    }

    public void Claim()
    {
        m_goalClaimed = true;
        buttonText.text = "Claimed";
        thisClaimButton.interactable = false;
    }

    //Disable method? Behaves like claim, but only blocks Claim and has alternate text. Allows claim to be used for... claiming the combo

    public void Unclaim()
    {
        m_goalClaimed = false;
        buttonText.text = "Claim";
        thisClaimButton.interactable = true;
    }

    public void EvaluateDiceRoll()
    {
        if (gameManager.foundCombos.Contains(holdingCombo))
        {
            //Is one of the detected combos this chosen button's combo? If yes (and it's not claimed), make sure it's activated
            Unclaim();
        }
        else
        {
            //Ensure that when rerolling from the first roll that any potentially lost combos will be flagged unavailable again.
            Claim();
        }
    }

    public void EvaluateDiceTurn()
    {
        //Check the game manager for what combos have been evaluated and flagged for the current turn player
        if (gameManager.currentControl == Controller.Player)
        {
            // Claimable combos start at 2, array starts at 0
            if (gameManager.playerCombos[(int)(holdingCombo - 2)] == 1)
            {
                Claim();
            }
            else
            {
                Unclaim();
            }
        }
        else
        {
            if (gameManager.aiCombos[(int)(holdingCombo - 2)] == 1)
            {
                Claim();
            }
            else
            {
                Unclaim();
            }
        }
    }
}
