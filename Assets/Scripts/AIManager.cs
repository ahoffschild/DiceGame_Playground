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
        for (int i = 0; i < gameManager.KeepDiceButtons.Length; i++)
        {
            gameManager.KeepDiceButtons[i].ToggleDice();
            yield return new WaitForSeconds(0.125f);
        }
        savingKeeps = false;
    }

    private void AIEvaluation()
    {
        Dice[] diceList = gameManager.Dicelist;
        int[] diceFaces = gameManager.diceFaces;

        saveRolls.Clear();

        //Saves any roll that cannot safely or meaningfully reroll
        if (gameManager.foundCombos.Contains(RollCombos.FullHouse) && gameManager.aiCombos[(int)RollCombos.FullHouse - 2] != 1)
        {
            for (int i = 0; i < diceList.Length; i++)
            {
                saveRolls.Add(i);
            }
        }

        if (gameManager.foundCombos.Contains(RollCombos.FourKind) && gameManager.aiCombos[(int)RollCombos.FourKind - 2] != 1)
        {
            for (int i = 0; i < diceList.Length; i++)
            {
                saveRolls.Add(i);
            }
        }

        if (gameManager.foundCombos.Contains(RollCombos.LargeStraight) && gameManager.aiCombos[(int)RollCombos.LargeStraight - 2] != 1)
        {
            for (int i = 0; i < diceList.Length; i++)
            {
                saveRolls.Add(i);
            }
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
                    Debug.Log("--");
                    Debug.Log(gameManager.aiCombos[0]);
                    Debug.Log(gameManager.aiCombos[1]);
                    Debug.Log(gameManager.aiCombos[2]);
                    Debug.Log(gameManager.aiCombos[3]);
                    Debug.Log(gameManager.aiCombos[4]);
                    Debug.Log(gameManager.aiCombos[5]);

                    //Add the array of ClaimButtons here and set any where their heldCombo == combo to claimed. Starts from top to bottom to prioritize high value combos
                    for (int i = GoalGUIManager.Instance.goalButtons.Length - 1; i >= 0 && !gameManager.turnClaimed; i--)
                    {
                        var holdingCombo = GoalGUIManager.Instance.goalButtons[i].holdingCombo;

                        if (gameManager.foundCombos.Contains(holdingCombo) && gameManager.aiCombos[(int)holdingCombo - 2] != 1)
                        {
                            GoalGUIManager.Instance.goalButtons[i].Claim();
                        }
                    }

                    state = AIStates.SelectKeeps;
                    break;

                case AIStates.SelectKeeps:
                    // AI determines it's logic pattern for keeps here, then selects thems

                    if (gameManager.rollsLeft > 0)
                    {
                        StartCoroutine(SelectKeeps());
                    }

                    state = AIStates.RollOrEnd;

                    break;

                case AIStates.RollOrEnd:
                {
                    //Determines if it should stop if it has no more rolls left or if it should wait for keeps then continue.
                    if (gameManager.rollsLeft > 0)
                    {
                        if (!savingKeeps)
                        {
                            state = AIStates.InitialRoll;
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
