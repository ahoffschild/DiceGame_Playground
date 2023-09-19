using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    private int[] diceFaces;
    private List<int> saveRolls;

    public enum RollCombos
    {
        Pair = 1,
        TwoPair,
        ThreeKind = 3,
        FourKind,
        FullHouse,
        SmallStraight,
        LargeStraight
    }

    private RollCombos[] foundCombos;
    private RollCombos finalCombo;

    public Turns currentTurn;

    public enum Turns
    {
        Player,
        Computer
    }

    // Start is called before the first frame update
    void Start()
    {
        currentTurn = Turns.Player;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
