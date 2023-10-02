using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISwapper : MonoBehaviour
{
    DiceGameManager gameManager;
    public GameObject target1, target2;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = DiceGameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.currentControl == DiceGameManager.Controller.Player && !target1.activeInHierarchy)
        {
            target1.SetActive(true);
            target2.SetActive(false);
        }
        if (gameManager.currentControl == DiceGameManager.Controller.Computer && !target2.activeInHierarchy)
        {
            target1.SetActive(false);
            target2.SetActive(true);
        }
    }
}
