using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsGUI : MonoBehaviour
{
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text rollTotalText;
    [SerializeField] TMP_Text rollsLeftText;
    // Doesn't need to be in update as the game only updates these GUI items when the dice roll anyways.
    public void UpdateStatsGUI()
    {
        rollTotalText.text = $"Total rolls: {DiceGameManager.Instance.rollCount}";
        rollsLeftText.text = $"Rolls left: {DiceGameManager.Instance.rollsLeft}";
        scoreText.text = $"Score: {DiceGameManager.Instance.score}";
    }
    private void Start()
    {
        UpdateStatsGUI();
    }
}
