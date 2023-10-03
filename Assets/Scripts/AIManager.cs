using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static DiceGameManager;

public class AIManager : MonoBehaviour
{
    private bool aiActive, savingKeeps;
    private List<int> saveRolls;
    private DiceGameManager gameManager;

    public static AIManager Instance;
    public Button continueButton;
    private RollCombos holdingCombo;

    public int maxCounter;
    private int waitCounter;

    //Utilized so the AI knows what it's doing

    private enum AIStates
    {
        InitialRoll,
        RollWait,
        SelectCombos,
        SelectKeeps,
        RollOrEnd,
        Pause
    }
    
    private AIStates state;

    // AI is disabled at startup, set it as instance

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            aiActive = false;
            saveRolls = new List<int>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //Turns on AI and waits for player input to continue
    public void AITurnStart()
    {
        aiActive = true;
        state = AIStates.Pause;
        gameManager = DiceGameManager.Instance;
        continueButton.interactable = true;

        //Remove keep button interactivity?
    }

    //Used by Continue button
    public void AIProceed()
    {
        if (state == AIStates.Pause)
        {
            state = AIStates.InitialRoll;
        }
    }

    IEnumerator SelectKeeps()   
    {
        savingKeeps = true;
        for (int i = 0; i < gameManager.Dicelist.Length; i++)
        {
            if (saveRolls.Contains(i))
            {
                gameManager.KeepDiceButtons[i].ToggleDice();
            }
            yield return new WaitForSeconds(0.125f);
        }
        savingKeeps = false;
    }

    private void AIEvaluation()
    {
        Dice[] diceList = gameManager.Dicelist;
        int[] diceFaces = gameManager.diceFaces;

        saveRolls.Clear();

        //Debug.Log(gameManager.lookFor);

        //Logic for saving runs
        if (gameManager.lookFor != 0 && gameManager.aiCombos[(int)RollCombos.SmallStraight - 2] == 0 || gameManager.lookFor != 0 && gameManager.aiCombos[(int)RollCombos.LargeStraight - 2] == 0)
        {
            bool foundNumber;
            for (int i = gameManager.lookFor; i <= i + 4 && i <= 6; i++)
            {
                foundNumber = false;
                for (int j = 0; j < diceList.Length; j++)
                {
                    if (diceList[j].m_dieValue == i && foundNumber == false)
                    {
                        saveRolls.Add(j);
                        Debug.Log($"Keeping dice {j}");
                        foundNumber = true;
                    }
                }
            }
        }

        //Logic for saving twoPair/fullHouse
        if (saveRolls.Count < 1 && gameManager.aiCombos[(int)RollCombos.FullHouse - 2] == 0 || saveRolls.Count < 1 && gameManager.aiCombos[(int)RollCombos.TwoPair - 2] == 0)
        {
            bool savedSingle = false;
            int saveTarget = 2;
            // AI will only look for groups of 3 when looking for full house, not Two Pair.
            if (gameManager.aiCombos[(int)RollCombos.FullHouse - 2] == 0 && gameManager.foundCombos.Contains(RollCombos.Pair) || gameManager.foundCombos.Contains(RollCombos.ThreeKind))
            {
                saveTarget = 3;
            }
            for (int i = 0; i < diceList.Length; i++)
            {
                int saves = 0;
                if (diceFaces[i] > 1)
                {
                    for (int j = 0; j < diceList.Length && saves < saveTarget; j++)
                    {
                        if (diceList[j].m_dieValue == i + 1)
                        {
                            saveRolls.Add(j);
                            saves++;
                        }
                    }
                }
            }
            if (saveRolls.Count < saveTarget + 1)
            {
                for (int i = 0;i < diceList.Length && !savedSingle; i++)
                {
                    //Prevent it from selecting a singular dice that is already on the list, or shares a face with the existing dice.
                    if (!saveRolls.Contains(i))
                    {
                        if (saveRolls.Count == 0)
                        {
                            saveRolls.Add(i);
                            savedSingle = true;
                        }
                        else if (diceList[i].m_dieValue != diceList[saveRolls[0]].m_dieValue)
                        {
                            saveRolls.Add(i);
                            savedSingle = true;
                        }
                    }
                }
            }
            
        }

        //Logic for saving 4/3

        if (saveRolls.Count < 1 && gameManager.aiCombos[(int)RollCombos.ThreeKind - 2] == 0 || saveRolls.Count < 1 && gameManager.aiCombos[(int)RollCombos.FourKind - 2] == 0)
        {
            if (gameManager.foundCombos.Contains(RollCombos.Pair) || gameManager.foundCombos.Contains(RollCombos.ThreeKind))
            {
                for (int i = 0; i < diceFaces.Length; i++)
                {
                    if (diceFaces[i] > 1)
                    {
                        for (int j = 0; j < diceList.Length; j++)
                        {
                            if (diceList[j].m_dieValue == i + 1)
                            {
                                saveRolls.Add(j);
                            }
                        }
                    }
                }
            }
        }

        if (saveRolls.Count < 1)
        {
            saveRolls.Add(0);
        }
    }

