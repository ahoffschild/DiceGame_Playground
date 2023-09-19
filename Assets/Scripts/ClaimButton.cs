using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ClaimButton : MonoBehaviour
{
    [SerializeField,HideInInspector]
    TMP_Text buttonText;
    [SerializeField,HideInInspector]
    Button thisClaimButton;
    public bool m_goalClaimed = false;

    private void Awake()
    {
        thisClaimButton = GetComponent<Button>();
    }

    public void Claim()
    {
        m_goalClaimed = true;
        buttonText.text = "Claimed";
        thisClaimButton.interactable = false;
    }

    public void EvaluateDice()
    {
        //Get list of dice, run through my listed evaluation logic
    }
}
