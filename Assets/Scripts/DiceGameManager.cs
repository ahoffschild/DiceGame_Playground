using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceGameManager : MonoBehaviour
{
    public Dice[] Dicelist;
    public DiceButton[] KeepDiceButtons;
    public Button swapButton;
    
    public bool isRolling, turnClaimed;

    public static DiceGameManager Instance;

    public int rollCount = 0;
    public int score = 0;
    public int rollsLeft = 0;
    public int lookFor;
    private int rollsMax = 1;

    // new variables for AI project

    //public variables that the AI will read + are used here

    public Controller currentControl;
    public List<RollCombos> foundCombos;
    public int[] diceFaces;
    public int[] playerCombos;
    public int[] aiCombos;

    // Enums for project usage.

    public enum Controller
    {
        Player, 
        Computer
    }

    public enum RollCombos
    {
        Pair = 1,
        TwoPair = 2,
        ThreeKind = 3,
        FourKind = 4,
        FullHouse = 5,
        SmallStraight = 6,
        LargeStraight = 7
    }

    //Former utilized while evaluating pairs. Latter used to contain all claimable combos

    private RollCombos[] pairCombos;

    //

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            rollCount = 0;
            score = 0;
            rollsLeft = 0;

            currentControl = Controller.Player;

            playerCombos = new int[6];
            aiCombos = new int[6];
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Roll()
    {
        if (rollsLeft == 0)
        {
            turnClaimed = false;
        }

        if (!isRolling)
        {
            diceFaces = new int[6];
            swapButton.interactable = false;
            StartCoroutine(RollAllDice());

            //Locks out swap control while rolling.
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

        //Post-roll functions.
        EvaluateCombos();
        //Replaced from release buttons so the buttons will accurately check for combos.
        GoalGUIManager.Instance.EvaluateButtonsRoll();
        rollCount += 1;
        StatsGUI.Instance.UpdateStatsGUI();

        if (rollsLeft == 0 && !swapButton.interactable)
        {
            swapButton.interactable = true;
        }
    }

    public void ClaimTurnEnd ()
    {
        turnClaimed = true;
        GoalGUIManager.Instance.ProtectButtons();
        rollsLeft = 0;
        swapButton.interactable = true;
        StatsGUI.Instance.UpdateStatsGUI();
    }

    public void SwapControl()
    {
        if (currentControl == Controller.Player)
        {
            currentControl = Controller.Computer;
            GoalGUIManager.Instance.EvaluateButtonsControl();
            AIManager.Instance.AITurnStart();
            GoalGUIManager.Instance.ProtectButtons();
            //Protect buttons here.
        }
        else
        {
            currentControl = Controller.Player;
            GoalGUIManager.Instance.EvaluateButtonsControl();
            GoalGUIManager.Instance.ProtectButtons();
            //Protect buttons here.
        }
        turnClaimed = false;
    }

    void EvaluateCombos()
    {
        lookFor = 0;
        //Insert combo logic here. If a combo is found, add it to the saved list.

        //Clear dicefaces, then saving how much of each face is present for future usage.
        foundCombos.Clear();
        diceFaces = new int[6];

        foreach (Dice dice in Dicelist)
        {
            diceFaces[dice.m_dieValue - 1]++;
        }

        //Group processing

        pairCombos = new RollCombos[2];
        var currentSlot = 0;

        for (int i = 0;  i < diceFaces.Length; i++)
        {
            //Add threekind or both if greater than four

            if (diceFaces[i] > 3)
            {
                foundCombos.Add(RollCombos.FourKind);
            }
            if (diceFaces[i] > 2)
            {
                foundCombos.Add(RollCombos.ThreeKind);
            }
            if (diceFaces[i] > 1)
            {
                foundCombos.Add(RollCombos.Pair);
            }

            //Add threekind and pairs found to the pairCombos which well then be checked

            if (diceFaces[i] == 3)
            {
                pairCombos[currentSlot] = RollCombos.ThreeKind;
                currentSlot++;
            }
            if (diceFaces[i] == 2)
            {
                pairCombos[currentSlot] = RollCombos.Pair;
                currentSlot++;
            }

        }

        //checking pairCombos
        if (pairCombos[1] != 0)
        {
            //If threekind is present, it must be a full house
            if (pairCombos[0] == RollCombos.ThreeKind || pairCombos[1] == RollCombos.ThreeKind)
            {
                foundCombos.Add(RollCombos.FullHouse);
            }
            foundCombos.Add(RollCombos.TwoPair);
        }

        //Straight processing

        if (foundCombos.Count == 0 || foundCombos.Count == 1 && foundCombos.Contains(RollCombos.Pair))
        {
            int amountCounted, brokenStreak;
            for (int i = 1; i <= 3; i++)
            {
                //amountCounted is the amount of numbers in the run that were found. brokenStreak records where and when they were missing a number
                amountCounted = 0;
                brokenStreak = 0;
                //i - 1 is the diceFaces identity, so i + 3 is a 5-size range despite adding 3 and not 4.
                for (int j = i - 1; j < i + 4 && j <= 5; j++)
                {
                    //Debug.Log($"J = {j}, I = {i}");
                    if (diceFaces[j] > 0)
                    {
                        amountCounted++;
                    }
                    else if (brokenStreak == 0)
                    {
                        brokenStreak = j + 1;
                    }
                }
                // If amountCounted is 5, it can only be LargeStraight. Second condition prevents it from potentially looping

                //Debug.Log($"broken at: {brokenStreak}, i: {i}");
                if (amountCounted > 4)
                {
                    foundCombos.Add(RollCombos.LargeStraight);
                }
                if (amountCounted > 3)
                {
                    //Second condition prevents it from looping multiple smallstraights.
                    if (brokenStreak == i + 4 || brokenStreak == 0 && !foundCombos.Contains(RollCombos.SmallStraight))
                    {
                        foundCombos.Add(RollCombos.SmallStraight);
                    }
                    else
                    {
                        //lookFor will go here
                        lookFor = i;
                    }
                }
                // add lookFor == 0 to this if branch once implemented
                if (amountCounted == 3 && lookFor == 0)
                {
                    //lookFor will also go here.
                    lookFor = i;
                }
            }
        }

        foreach (RollCombos combos in foundCombos)
        {
            //Debug.Log(combos.ToString());
        }
    }

    void CheckRollsLeft()
    {
        rollsLeft -= 1;
        if (rollsLeft < 0)
        {
            foreach (var d in KeepDiceButtons)
            {
                d.ResetDice();
            }
            rollsLeft = rollsMax;
        }
    }
}