    private void Update()
    {
        if (aiActive)
        {
            switch (state) 
            {
                // AI rolls the dice then waits for it to finish.
                case AIStates.InitialRoll:
                    continueButton.interactable = false;
                    gameManager.Roll();
                    state = AIStates.RollWait;
                    break;

                //AI waits for diceroll to finish.
                case AIStates.RollWait:
                    if (!gameManager.isRolling)
                    {
                        state = AIStates.SelectCombos;
                    }
                    break;

                case AIStates.SelectCombos:
                    // Combos if seen will be flagged by AI here

                    /*Debug.Log("--");
                    Debug.Log(gameManager.aiCombos[0]);
                    Debug.Log(gameManager.aiCombos[1]);
                    Debug.Log(gameManager.aiCombos[2]);
                    Debug.Log(gameManager.aiCombos[3]);
                    Debug.Log(gameManager.aiCombos[4]);
                    Debug.Log(gameManager.aiCombos[5]);*/

                    //Add the array of ClaimButtons here and set any where their heldCombo == combo to claimed. Starts from top to bottom to prioritize high value combos
                    for (int i = GoalGUIManager.Instance.goalButtons.Length - 1; i >= 0 && !gameManager.turnClaimed; i--)
                    {
                        holdingCombo = GoalGUIManager.Instance.goalButtons[i].holdingCombo;

                        if (gameManager.foundCombos.Contains(holdingCombo) && gameManager.aiCombos[(int)holdingCombo - 2] != 1)
                        {
                            //Branch to delay claim if they have a better option
                            if (gameManager.rollsLeft > 0 && holdingCombo == RollCombos.ThreeKind && gameManager.aiCombos[(int)RollCombos.FourKind - 2] == 0)
                            {
                                //Four of a kind branch
                            }
                            else if (gameManager.rollsLeft > 0 && holdingCombo == RollCombos.TwoPair && gameManager.aiCombos[(int)RollCombos.FullHouse - 2] == 0)
                            {
                                //Full House branch
                            }
                            else if (gameManager.rollsLeft > 0 && holdingCombo == RollCombos.SmallStraight && gameManager.aiCombos[(int)RollCombos.LargeStraight - 2] == 0)
                            {
                                //Large Straight branch
                            }
                            else
                            {
                                GoalGUIManager.Instance.goalButtons[i].Claim();
                            }

                        }
                        else
                        {
                            //Debug.Log("loop runs, if fails");
                        }
                    }

                    state = AIStates.SelectKeeps;
                    break;

                case AIStates.SelectKeeps:
                    // AI determines it's logic pattern for keeps here, then selects thems

                    if (gameManager.rollsLeft > 0)
                    {
                        AIEvaluation();
                        StartCoroutine(SelectKeeps());
                    }

                    waitCounter = 0;
                    state = AIStates.RollOrEnd;

                    break;

                case AIStates.RollOrEnd:
                {
                    //Determines if it should stop if it has no more rolls left or if it should wait for keeps then continue.
                    if (gameManager.rollsLeft > 0)
                    {
                        if (!savingKeeps)
                        {
                            if (waitCounter >= maxCounter)
                            {
                                state = AIStates.InitialRoll;
                            }
                            else
                            {
                                waitCounter++;
                            }
                        }
                    }
                    else
                    {
                        state = AIStates.Pause;
                    }
                    break;
                }

                //Ai does nothing when it's on pause/default. This is to wait for the player to prompt a continue of the AI.
                default:
                    if (!continueButton.interactable)
                    {
                        continueButton.interactable = true;
                    }
                    break;
            }
        }
    }
}
